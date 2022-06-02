
using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

IPositionStringConverter positionStringConverter = new PositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
CommandHandler commandHandler = new(instructionReader, positionStringConverter);
RectangularPlateau plateau;

plateau = AskUserToMakePlateau(positionStringConverter, commandHandler);

while (true)
{
    try
    {
        ClearScreenAndPrintMap();
        string obstacleCoordinates = Ask($"Enter Obstacle Coordinate " +
            $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
            (s) => IsEmptyNullOrValidString(s, positionStringConverter.IsValidCoordinateString));

        if (string.IsNullOrEmpty(obstacleCoordinates))
            break;

        plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

while (true)
{
    commandHandler.ResetRecentPath();
    ClearScreenAndPrintMap();

    string vehicleInitialPositionOrCoordinates = Ask($"Enter Position (eg \"{positionStringConverter.ExamplePositionString}\") to add new Vehicle, or " +
        $"\nEnter Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\") to connect with existing vehicle: ", IsValidVehicleInitialPositionOrCoordinates);
    if (IsValidVehicleInitialCoordinates(vehicleInitialPositionOrCoordinates))
    {
        Coordinates initialCoordinates = positionStringConverter.ToCoordinates(vehicleInitialPositionOrCoordinates);
        commandHandler.ConnectToVehicleAtCoordinates(initialCoordinates);
    }
    else
    {
        Position initialPosition = positionStringConverter.ToPosition(vehicleInitialPositionOrCoordinates);
        commandHandler.AddVehicleToPlateau(new Rover(initialPosition));
    }
    Console.WriteLine($"Connected with {commandHandler.GetVehicle()!.GetType()} at Position [{commandHandler.GetPositionString()}]");

    ClearScreenAndPrintMap();

    string instructionString = Ask($"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ", instructionReader.IsValidInstruction);
    (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

    ClearScreenAndPrintMap();
    Console.WriteLine(message);

    Console.WriteLine();
    Console.Write("Press any key to continue.. ");
    Console.ReadKey();
}

static string Ask(string prompt, Func<string, bool> validationFunc)
{
    while (true)
    {
        try
        {
            Console.WriteLine();
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (input is null)
                continue;

            if (!validationFunc(input))
                continue;

            return input;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

static bool IsEmptyNullOrValidString(string input, Func<string, bool> validationFunc)
{
    if (string.IsNullOrEmpty(input))
        return true;

    return validationFunc(input);
}

bool IsValidVehicleInitialPositionOrCoordinates(string inputString)
{
    if (!positionStringConverter.IsValidPositionString(inputString))
    {
        return IsValidVehicleInitialCoordinates(inputString);
    }

    Position position = positionStringConverter.ToPosition(inputString);
    if (!plateau.IsCoordinateValidInPlateau(position.Coordinates))
    {
        Console.WriteLine($"Cannot add new vehicle at [{inputString}] on plateau");
        return false;
    }

    return true;
}

bool IsValidVehicleInitialCoordinates(string inputString)
{
    if (!positionStringConverter.IsValidCoordinateString(inputString))
    {
        Console.WriteLine($"[{inputString}] is neither Position nor Coordinates");
        return false;
    }

    Coordinates coordinates = positionStringConverter.ToCoordinates(inputString);
    if (plateau.VehiclesContainer.GetVehicleAtCoordinates(coordinates) is null)
    {
        Console.WriteLine($"No Vehicle at coordinates [{inputString}]");
        return false;
    }

    return true;
}

void ClearScreenAndPrintMap()
{
    Console.Clear();
    Console.WriteLine("ctrl-C to exit");

    try
    {
        plateau.PrintMap(commandHandler.RecentPath);
    }
    catch
    { }

    Console.WriteLine();
}

static RectangularPlateau AskUserToMakePlateau(IPositionStringConverter positionStringConverter, CommandHandler commandHandler)
{
    RectangularPlateau plateau;
    while (true)
    {
        try
        {
            string maximumCoordinatesString = Ask($"Enter Maximum Coordinates " +
                $"(eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                positionStringConverter.IsValidCoordinateString);

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
            plateau = new(maximumCoordinates);
            commandHandler.ConnectPlateau(plateau);
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    return plateau;
}