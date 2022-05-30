using MarsRover.Models.Positions;

namespace MarsRover.Models.Plateaus
{
    public class RectangularPlateau : PlateauBase
    {
        public Coordinates PlateauSize { get; }

        public RectangularPlateau(Coordinates plateauSize)
        {
            if (!IsValidPlateauSize(plateauSize))
                throw new ArgumentException($"{nameof(plateauSize)} {plateauSize} cannot have negative components", nameof(plateauSize));

            PlateauSize = plateauSize;
        }

        public override bool IsCoordinateValidInPlateau(Coordinates coordinates)
        {
            if (coordinates.X < 0 || coordinates.X > PlateauSize.X)
                return false;

            if (coordinates.Y < 0 || coordinates.Y > PlateauSize.Y)
                return false;

            if (ObstacleCoordinates.Contains(coordinates))
                return false;

            return true;
        }

        public override void PrintMap(List<(Coordinates coordinates, Direction direction)> recentPath)
        {
            int width = PlateauSize.X;
            int height = PlateauSize.Y;

            if (width > 40 || height > 40)
            {
                Console.WriteLine($"Rectangular plateau: width [{width}] and height [{height}]");
                Console.WriteLine($"Obstacles Count: {ObstacleCoordinates.Count}");

                if (ObstacleCoordinates.Count > 0)
                {
                    Console.WriteLine("Obstacles at Coordinates:");
                    foreach (var obstacle in ObstacleCoordinates)
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
                invalidGroundColor : ConsoleColor.DarkGray);

            Console.WriteLine($"  Y");
            for (int y = height; y >= 0; y--)
            {
                Console.Write($"{y,3} ");
                for (int x = 0; x <= width; x++)
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
            for (int x = 0; x <= width; x++)
            {
                Console.Write($"{x,3} ");
            }
            Console.WriteLine("  X");
        }

        private (string symbol, ConsoleColor bgColor)[,] GetMatrixToPrint(
            List<(Coordinates coordinates, Direction direction)> recentPath, 
            ConsoleColor validGroundColor, 
            ConsoleColor visitedGroundColor,
            ConsoleColor lastVisitedGroundColor, 
            ConsoleColor invalidGroundColor)
        {
            (string symbol, ConsoleColor bgColor)[,] matrixToPrint = new (string, ConsoleColor)[PlateauSize.X + 1, PlateauSize.Y + 1];

            for (int i = 0; i < matrixToPrint.GetLength(0); i++)
            {
                for (int j = 0; j < matrixToPrint.GetLength(1); j++)
                {
                    matrixToPrint[i, j] = (" ", validGroundColor);
                }
            }

            foreach (var obstacleCoordinate in ObstacleCoordinates)
            {
                matrixToPrint[obstacleCoordinate.X, obstacleCoordinate.Y] = ("X", invalidGroundColor);
            }

            if (recentPath.Count == 0)
                return matrixToPrint;

            foreach (var recentPathItem in recentPath)
            {
                int x = recentPathItem.coordinates.X;
                int y = recentPathItem.coordinates.Y;
                string symbol = recentPathItem.direction.Name switch
                {
                    "north" => "\u2191",
                    "east" => ">",
                    "south" => "\u2193",
                    "west" => "<",
                    _ => " "
                };
                matrixToPrint[x, y] = (symbol, visitedGroundColor);
            }

            int lastX = recentPath.Last().coordinates.X;
            int lastY = recentPath.Last().coordinates.Y;
            matrixToPrint[lastX, lastY].bgColor = lastVisitedGroundColor;
            return matrixToPrint;
        }

        private static bool IsValidPlateauSize(Coordinates plateauSize) => plateauSize.X >= 0 && plateauSize.Y >= 0;
    }
}
