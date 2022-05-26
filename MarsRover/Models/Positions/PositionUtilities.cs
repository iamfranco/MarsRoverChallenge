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

            return (new Coordinates(x, y), new Direction(positionComponents[2]));
        }
    }
}