# Mars Rover Challenge

This repository contains a C# solution to the Mars Rover Challenge, where we're trying to create a program to represent Mars Rovers moving around a **Plateau** (a high ground surface with a well defined boundary)

Here we have 3 folders:

1. The `MarsRover` folder contains the C# solution to the challenge
2. The `MarsRover.Tests` folder contains the unit tests for the solution
3. The `diagrams` folder contains diagrams related to the solution

# Instructions

To run the application:

1. clone the repository to your computer
2. then navigate to the `MarsRover` folder (with `cd` command or otherwise)
3. then run the following command

```c#
dotnet run
```

# Demo

![alt text](diagrams/appDemo.gif "app demo GIF")

# Key Features

The application can:

1. create a **plateau**, based on user input
2. create **multiple obstacles** on the plateau, based on user input
3. create **multiple vehicles** on the Plateau, based on user input
4. move vehicles on the Plateau by reading **movement instruction**, such as "LMMRMMMRLMRMMLMR", from user input
5. print the vehicle's updated position after applying the movement instruction to the console

# Assumptions

1. Vehicles will only face 1 of 4 different directions:
   **North**, **East**, **South**, and **West**.

2. We will use cartesian coordinates $(x, y)$ to represent any location on a plateau. In particular, $x$ and $y$ are both integer values, so the plateau can be separated into "grids" on a 2D plane.

3. Vehicles only have 3 different movements: turn left, turn right, and move forward. Turning left or right will rotate the vehicle by 90 degrees counter-clockwise or clockwise, and moving forward will move the vehicle in the direction its facing by "1 unit".

4. Vehicles will only move one at a time. This means when one vehicle is moving, we can assume all other vehicles are not moving.

5. No vehicle can "overcome" obstacles, or "escape" the plateau boundary. In reality, something like a helicopter would not be restricted by obstacles or plateau boundary, but in this application we will assume that all vehicles are restricted by obstacles and boundaries.

6. Vehicles will not crash into another object (obstacles or other vehicles) or fall off the plateau boundary. Instead they will stop moving immediately before crashing into something.

7. Vehicles all take up the just one grid. This application assumes that we won't have long vehicles that take up multiple grids on the plateau.

# Approaches

Before I describe my approach and design decisions, let me show the full UML diagram so we can see the big picture before I dive into the individual parts:

![UML Diagram](https://raw.githubusercontent.com/iamfranco/MarsRoverChallenge/main/diagrams/Mars%20Rover%20Challenge.png)

## Design decision on `Direction`, and `Coordinates`

We assumed that vehicles will only face 4 directions. Since direction can only be 1 of 4 possible values, we can use an `enum` to store the `Direction`:

```c#
public enum Direction
{
    North,
    East,
    South,
    West
}
```

In a similar way, we use `enum` to store `SingularInstruction`, which we assumed it can only be "turn left", "turn right", or "move forward":

```c#
public enum SingularInstruction
{
    TurnLeft,
    TurnRight,
    MoveForward
}
```

We assumed all locations on the plateau can be represented by integer cartesian coordinates $(x, y)$, so we can use a `struct` to store `Coordinates`:

```c#
public struct Coordinates
{
    public int X { get; }
    public int Y { get; }

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Coordinates operator +(Coordinates a, Coordinates b) =>
        new(a.X + b.X, a.Y + b.Y);

    public override string ToString() => $"({X}, {Y})";
}
```

where we overload the `+` operator so that adding 2 Coordinates will return its "vector sum":

$$
\begin{align}
    \begin{bmatrix}
        x_1 \\
        y_1
    \end{bmatrix}
    +
    \begin{bmatrix}
        x_2 \\
        y_2
    \end{bmatrix}
    =
    \begin{bmatrix}
        x_1 + x_2 \\
        y_1 + y_2
    \end{bmatrix}
\end{align}
$$

The "vector sum" is important because we can represent a direction's "movement vector" as `Coordinates`:

```c#
public static Coordinates GetMovementVector(this Direction direction)
{
    return direction switch
    {
        Direction.North => new(0, 1),
        Direction.South => new(0, -1),
        Direction.East => new(1, 0),
        Direction.West => new(-1, 0),
        _ => new(0, 0)
    };
}
```

so that if a vehicle is currently at coordinates $(3, 4)$ and facing **north** and is about to "move forward", then its next coordinates after moving forward would be:
$$\texttt{next coordinates} = (3, 4) + \texttt{north} = (3, 4) + (0, 1) = (3, 5)$$
So the c# code for "moving forward" can be written as:

```c#
nextCoordinates = currentCoordinates + currentDirection.GetMovementVector();
```

Doing this whole "overload the `+` operator for `Coordinates` and have an extension method to turn `Direction` into `Coordinates`" thing saves me from writing switch statements on the part of code that applies the "moving forward" action, which would be in a completely different file from the `Coordinates` file.

Although the extension method `GetMovementVector()` also uses switch statements, it is at least in the same file as the `Direction` enum, so if later on we decided to change the way we define `Direction`, we can do the appropriate adjustment to the `GetMovementVector` in the same file.

## Design Decision on `PlateauBase`, `VehicleBase`, `IInstructionReader`, `IPositionStringConverter`, and `IMapPrinter`

From the UML, we can see that `PlateauBase` and `VehicleBase` are abstract classes, and `IInstructionReader`, `IPositionStringConverter` and `IMapPrinter` are interfaces.

In particular, other classes can only interact with their concrete subclasses through them. For example, the `AppController` class doesn't interact directly with the `Rover` class, instead `AppController` interacts with `VehicleBase` which is the abstract base class of `Rover`.

This is to make **plateau**, **vehicle**, **instructionReader**, **positionStringConverter** and **mapPrinter** easily "extendable" by users. For example, if a user wants to model a "Tesla" as a vehicle in this application, then they would just need to:

1. create a class for `Tesla` that inherits from the `VehicleBase` class
2. add a new line in `Program.cs` where it defines the `vehicleMakers`:

```c#
Dictionary<string, Func<Position, VehicleBase>> vehicleMakers = new()
{
    { "Rover", position => new Rover(position) },
    { "Tesla", position => new Tesla(position) } // <-- new line added
};
```

3. then the application will "know" about the `Tesla` class, so when it comes to a stage where the application is about to create a new vehicle, it'll include `Tesla` in its "list of vehicle types".

To extend for **plateau**, **instructionReader**, **positionStringConverter** or **mapPrinter**, the user will do something similar (create a new class that inherits from the base class, then add a new line in the "dependency injection container"). The procedure for them will be fully explained in the [Open for extension but closed for modification](#open-for-extension-but-closed-for-modification) section.

## `AppController` and `AppUIHandler`

`AppController` controls the interaction between `PlateauBase` and `VehicleBase` "at a high level". This means the `AppController` will NOT micro-manage the mechanics of **plateau** and **vehicle**. Instead, it'll just "tell" **plateau** and **vehicle** to "do their thing".

For example, the `SendMoveInstruction(string instruction)` method in `AppController` does the following things:

1. checks if it is "appropriate" to send `instruction` to vehicle (are we even connected to a vehicle?)
2. uses `IInstructionReader` to evaluate the `instruction` string into a `List<SingularInstruction>`
3. send that `List<SingularInstruction>` to the connected `vehicle`, where the `vehicle` will do its thing and apply those movements
4. the `vehicle` will return a **status** of whether it reached its destination, or it had to stop prematurely because it detected danger ahead
5. return that `vehicle` **status** to the caller, which is `AppUIHandler`, so that `AppUIHandler` can print to the console something like "vehicle applied the instruction and has reached [`4 5 W`]"

So the actual "heavy lifting" of "evaluate instruction" and "make the vehicle move" were delegated to `IInstructionReader` and `VehicleBase`. All `AppController` did was to be the middle man that exposes methods (such as `SendMoveInstruction` ) to `AppUIHandler`, and direct the appropriate components to do the right thing.

Now for `AppUIHandler`, it has methods that:

1. prompts the user for input,
2. then delegates to `AppController` to actually perform the user requests,
3. then displays the **result** back to the user (where the **result** will be formatted using `IPositionStringConvert`)

## "Mocking user input" in tests

At the later stages, I wanted to expand the tests to cover even methods that requires user inputs (for example, methods that uses `Console.ReadLine()` or `Console.ReadKey()`).

To achieve that, I decided to create 2 classes in the "MarsRover" project called `InputReader` and `InputReaderContainer`, and create a class in "MarsRover.Tests" project called `InputReaderForTest`, as shown in UML diagram below:
![InputReader](/diagrams/InputReader/InputReader.png)

So that in the main **MarsRover** project, I can replace the old code that gets the user input:

```c#
Console.Write("Enter Radius: ");
string radius = Console.ReadLine();
```

with the new code that does the same thing:

```c#
string radius = InputReaderContainer.GetUserInput("Enter Radius: ");
```

with the benefit that `InputReader` is substitutable in the unit test.

So in the unit testing **MarsRover.Tests** project, I can simulate the user inputting `10` for the radius by doing:

```c#
List<string> userInputs = new() { "10" };
InputReaderForTest inputReader = new(userInputs);
InputReaderContainer.SetInputReader(inputReader);
```

so that when the unit test executes the main code that involves user inputs, it will call the `InputReaderContainer` methods and will return `10` to the caller:

```c#
string radius = InputReaderContainer.GetUserInput("Enter Radius: "); // sets radius as 10
```

# Open for extension but closed for modification

The user can extend the application in the following ways:

1. Make a new plateau of a different shape (default shape is **rectangular**)
2. Make a new vehicle (default vehicle is **rover**)
3. Make a new instruction reader (default instruction reader only undertands string that consist of "L", "R", and "M")
4. Make a new position string converter (default position string converter will format position as `1 2 N`, and coordinates as `1 2`)
5. Make a new map printer

## New Plateau Shape

Let's say the user wants to make a **circular** plateau, then they'll just need to:

1. create a class for `CircularPlateau` that inherits from the `PlateauBase` class
2. add a new "key value pair" in `Program.cs` where it defines the `plateauMakers`:

```c#
Dictionary<string, Func<PlateauBase>> plateauMakers = new()
{
    {
        "Rectangular Plateau", () =>
        {
            // ...
            return new RectangularPlateau(maximumCoordinates);
        }
    },

    // new "key value pair" added
    {
        "Circular Plateau", () =>
        {
            string radiusString = AppUIHelpers.AskUntilValidStringInput(
                $"Enter Radius (eg \"5\"): ",
                s => int.TryParse(s, out _));

            return new CircularPlateau(int.Parse(radiusString));
        }
    },
};
```

Notice in the example above, we used the `AppUIHelpers.AskUntilValidStringInput` method to prompt the user to enter the **radius** of the circle.

Now that the user added a new class for `CircularPlateau` and added a new item for it in the `Program.cs`'s `plateauMakers`, the application will now know about the `CircularPlateau` class.

So that when the user runs the application, the application will include `CircularPlateau` as an "available plateau type", so the console app will show this:

```
All available plateau types:
  1 - Rectangular Plateau
  2 - Circular Plateau

Enter a number to select a plateau (number between 1 and 2):
```

If the user enters `2`, which selects the circular plateau, the console app will show:

```
Enter Radius (eg "5"):
```

which is the prompt the user specified in the new "key value pair" in `plateauMakers`.

## New Vehicle Type

Let's say the user wants to make a **Tesla** as vehicle, then they'll just need to:

1. create a class for `Tesla` that inherits from the `VehicleBase` class
2. add a new line in `Program.cs` where it defines the `vehicleMakers`:

```c#
Dictionary<string, Func<Position, VehicleBase>> vehicleMakers = new()
{
    { "Rover", position => new Rover(position) },
    { "Tesla", position => new Tesla(position) } // <-- new line added
};
```

Now that the user added a new class for `Tesla` and added a new item for it in the `Program.cs`'s `vehicleMakers`, the application will now know about the `Tesla` class.

So that if the user runs the application, then when it comes to the stage where a new vehicle needs to be created, the console app will show this:

```
All available vehicle types:
  1 - Rover
  2 - Tesla

Enter a number to select a vehicle (number between 1 and 2):
```

And the user can select the `Tesla` by entering `2`.

## New Instruction Reader

Let's say the user is tired of typing `LMMMMMMMMMMR` just to make the vehicle "turn left, then move forward **10** steps, then turn right".

Instead, they want to just type `L M10 R` and want the application understand this "shortened instruction" as the same as above.

To achieve this, they'll just need to:

1. create a class for `StepCountInstructionReader` that implements the `IInstructionReader` interface. In particular, they'll code the body of the `EvaluateInstruction` method so that it will evaluate `L M10 R` correctly.
2. modify a line `Program.cs` where it defines the `instructionReader`:

```c#
// IInstructionReader instructionReader = new StandardInstructionReader(); // old
IInstructionReader instructionReader = new StepCountInstructionReader(); // new
```

Now the application will understand movement instructions that look like `L M10 R`, so that when the console app comes to the stage where it needs movement instruction, the console will show this:

```
Enter Movement Instruction (eg "L M3 R M2 L2"):
```

And the user can enter `L M10 R` and the application will understand it.

## New position string converter

Let's say the user doesn't like the standard position string formatting of `1 2 N`, and prefers it to be `1 2 North`.

To achieve that, they'll just need to:

1. Create a new class, say `FullWordPositionStringConverter`, that implements the `IPositionStringConverter` interface. Implement the method bodies appropriately to achieve the `1 2 North` string format.
2. modify a line `Program.cs` where it defines the `positionStringConverter`:

```c#
// IPositionStringConverter positionStringConverter = new StandardPositionStringConverter(); // old
IPositionStringConverter positionStringConverter = new FullWordPositionStringConverter(); // new
```

Now the application will read position and coordinates strings using the new format.

For example, the console app would now show the connected rover position like this:

```
Connected to [Rover] at [1 2 North]
```

## New Map Printer

Let's say the user doesn't like the look and feel of the default map printer, and they want to their map printer to use a different sized grids with different white spaces and perhaps different font or background color.

To achieve that, they'll just need to:

1. Create a new class, say `NewMapPrinter`, that implements the `IMapPrinter` interface. Implement the `PrintMap` method to achieve their desired look and feel.
2. modify a line `Program.cs` where it defines the `mapPrinter`:

```c#
// IMapPrinter mapPrinter = new MapPrinter(); // old
IMapPrinter mapPrinter = new NewMapPrinter(); // new
```

Now the application will use the `NewMapPrinter`'s `PrintMap` method to print map on the console.

# Future thoughts

The 4 possible directions might be too restrictive for our vehicles. It might be good to use Direction that can be more precise, such as "North West", "South East" etc. Or even to use angular direction.

We assumed that all vehicles treat obstacles the same, such that no vehicle can "overcome" an obstacle. But as said in the assumptions section, helicopter is a vehicle and yet it probably wouldn't see big rocks on the ground as obstacles. So it might be a good idea to differentiate different types of obstacles and let the vehicle class individually specify whether a particular type of "obstacle" is actually an obstacle for that vehicle class.

We assumed that all vehicles would perform an emergency stop immediately before it's about to crash. This assumption is to simplify the application so we don't have to deal with the uncertainty of what happens when a vehicle crashes, because the outcome of a crash is arguably non-deterministic. But in reality, not every vehicle has a "danger detection mechanism".

For TDD, it is challenging to unit test the `AppUIHandler` class since it prints the result to the console with the color grids. It would be interesting to learn ways to test methods that only prints stuff to console.
