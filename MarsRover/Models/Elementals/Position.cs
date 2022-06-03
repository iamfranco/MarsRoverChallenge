namespace MarsRover.Models.Elementals;

public struct Position
{
    public Coordinates Coordinates { get; }
    public Direction Direction { get; }

    public Position(Coordinates coordinates, Direction direction)
    {
        Coordinates = coordinates;
        Direction = direction;
    }
}
