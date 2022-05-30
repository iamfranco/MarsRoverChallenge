namespace MarsRover.Models.Positions
{
    public interface IPositionStringConverter
    {
        bool IsValidPositionString(string? positionString);
        bool IsValidCoordinateString(string? coordinateString);
        (Coordinates, Direction) ToCoordinatesDirection(string? positionString);
        Coordinates ToCoordinates(string? coordinateString);
        string ToPositionString(Coordinates coordinates, Direction direction);
    }
}