using System.Text.RegularExpressions;

namespace MarsRover.Models.Positions
{
    public class PositionStringConverter : IPositionStringConverter
    {
        private readonly Regex _positionRegex = new(@"^\d+ \d+ (N|E|S|W)$");
        public bool IsValidPositionString(string position) => _positionRegex.IsMatch(position);

        public (Coordinates, Direction) ToCoordinatesDirection(string position)
        {
            if (!IsValidPositionString(position))
                throw new ArgumentException("Invalid position string", nameof(position));

            string[] positionComponents = position.Split(' ');

            if (!int.TryParse(positionComponents[0], out int x))
                throw new ArgumentException("Invalid X component in position string", nameof(position));

            if (!int.TryParse(positionComponents[1], out int y))
                throw new ArgumentException("Invalid Y component in position string", nameof(position));

            return (new Coordinates(x, y), new Direction(positionComponents[2]));
        }

        public string ToPositionString(Coordinates coordinates, Direction direction) =>
            $"{coordinates.X} {coordinates.Y} {direction.Char}";
    }
}