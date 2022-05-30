namespace MarsRover.Models.Positions
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public static class DirectionExtensions
    {
        public static Coordinates GetMovementVector(this Direction direction)
        {
            return direction switch
            {
                Direction.North => new(0, 1),
                Direction.South => new(0, -1),
                Direction.East => new(1, 0),
                Direction.West => new(-1, 0),
                _ => new(0, 0)
            };
        }

        public static Direction GetLeftTurn(this Direction direction) => (Direction)(((int)direction + 3) % 4);
        public static Direction GetRightTurn(this Direction direction) => (Direction)(((int)direction + 1) % 4);
    }
}
