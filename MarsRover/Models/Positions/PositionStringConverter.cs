using System.Text.RegularExpressions;

namespace MarsRover.Models.Positions
{
    public class PositionStringConverter : IPositionStringConverter
    {
        private readonly Regex _positionStringRegex = new(@"^-?\d+ -?\d+ (N|E|S|W)$");
        private readonly Regex _coordinateStringRegex = new(@"^-?\d+ -?\d+$");
        private static List<(string directionSymbol, string directionName)> _directionSymbolNames = new()
        {
            ("N", "north"),
            ("E", "east"),
            ("S", "south"),
            ("W", "west")
        };

        public bool IsValidPositionString(string? positionString) => IsNotNullAndMatchRegex(positionString, _positionStringRegex);

        public bool IsValidCoordinateString(string? coordinateString) => IsNotNullAndMatchRegex(coordinateString, _coordinateStringRegex);

        public (Coordinates, Direction) ToCoordinatesDirection(string? positionString)
        {
            if (!IsValidPositionString(positionString))
                throw new ArgumentException($"Input {positionString} is not in correct format for positionString (eg \"1 2 N\")", nameof(positionString));

            (string coordinateString, string directionString) = SplitPositionString(positionString!);

            return (ToCoordinates(coordinateString), new Direction(directionString));
        }

        public Coordinates ToCoordinates(string? coordinateString)
        {
            if (!IsValidCoordinateString(coordinateString))
                throw new ArgumentException($"Input {coordinateString} is not in correct format for coordianteString (eg \"1 2\")", nameof(coordinateString));

            string[] coordinateStringArray = coordinateString!.Split(" ");
            int x = int.Parse(coordinateStringArray[0]);
            int y = int.Parse(coordinateStringArray[1]);

            return new Coordinates(x, y);
        }

        public string ToPositionString(Coordinates coordinates, Direction? direction)
        {
            if (direction is null)
                throw new ArgumentNullException(nameof(direction));

            string directionSymbol = _directionSymbolNames
                .FirstOrDefault(x => x.directionName == direction.Name)
                .directionSymbol;

            return $"{coordinates.X} {coordinates.Y} {directionSymbol}";

            throw new NotImplementedException();
        }

        private static bool IsNotNullAndMatchRegex(string? str, Regex regex) => str is not null && regex.IsMatch(str);

        private static (string coordinateString, string directionString) SplitPositionString(string positionString)
        {
            int lastSpaceIndex = positionString.LastIndexOf(" ");

            string coordinateString = positionString[..lastSpaceIndex];
            string directionString = _directionSymbolNames
                .FirstOrDefault(x => x.directionSymbol == positionString[(lastSpaceIndex + 1)..])
                .directionName;

            return (coordinateString, directionString);
        }
    }
}
