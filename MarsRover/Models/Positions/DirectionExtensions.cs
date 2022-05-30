namespace MarsRover.Models.Positions
{
    public enum DirectionEnum
    {
        North,
        East,
        South,
        West
    }

    public static class DirectionExtensions
    {
        public static Coordinates GetMovementVector(this DirectionEnum direction)
        {
            return direction switch
            {
                DirectionEnum.North => new(0, 1),
                DirectionEnum.South => new(0, -1),
                DirectionEnum.East => new(1, 0),
                DirectionEnum.West => new(-1, 0),
                _ => new(0, 0)
            };
        }

        public static DirectionEnum GetLeftTurn(this DirectionEnum direction) => (DirectionEnum)(((int)direction + 3) % 4);
        public static DirectionEnum GetRightTurn(this DirectionEnum direction) => (DirectionEnum)(((int)direction + 1) % 4);
    }
}
