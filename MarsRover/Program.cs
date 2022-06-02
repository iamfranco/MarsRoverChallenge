
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

AskUserToMakeObstacles(positionStringConverter, commandHandler, plateau);

while (true)
{
    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);

    AskUserToCreateNewVehicleOrConnectToExistingVehicle(positionStringConverter, commandHandler, plateau);

    AskUserForMovementInstructionAndSendToVehicle(instructionReader, commandHandler, plateau);

    Console.WriteLine();
    Console.Write("Press any key to continue.. ");
    Console.ReadKey();

    commandHandler.ResetRecentPath();
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
                throw new Exception($"Input cannot be null");

            if (!validationFunc(input))
                throw new Exception($"Input [{input}] is not in correct format");

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

static void ClearScreenAndPrintMap(PlateauBase plateau, List<Position> recentPath)
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

            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
            return plateau;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

static void AskUserToMakeObstacles(IPositionStringConverter positionStringConverter, CommandHandler commandHandler, RectangularPlateau plateau)
{
    while (true)
    {
        try
        {
            string obstacleCoordinates = Ask($"Enter Obstacle Coordinate " +
                $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
                (s) => IsEmptyNullOrValidString(s, positionStringConverter.IsValidCoordinateString));

            if (string.IsNullOrEmpty(obstacleCoordinates))
                break;

            plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

static void AskUserForMovementInstructionAndSendToVehicle(IInstructionReader instructionReader, CommandHandler commandHandler, RectangularPlateau plateau)
{
    string instructionString = Ask($"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ", 
        instructionReader.IsValidInstruction);

    (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
    Console.WriteLine(message);
}

static void AskUserToCreateNewVehicleOrConnectToExistingVehicle(IPositionStringConverter positionStringConverter, CommandHandler commandHandler, RectangularPlateau plateau)
{
    bool isConnectedToVehicle;
    string message;
    while (true)
    {
        string positionOrCoordinatesString = AskForPositionOrCoordinateasString(positionStringConverter);

        if (positionStringConverter.IsValidPositionString(positionOrCoordinatesString))
        {
            // create new vehicle at position 
            Position initialPosition = positionStringConverter.ToPosition(positionOrCoordinatesString);
            VehicleBase vehicle = new Rover(initialPosition);
            (isConnectedToVehicle, message) = commandHandler.AddVehicleToPlateau(vehicle);
        }
        else
        {
            // connect to existing vehicle at coordinates
            Coordinates initialCoordinates = positionStringConverter.ToCoordinates(positionOrCoordinatesString);
            (isConnectedToVehicle, message) = commandHandler.ConnectToVehicleAtCoordinates(initialCoordinates);
        }

        if (!isConnectedToVehicle)
        {
            Console.WriteLine(message);
            continue;
        }

        break;
    }
    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
    Console.WriteLine(message);

    static string AskForPositionOrCoordinateasString(IPositionStringConverter positionStringConverter)
    {
        return Ask(
                    $"Enter Position (eg \"{positionStringConverter.ExamplePositionString}\") to add new Vehicle, or " +
                    $"\nEnter Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\") to connect with existing vehicle: ",
                    (input) => positionStringConverter.IsValidPositionString(input) || positionStringConverter.IsValidCoordinateString(input));
    }
}