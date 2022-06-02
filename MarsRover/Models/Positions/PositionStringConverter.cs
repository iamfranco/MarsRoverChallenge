using System.Text.RegularExpressions;

namespace MarsRover.Models.Positions;

public class PositionStringConverter : IPositionStringConverter
{
    private readonly Regex _positionStringRegex = new(@"^-?\d+ -?\d+ (N|E|S|W)$");
    private readonly Regex _coordinateStringRegex = new(@"^-?\d+ -?\d+$");
    private static readonly List<(string directionSymbol, Direction direction)> _directionSymbolEnum = new()
    {
        ("N", Direction.North),
        ("E", Direction.East),
        ("S", Direction.South),
        ("W", Direction.West)
    };

    public string ExamplePositionString => "1 2 N";

    public string ExampleCoordinateString => "1 2";

    public bool IsValidPositionString(string? positionString) => IsNotNullAndMatchRegex(positionString, _positionStringRegex);

    public bool IsValidCoordinateString(string? coordinateString) => IsNotNullAndMatchRegex(coordinateString, _coordinateStringRegex);

    public Position ToPosition(string? positionString)
    {
        if (!IsValidPositionString(positionString))
            throw new ArgumentException($"Input {positionString} is not in correct format for positionString (eg \"{ExamplePositionString}\")");

        (string coordinateString, string directionString) = SplitPositionString(positionString!);

        Direction direction = _directionSymbolEnum
            .FirstOrDefault(item => item.directionSymbol == directionString)
            .direction;

        return new(ToCoordinates(coordinateString), direction);
    }

    public Coordinates ToCoordinates(string? coordinateString)
    {
        if (!IsValidCoordinateString(coordinateString))
            throw new ArgumentException($"Input {coordinateString} is not in correct format for coordianteString (eg \"{ExampleCoordinateString}\")");

        string[] coordinateStringArray = coordinateString!.Split(" ");
        int x = int.Parse(coordinateStringArray[0]);
        int y = int.Parse(coordinateStringArray[1]);

        return new Coordinates(x, y);
    }

    public string ToPositionString(Position position)
    {
        string directionSymbol = _directionSymbolEnum
            .FirstOrDefault(x => x.direction == position.Direction)
            .directionSymbol;

        return $"{position.Coordinates.X} {position.Coordinates.Y} {directionSymbol}";
    }

    private static bool IsNotNullAndMatchRegex(string? str, Regex regex) => str is not null && regex.IsMatch(str);

    private static (string coordinateString, string directionString) SplitPositionString(string positionString)
    {
        int lastSpaceIndex = positionString.LastIndexOf(" ");

        string coordinateString = positionString[..lastSpaceIndex];
        string directionString = positionString[(lastSpaceIndex + 1)..];

        return (coordinateString, directionString);
    }
}
