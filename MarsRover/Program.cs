
using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

IPositionStringConverter positionStringConverter = new PositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
Rover vehicle;
CommandHandler commandHandler = new(instructionReader, positionStringConverter);

string plateauSizeString = Ask("Enter Plateau Size (eg \"5 5\"): ", new Regex(@"^\d+ \d+$").IsMatch);
Coordinates plateauSize = positionStringConverter.ToCoordinates(plateauSizeString);
RectangularPlateau plateau = new(plateauSize);

while (true)
{
    PrintRectangle(plateauSize.X, plateauSize.Y, new(), plateau.ObstacleCoordinates);
    string obstacleCoordinates = Ask("Enter Obstacle Coordinate (eg \"5 5\", or empty if no more obstacle): ", new Regex(@"^(\d+ \d+)?$").IsMatch);
    if (string.IsNullOrEmpty(obstacleCoordinates))
        break;

    plateau.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
}


while (true)
{
    PrintRectangle(plateauSize.X, plateauSize.Y, new(), plateau.ObstacleCoordinates);

    string vehicleInitialPosition = Ask("Enter Vehicle Initial Position (eg \"1 2 N\"): ", IsValidVehicleInitialPosition);
    (Coordinates initialCoordinates, Direction initialDirection) = positionStringConverter.ToCoordinatesDirection(vehicleInitialPosition);
    vehicle = new(initialCoordinates, initialDirection, plateau);
    commandHandler.ConnectVehicle(vehicle);

    PrintRectangle(plateauSize.X, plateauSize.Y, new() { (initialCoordinates, initialDirection) }, plateau.ObstacleCoordinates);

    string instructionString = Ask("Enter Movement Instruction (eg \"LMMMLRRL\"): ", instructionReader.IsValidInstruction);
    (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

    PrintRectangle(plateauSize.X, plateauSize.Y, commandHandler.RecentPath, plateau.ObstacleCoordinates);
    if (status)
    {
        Console.WriteLine($"Instruction [{instructionString}] lead to Position: {commandHandler.RequestPosition()}");
    }
    else
    {
        Console.WriteLine($"DANGEROUS INSTRUCTION [{instructionString}]: {message}");
    }
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

bool IsValidVehicleInitialPosition(string position)
{
    if (!positionStringConverter.IsValidPositionString(position))
        return false;

    (Coordinates coordinates, Direction direction) = positionStringConverter.ToCoordinatesDirection(position);
    return plateau.IsCoordinateValidInPlateau(coordinates);
}

void PrintRectangle(int width, int height, 
    List<(Coordinates coordinates, Direction direction)> recentPath,
    ReadOnlyCollection<Coordinates> obstacleCoordinates)
{
    ConsoleColor defaultBGColor = Console.BackgroundColor;
    ConsoleColor validGroundColor = ConsoleColor.Blue;
    ConsoleColor visitedGroundColor = ConsoleColor.DarkBlue;
    ConsoleColor lastVisitedGroundColor = ConsoleColor.Red;
    ConsoleColor invalidGroundColor = ConsoleColor.DarkGray;

    string?[,] symbols = new string?[width+1, height+1];
    foreach (var recentPathItem in recentPath)
    {
        int x = recentPathItem.coordinates.X;
        int y = recentPathItem.coordinates.Y;
        symbols[x, y] = recentPathItem.direction.Name switch
        {
            "north" => "\u2191",
            "east" => ">",
            "south" => "\u2193",
            "west" => "<",
            _ => null
        };
    }
    int lastX = -1;
    int lastY = -1;
    if (recentPath.Count > 0)
    {
        lastX = recentPath.Last().coordinates.X;
        lastY = recentPath.Last().coordinates.Y;
    }

    Console.Clear();
    Console.WriteLine("ctrl-C to quit");
    Console.WriteLine();
    Console.WriteLine($"  Y");
    for (int y = height; y >= 0; y--)
    {
        Console.Write($"{y, 3} ");
        for (int x = 0; x <= width; x++)
        {
            Coordinates currentCoordinates = new(x, y);

            Console.BackgroundColor = visitedGroundColor;
            string? symbol = symbols[x, y];

            if (symbol is null)
            {
                symbol = " ";
                Console.BackgroundColor = validGroundColor;
            }

            if (x == lastX && y == lastY)
                Console.BackgroundColor = lastVisitedGroundColor;

            if (obstacleCoordinates.Contains(currentCoordinates))
            {
                symbol = "X";
                Console.BackgroundColor = invalidGroundColor;
            }

            Console.Write($" {symbol} ");
            Console.BackgroundColor = defaultBGColor;
            Console.Write(" ");
        }
        Console.WriteLine();
    }
    Console.Write("   ");
    for (int x = 0; x <= width; x++)
    {
        Console.Write($"{x, 3} ");
    }
    Console.WriteLine("  X");
    Console.WriteLine();
}