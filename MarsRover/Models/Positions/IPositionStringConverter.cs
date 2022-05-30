namespace MarsRover.Models.Positions
{
    public interface IPositionStringConverter
    {
        bool IsValidCoordinateString(string? coordinateString);
        bool IsValidPositionString(string? positionString);
        Coordinates ToCoordinates(string? coordinateString);
        (Coordinates, DirectionEnum) ToCoordinatesDirection(string? positionString);
        string ToPositionString(Coordinates coordinates, DirectionEnum direction);
    }
}