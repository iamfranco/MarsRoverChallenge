namespace MarsRover.Models.Positions
{
    public interface IPositionStringConverter
    {
        bool IsValidPositionString(string? positionString);
        bool IsValidCoordinateString(string? coordinateString);
        Position ToPosition(string? positionString);
        Coordinates ToCoordinates(string? coordinateString);
        string ToPositionString(Position position);
    }
}