using System.Text.RegularExpressions;
using MarsRover.Models.Elementals;

namespace MarsRover.AppUI.PositionStringFormat;

public class StandardPositionStringConverter : IPositionStringConverter
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

        (var coordinateString, var directionString) = SplitPositionString(positionString!);

        var direction = _directionSymbolEnum
            .FirstOrDefault(item => item.directionSymbol == directionString)
            .direction;

        return new(ToCoordinates(coordinateString), direction);
    }

    public Coordinates ToCoordinates(string? coordinateString)
    {
        if (!IsValidCoordinateString(coordinateString))
            throw new ArgumentException($"Input {coordinateString} is not in correct format for coordianteString (eg \"{ExampleCoordinateString}\")");

        var coordinateStringArray = coordinateString!.Split(" ");
        var x = int.Parse(coordinateStringArray[0]);
        var y = int.Parse(coordinateStringArray[1]);

        return new Coordinates(x, y);
    }

    public string ToPositionString(Position position)
    {
        var directionSymbol = _directionSymbolEnum
            .FirstOrDefault(x => x.direction == position.Direction)
            .directionSymbol;

        return $"{position.Coordinates.X} {position.Coordinates.Y} {directionSymbol}";
    }

    private static bool IsNotNullAndMatchRegex(string? str, Regex regex) => str is not null && regex.IsMatch(str);

    private static (string coordinateString, string directionString) SplitPositionString(string positionString)
    {
        var lastSpaceIndex = positionString.LastIndexOf(" ");

        var coordinateString = positionString[..lastSpaceIndex];
        var directionString = positionString[(lastSpaceIndex + 1)..];

        return (coordinateString, directionString);
    }
}
