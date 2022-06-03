namespace MarsRover.Models.Elementals;

public struct Coordinates
{
    public int X { get; }
    public int Y { get; }

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Coordinates operator +(Coordinates a, Coordinates b) => new(a.X + b.X, a.Y + b.Y);

    public override string ToString() => $"({X}, {Y})";
}
