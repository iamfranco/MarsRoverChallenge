
//using MarsRover.AppUI;
//using MarsRover.Models.Instructions;
//using MarsRover.Models.Plateaus;
//using MarsRover.Models.Positions;
//using MarsRover.Models.Vehicles;
//using System.Text.RegularExpressions;

//IPositionStringConverter positionStringConverter = new PositionStringConverter();
//IInstructionReader instructionReader = new StandardInstructionReader();
//Rover vehicle;
//CommandHandler commandHandler = new(instructionReader, positionStringConverter);

//string plateauSizeString = Ask("Enter Plateau Size (eg \"5 5\"): ", new Regex(@"^\d+ \d+$").IsMatch);
//Coordinates plateauSize = positionStringConverter.ToCoordinates(plateauSizeString);
//RectangularPlateau plateau = new(plateauSize);

//while (true)
//{
//    ClearScreenAndPrintMap(plateau, new());
//    string obstacleCoordinates = Ask("Enter Obstacle Coordinate (eg \"5 5\", or empty if no more obstacle): ", new Regex(@"^(\d+ \d+)?$").IsMatch);
//    if (string.IsNullOrEmpty(obstacleCoordinates))
//        break;

//    plateau.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
//}

//while (true)
//{
//    ClearScreenAndPrintMap(plateau, new());

//    string vehicleInitialPosition = Ask("Enter Vehicle Initial Position (eg \"1 2 N\"): ", IsValidVehicleInitialPosition);
//    Position initialPosition = positionStringConverter.ToPosition(vehicleInitialPosition);
//    vehicle = new(initialPosition, plateau);
//    commandHandler.ConnectVehicle(vehicle);

//    ClearScreenAndPrintMap(plateau, new() { initialPosition });

//    string instructionString = Ask("Enter Movement Instruction (eg \"LMMMLRRL\"): ", instructionReader.IsValidInstruction);
//    (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

//    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
//    if (status)
//    {
//        Console.WriteLine($"Instruction [{instructionString}] lead to Position: {commandHandler.RequestPosition()}");
//    }
//    else
//    {
//        Console.WriteLine($"DANGEROUS INSTRUCTION [{instructionString}]: {message}");
//    }
//    Console.WriteLine();
//    Console.Write("Press any key to continue.. ");
//    Console.ReadKey();
//}

//static string Ask(string prompt, Func<string, bool> validationFunc)
//{
//    while (true)
//    {
//        try
//        {
//            Console.Write(prompt);
//            string? input = Console.ReadLine();

//            if (input is null)
//                continue;

//            if (!validationFunc(input))
//                continue;

//            return input;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message);
//            continue;
//        }
//    }
//}

//bool IsValidVehicleInitialPosition(string positionString)
//{
//    if (!positionStringConverter.IsValidPositionString(positionString))
//        return false;

//    Position position = positionStringConverter.ToPosition(positionString);
//    return plateau.IsCoordinateValidInPlateau(position.Coordinates);
//}

//void ClearScreenAndPrintMap(PlateauBase plateau, List<Position> recentPath)
//{
//    Console.Clear();
//    Console.WriteLine("ctrl-C to exit");

//    try
//    {
//        plateau.PrintMap(recentPath);
//    }
//    catch
//    { }

//    Console.WriteLine();
//}
Console.WriteLine("hello");