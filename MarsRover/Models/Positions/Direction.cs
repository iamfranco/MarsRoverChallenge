namespace MarsRover.Models.Positions
{
    public class Direction
    {
        private readonly List<(char directionChar, Coordinates directionCoordinates)> _directions = new()
        {
            ('N', new( 0, 1)),
            ('E', new( 1, 0)),
            ('S', new( 0,-1)),
            ('W', new(-1, 0)),
        };

        private int _directionIndex;

        public Direction(char directionChar)
        {
            int index = _directions.FindIndex(item => item.directionChar == directionChar);

            if (index == -1)
                throw new ArgumentException("invalid direction", nameof(directionChar));

            _directionIndex = index;
        }
        public Direction(string directionString) : this(directionString[0]) { }
        
        public Direction(Direction thisDirection)
        {
            _directionIndex = thisDirection._directionIndex;
        }
        public Direction Clone() => new Direction(this);

        public char Char => _directions[_directionIndex].directionChar;
        public Coordinates Coordinates => _directions[_directionIndex].directionCoordinates;

        public void TurnRight() => IncreaseIndex(+1);
        public void TurnLeft() => IncreaseIndex(-1);

        private void IncreaseIndex(int amount)
        {
            int directionCount = _directions.Count;
            _directionIndex = (_directionIndex + directionCount + amount) % directionCount;
        }

    }
}
