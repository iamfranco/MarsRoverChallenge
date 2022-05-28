namespace MarsRover.Models.Positions
{
    public struct Coordinates
    {
        private readonly int _x;
        private readonly int _y;

        public Coordinates(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static Coordinates operator +(Coordinates a, Coordinates b) => new(a._x + b._x, a._y + b._y);

        public override string ToString() => $"({_x}, {_y})";
    }
}
