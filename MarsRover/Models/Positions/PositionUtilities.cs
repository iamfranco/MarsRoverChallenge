namespace MarsRover.Models.Positions
{
    public static class PositionUtilities
    {
        public static (Coordinates, Direction) GetCoordinatesDirectionFromPosition(string position)
        {
            string[] positionComponents = position.Split(' ');
            if (positionComponents.Length != 3)
                throw new ArgumentException("Invalid position string", nameof(position));

            if (!int.TryParse(positionComponents[0], out int x))
                throw new ArgumentException("Invalid X component in position string", nameof(position));

            if (!int.TryParse(positionComponents[1], out int y))
                throw new ArgumentException("Invalid Y component in position string", nameof(position));

            Direction direction = positionComponents[2] switch
            {
                "N" => Direction.N,
                "E" => Direction.E,
                "S" => Direction.S,
                "W" => Direction.W,
                _ => throw new ArgumentException("Invalid compass orientation in position string", nameof(position))
            };

            return (new Coordinates(x, y), direction);
        }

        public static Direction GetDirectionAfterClockwiseRotation(Direction direction, int rotateCount)
        {
            const int directionsCount = 4;
            return (Direction)((int)(direction + directionsCount + rotateCount) % directionsCount);
        }

        public static Coordinates GetForwardCoordinate(Coordinates nextCoordinate, Direction direction)
        {
            if (direction is Direction.N)
                return nextCoordinate + new Coordinates(0, 1);

            if (direction is Direction.E)
                return nextCoordinate + new Coordinates(1, 0);

            if (direction is Direction.S)
                return nextCoordinate + new Coordinates(0, -1);

            if (direction is Direction.W)
                return nextCoordinate + new Coordinates(-1, 0);

            return nextCoordinate;
        }
    }
}