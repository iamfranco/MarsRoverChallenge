namespace MarsRover.Models.Positions
{
    public class Direction
    {
        private readonly List<(string Name, Coordinates MovementVector)> _directions = new()
        {
            ("north", new( 0, +1)),
            ("east" , new(+1,  0)),
            ("south", new( 0, -1)),
            ("west" , new(-1,  0)),
        };

        private int _directionIndex;

        public Direction(string? directionName)
        {
            if (directionName is null)
                throw new ArgumentNullException(nameof(directionName));

            _directionIndex = _directions.FindIndex(x => x.Name == directionName.ToLower());

            if (_directionIndex == -1)
                throw new ArgumentOutOfRangeException(nameof(directionName));
        }

        public Direction Clone() => new(Name);

        public string Name => _directions[_directionIndex].Name;
        public Coordinates MovementVector => _directions[_directionIndex].MovementVector;

        public void TurnLeft() => RotateByQuarters(-1);
        public void TurnRight() => RotateByQuarters(+1);

        private void RotateByQuarters(int amount)
        {
            int quartersCount = _directions.Count;
            _directionIndex = (_directionIndex + amount + quartersCount) % quartersCount;
        }
    }
}
