using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions.Elementals;

namespace MarsRover.AppUI.Components;
public class MapPrinter
{
    public void PrintMap(CommandHandler commandHandler)
    {
        var plateau = commandHandler.GetPlateau();
        if (plateau is null)
            throw new Exception("Plateau not connected");

        var recentPath = commandHandler.RecentPath;

        var maxX = plateau.MaximumCoordinates.X;
        var maxY = plateau.MaximumCoordinates.Y;
        var minX = plateau.MinimumCoordinates.X;
        var minY = plateau.MinimumCoordinates.Y;

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;

        var obstacles = plateau.ObstaclesContainer.ObstacleCoordinates;
        var vehicles = plateau.VehiclesContainer.Vehicles;

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

        var defaultBGColor = Console.BackgroundColor;

        var matrixToPrint = GetMatrixToPrint(recentPath, plateau, width, height,
            defaultGroundColor: defaultBGColor,
            validGroundColor: ConsoleColor.Blue,
            visitedGroundColor: ConsoleColor.DarkBlue,
            lastVisitedGroundColor: ConsoleColor.Red,
            invalidGroundColor: ConsoleColor.DarkGray,
            availableVehicleColor: ConsoleColor.DarkYellow);

        Console.WriteLine($"  Y");
        for (var y = maxY; y >= 0; y--)
        {
            Console.Write($"{y,3} ");
            for (var x = 0; x <= maxX; x++)
            {
                (var symbol, var bgColor) = matrixToPrint[x, y];

                Console.BackgroundColor = bgColor;
                Console.Write($" {symbol} ");
                Console.BackgroundColor = defaultBGColor;
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        Console.Write("   ");
        for (var x = 0; x <= maxX; x++)
        {
            Console.Write($"{x,3} ");
        }
        Console.WriteLine("  X");
    }

    private (string symbol, ConsoleColor bgColor)[,] GetMatrixToPrint(
        List<Position> recentPath,
        PlateauBase plateau,
        int width,
        int height,
        ConsoleColor defaultGroundColor,
        ConsoleColor validGroundColor,
        ConsoleColor visitedGroundColor,
        ConsoleColor lastVisitedGroundColor,
        ConsoleColor invalidGroundColor,
        ConsoleColor availableVehicleColor)
    {
        (string symbol, ConsoleColor bgColor)[,] matrixToPrint =
            new (string, ConsoleColor)[width, height];

        for (var i = 0; i < matrixToPrint.GetLength(0); i++)
        {
            for (var j = 0; j < matrixToPrint.GetLength(1); j++)
            {
                if (plateau.IsCoordinateWithinPlateauBoundary(new(i, j)))
                    matrixToPrint[i, j] = (" ", validGroundColor);
                else
                    matrixToPrint[i, j] = (" ", defaultGroundColor);
            }
        }

        foreach (var obstacleCoordinate in plateau.ObstaclesContainer.ObstacleCoordinates)
        {
            matrixToPrint[obstacleCoordinate.X, obstacleCoordinate.Y] = ("X", invalidGroundColor);
        }

        foreach (var vehicle in plateau.VehiclesContainer.Vehicles)
        {
            PopulateMatrixToPrint(matrixToPrint, vehicle.Position, availableVehicleColor);
        }

        foreach (var recentPathItem in recentPath)
        {
            PopulateMatrixToPrint(matrixToPrint, recentPathItem, visitedGroundColor);
        }

        if (recentPath.Count > 0)
        {
            var lastX = recentPath.Last().Coordinates.X;
            var lastY = recentPath.Last().Coordinates.Y;
            matrixToPrint[lastX, lastY].bgColor = lastVisitedGroundColor;
        }

        return matrixToPrint;
    }

    private static void PopulateMatrixToPrint((string symbol, ConsoleColor bgColor)[,] matrixToPrint,
        Position position, ConsoleColor color)
    {
        var x = position.Coordinates.X;
        var y = position.Coordinates.Y;
        var symbol = position.Direction switch
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
