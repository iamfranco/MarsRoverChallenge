namespace MarsRover.Models.Positions;

public interface IPositionStringConverter
{
    string ExamplePositionString { get; }
    string ExampleCoordinateString { get; }
    bool IsValidPositionString(string? positionString);
    bool IsValidCoordinateString(string? coordinateString);
    Position ToPosition(string? positionString);
    Coordinates ToCoordinates(string? coordinateString);
    string ToPositionString(Position position);
}
