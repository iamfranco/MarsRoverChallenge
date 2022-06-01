
using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;
using System.Text.RegularExpressions;

IPositionStringConverter positionStringConverter = new PositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
CommandHandler commandHandler = new(instructionReader, positionStringConverter);

string maximumCoordinatesString = Ask($"Enter Maximum Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\"): ", new Regex(@"^\d+ \d+$").IsMatch);
Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
RectangularPlateau plateau = new(maximumCoordinates);
commandHandler.ConnectPlateau(plateau);

while (true)
{
    ClearScreenAndPrintMap(plateau, new());
    string obstacleCoordinates = Ask($"Enter Obstacle Coordinate (eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ", new Regex(@"^(\d+ \d+)?$").IsMatch);
    if (string.IsNullOrEmpty(obstacleCoordinates))
        break;

    plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
}

while (true)
{
    commandHandler.ResetRecentPath();
    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);

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

    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);

    string instructionString = Ask($"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ", instructionReader.IsValidInstruction);
    (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
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
            continue;
        }
    }
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

void ClearScreenAndPrintMap(PlateauBase plateau, List<Position> recentPath)
{
    Console.Clear();
    Console.WriteLine("ctrl-C to exit");

    try
    {
        plateau.PrintMap(recentPath);
    }
    catch
    { }

    Console.WriteLine();
}
