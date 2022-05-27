namespace MarsRover.Models.Positions
{
    public interface IPositionStringConverter
    {
        bool IsValidPositionString(string position);
        (Coordinates, Direction) ToCoordinatesDirection(string position);
        string ToPositionString(Coordinates coordinates, Direction direction);
    }
}