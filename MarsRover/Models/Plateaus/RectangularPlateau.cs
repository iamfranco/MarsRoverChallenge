using MarsRover.Models.Positions;

namespace MarsRover.Models.Plateaus
{
    public class RectangularPlateau : PlateauBase
    {
        private readonly Coordinates _maximumCoordinates;

        public RectangularPlateau(Coordinates maximumCoordinates)
        {
            if (!IsValidMaximumCoordinates(maximumCoordinates))
                throw new ArgumentException($"Maximum Coordinates {maximumCoordinates} cannot have negative components");

            _maximumCoordinates = maximumCoordinates;
        }

        public override void PrintMap(List<Position> recentPath)
        {
            int maxX = _maximumCoordinates.X;
            int maxY = _maximumCoordinates.Y;
            var obstacles = ObstaclesContainer.ObstacleCoordinates;

            if (maxX > 40 || maxY > 40)
            {
                Console.WriteLine($"Rectangular plateau: width [{maxX}] and height [{maxY}]");
                Console.WriteLine($"Obstacles Count: {obstacles.Count}");

                if (obstacles.Count > 0)
                {
                    Console.WriteLine("Obstacles at Coordinates:");
                    foreach (var obstacle in obstacles)
                    {
                        Console.WriteLine($"   {obstacle}");
                    }
                }

                return;
            }

            ConsoleColor defaultBGColor = Console.BackgroundColor;
            
            (string symbol, ConsoleColor bgColor)[,] matrixToPrint = GetMatrixToPrint(recentPath, 
                validGroundColor : ConsoleColor.Blue, 
                visitedGroundColor : ConsoleColor.DarkBlue, 
                lastVisitedGroundColor : ConsoleColor.Red, 
                invalidGroundColor : ConsoleColor.DarkGray,
                availableVehicleColor : ConsoleColor.DarkYellow);

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
            ConsoleColor validGroundColor, 
            ConsoleColor visitedGroundColor,
            ConsoleColor lastVisitedGroundColor, 
            ConsoleColor invalidGroundColor,
            ConsoleColor availableVehicleColor)
        {
            (string symbol, ConsoleColor bgColor)[,] matrixToPrint = new (string, ConsoleColor)[_maximumCoordinates.X + 1, _maximumCoordinates.Y + 1];

            for (int i = 0; i < matrixToPrint.GetLength(0); i++)
            {
                for (int j = 0; j < matrixToPrint.GetLength(1); j++)
                {
                    matrixToPrint[i, j] = (" ", validGroundColor);
                }
            }

            foreach (var obstacleCoordinate in ObstaclesContainer.ObstacleCoordinates)
            {
                matrixToPrint[obstacleCoordinate.X, obstacleCoordinate.Y] = ("X", invalidGroundColor);
            }

            foreach (var vehicle in VehiclesContainer.Vehicles)
            {
                int x = vehicle.Position.Coordinates.X;
                int y = vehicle.Position.Coordinates.Y;
                string symbol = vehicle.Position.Direction switch
                {
                    Direction.North => "\u2191",
                    Direction.East => ">",
                    Direction.South => "\u2193",
                    Direction.West => "<",
                    _ => " "
                };
                matrixToPrint[x, y] = (symbol, availableVehicleColor);
            }

            if (recentPath.Count == 0)
                return matrixToPrint;

            foreach (var recentPathItem in recentPath)
            {
                int x = recentPathItem.Coordinates.X;
                int y = recentPathItem.Coordinates.Y;
                string symbol = recentPathItem.Direction switch
                {
                    Direction.North => "\u2191",
                    Direction.East => ">",
                    Direction.South => "\u2193",
                    Direction.West => "<",
                    _ => " "
                };
                matrixToPrint[x, y] = (symbol, visitedGroundColor);
            }

            int lastX = recentPath.Last().Coordinates.X;
            int lastY = recentPath.Last().Coordinates.Y;
            matrixToPrint[lastX, lastY].bgColor = lastVisitedGroundColor;
            return matrixToPrint;
        }

        private static bool IsValidMaximumCoordinates(Coordinates maximumCoordinates) => maximumCoordinates.X >= 0 && maximumCoordinates.Y >= 0;

        protected override bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates)
        {
            if (coordinates.X < 0 || coordinates.X > _maximumCoordinates.X)
                return false;

            if (coordinates.Y < 0 || coordinates.Y > _maximumCoordinates.Y)
                return false;

            return true;
        }
    }
}
