using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI.Components;
public class MapPrinter
{
    private readonly ConsoleColor _defaultGroundColor = Console.BackgroundColor;
    private readonly ConsoleColor _validGroundColor = ConsoleColor.Blue;
    private readonly ConsoleColor _visitedGroundColor = ConsoleColor.DarkBlue;
    private readonly ConsoleColor _lastVisitedGroundColor = ConsoleColor.Red;
    private readonly ConsoleColor _invalidGroundColor = ConsoleColor.DarkGray;
    private readonly ConsoleColor _availableVehicleColor = ConsoleColor.DarkYellow;

    public void PrintMap(AppController appController)
    {
        if (appController is null)
            throw new ArgumentNullException(nameof(appController));

        PlateauBase? plateau = appController.Plateau;
        if (plateau is null)
            throw new Exception("Plateau not connected");

        List<Position> recentPath = appController.RecentPath;

        int maxX = plateau.MaximumCoordinates.X;
        int maxY = plateau.MaximumCoordinates.Y;
        int minX = plateau.MinimumCoordinates.X;
        int minY = plateau.MinimumCoordinates.Y;

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        List<Coordinates> obstacles = plateau.ObstaclesContainer.ObstacleCoordinates.ToList();
        List<VehicleBase> vehicles = plateau.VehiclesContainer.Vehicles.ToList();

        if (width > 40 || height > 40)
        {
            PrintTextDescriptionOfMap(plateau, obstacles, vehicles);
            return;
        }

        (string symbol, ConsoleColor bgColor)[,] matrixToPrint = GetMatrixToPrint(recentPath, plateau);

        Console.WriteLine($"  Y");
        for (int y = height - 1; y >= 0; y--)
        {
            Console.Write($"{y + minY,3} ");
            for (int x = 0; x < width; x++)
            {
                (string symbol, ConsoleColor bgColor) = matrixToPrint[x, y];

                Console.BackgroundColor = bgColor;
                Console.Write($" {symbol} ");
                Console.BackgroundColor = _defaultGroundColor;
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        Console.Write("   ");
        for (int x = minX; x <= maxX; x++)
        {
            Console.Write($"{x,3} ");
        }
        Console.WriteLine("  X");
    }

    private void PrintTextDescriptionOfMap(PlateauBase plateau, List<Coordinates> obstacles, List<VehicleBase> vehicles)
    {
        Console.WriteLine($"{GetType().Name}: between {plateau.MinimumCoordinates} and {plateau.MaximumCoordinates}");
        if (obstacles.Count > 0)
        {
            Console.WriteLine($"\nObstacles (count = {obstacles.Count}) at Coordinates:");
            foreach (Coordinates obstacle in obstacles)
            {
                Console.WriteLine($"   {obstacle}");
            }
        }

        if (vehicles.Count > 0)
        {
            Console.WriteLine($"\nVehicles (count = {vehicles.Count}): ");
            foreach (VehicleBase vehicle in vehicles)
            {
                Console.WriteLine($"   [{vehicle.GetType().Name}] at {vehicle.Position.Coordinates}");
            }
        }
    }

    private (string symbol, ConsoleColor bgColor)[,] GetMatrixToPrint(
        List<Position> recentPath, PlateauBase plateau)
    {
        Coordinates sizeCoordinates = plateau.MaximumCoordinates - plateau.MinimumCoordinates + new Coordinates(1, 1);
        int width = sizeCoordinates.X;
        int height = sizeCoordinates.Y;

        (string symbol, ConsoleColor bgColor)[,] matrixToPrint = new (string, ConsoleColor)[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Coordinates coordinates = new Coordinates(i, j) + plateau.MinimumCoordinates;
                if (plateau.IsCoordinateWithinPlateauBoundary(coordinates))
                    matrixToPrint[i, j] = (" ", _validGroundColor);
                else
                    matrixToPrint[i, j] = (" ", _defaultGroundColor);
            }
        }

        foreach (Coordinates obstacleCoordinate in plateau.ObstaclesContainer.ObstacleCoordinates)
        {
            Coordinates indices = obstacleCoordinate - plateau.MinimumCoordinates;
            matrixToPrint[indices.X, indices.Y] = ("X", _invalidGroundColor);
        }

        foreach (VehicleBase vehicle in plateau.VehiclesContainer.Vehicles)
        {
            PopulateMatrixToPrint(matrixToPrint, vehicle.Position, plateau.MinimumCoordinates, _availableVehicleColor);
        }

        foreach (Position recentPathItem in recentPath)
        {
            PopulateMatrixToPrint(matrixToPrint, recentPathItem, plateau.MinimumCoordinates, _visitedGroundColor);
        }

        if (recentPath.Count > 0)
        {
            Coordinates indices = recentPath.Last().Coordinates - plateau.MinimumCoordinates;
            matrixToPrint[indices.X, indices.Y].bgColor = _lastVisitedGroundColor;
        }

        return matrixToPrint;
    }

    private static void PopulateMatrixToPrint((string symbol, ConsoleColor bgColor)[,] matrixToPrint,
        Position position, Coordinates minimumCoordinates, ConsoleColor color)
    {
        Coordinates indices = position.Coordinates - minimumCoordinates;
        string symbol = position.Direction switch
        {
            Direction.North => "\u2191",
            Direction.East => ">",
            Direction.South => "\u2193",
            Direction.West => "<",
            _ => " "
        };
        matrixToPrint[indices.X, indices.Y] = (symbol, color);
    }
}
