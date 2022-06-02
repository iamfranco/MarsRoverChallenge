using MarsRover.Models.Plateaus.Containers;
using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Models.Plateaus;

public abstract class PlateauBase
{
    public ObstaclesContainer ObstaclesContainer { get; }
    public VehiclesContainer VehiclesContainer { get; }

    public PlateauBase()
    {
        ObstaclesContainer = new ObstaclesContainer(IsCoordinateValidInPlateau);
        VehiclesContainer = new VehiclesContainer(IsCoordinateValidInPlateau);
    }

    public bool IsCoordinateValidInPlateau(Coordinates coordinates)
    {
        if (!IsCoordinateWithinPlateauBoundary(coordinates))
            return false;

        if (ObstaclesContainer.ObstacleCoordinates.Contains(coordinates))
            return false;

        if (VehiclesContainer.Vehicles.Select(vehicle => vehicle.Position.Coordinates).Contains(coordinates))
            return false;

        return true;
    }
    public abstract void PrintMap(List<Position> recentPath);
    protected abstract bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates);

    protected void PrintMap(List<Position> recentPath, Coordinates maxCoordinates)
    {
        int maxX = maxCoordinates.X;
        int maxY = maxCoordinates.Y;
        var obstacles = ObstaclesContainer.ObstacleCoordinates;
        var vehicles = VehiclesContainer.Vehicles;

        if (maxX > 40 || maxY > 40)
        {
            Console.WriteLine($"{GetType().Name}: width [{maxX}] and height [{maxY}]");
            if (obstacles.Count > 0)
            {
                Console.WriteLine($"\nObstacles (count = {obstacles.Count}) at Coordinates:");
                foreach (var obstacle in obstacles)
                {
                    Console.WriteLine($"   {obstacle}");
                }
            }

            if (vehicles.Count > 0)
            {
                Console.WriteLine($"\nVehicles (count = {vehicles.Count}): ");
                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine($"   [{vehicle.GetType().Name}] at {vehicle.Position.Coordinates}");
                }
            }

            return;
        }

        ConsoleColor defaultBGColor = Console.BackgroundColor;

        (string symbol, ConsoleColor bgColor)[,] matrixToPrint = GetMatrixToPrint(recentPath, maxCoordinates,
            defaultGroundColor: defaultBGColor,
            validGroundColor: ConsoleColor.Blue,
            visitedGroundColor: ConsoleColor.DarkBlue,
            lastVisitedGroundColor: ConsoleColor.Red,
            invalidGroundColor: ConsoleColor.DarkGray,
            availableVehicleColor: ConsoleColor.DarkYellow);

        Console.WriteLine($"  Y");
        for (int y = maxY; y >= 0; y--)
        {
            Console.Write($"{y,3} ");
            for (int x = 0; x <= maxX; x++)
            {
                (string symbol, ConsoleColor bgColor) = matrixToPrint[x, y];

                Console.BackgroundColor = bgColor;
                Console.Write($" {symbol} ");
                Console.BackgroundColor = defaultBGColor;
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        Console.Write("   ");
        for (int x = 0; x <= maxX; x++)
        {
            Console.Write($"{x,3} ");
        }
        Console.WriteLine("  X");
    }

    private (string symbol, ConsoleColor bgColor)[,] GetMatrixToPrint(
        List<Position> recentPath,
        Coordinates maxCoordinates,
        ConsoleColor defaultGroundColor,
        ConsoleColor validGroundColor,
        ConsoleColor visitedGroundColor,
        ConsoleColor lastVisitedGroundColor,
        ConsoleColor invalidGroundColor,
        ConsoleColor availableVehicleColor)
    {
        (string symbol, ConsoleColor bgColor)[,] matrixToPrint = 
            new (string, ConsoleColor)[maxCoordinates.X + 1, maxCoordinates.Y + 1];

        for (int i = 0; i < matrixToPrint.GetLength(0); i++)
        {
            for (int j = 0; j < matrixToPrint.GetLength(1); j++)
            {
                if (IsCoordinateWithinPlateauBoundary(new(i, j)))
                    matrixToPrint[i, j] = (" ", validGroundColor);
                else
                    matrixToPrint[i, j] = (" ", defaultGroundColor);
            }
        }

        foreach (var obstacleCoordinate in ObstaclesContainer.ObstacleCoordinates)
        {
            matrixToPrint[obstacleCoordinate.X, obstacleCoordinate.Y] = ("X", invalidGroundColor);
        }

        foreach (var vehicle in VehiclesContainer.Vehicles)
        {
            PopulateMatrixToPrint(matrixToPrint, vehicle.Position, availableVehicleColor);
        }

        foreach (var recentPathItem in recentPath)
        {
            PopulateMatrixToPrint(matrixToPrint, recentPathItem, visitedGroundColor);
        }

        if (recentPath.Count > 0)
        {
            int lastX = recentPath.Last().Coordinates.X;
            int lastY = recentPath.Last().Coordinates.Y;
            matrixToPrint[lastX, lastY].bgColor = lastVisitedGroundColor;
        }

        return matrixToPrint;
    }

    private static void PopulateMatrixToPrint((string symbol, ConsoleColor bgColor)[,] matrixToPrint, Position position, ConsoleColor color)
    {
        int x = position.Coordinates.X;
        int y = position.Coordinates.Y;
        string symbol = position.Direction switch
        {
            Direction.North => "\u2191",
            Direction.East => ">",
            Direction.South => "\u2193",
            Direction.West => "<",
            _ => " "
        };
        matrixToPrint[x, y] = (symbol, color);
    }
}
