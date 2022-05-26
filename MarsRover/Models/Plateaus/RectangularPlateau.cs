using MarsRover.Models.Positions;
using System.Linq;

namespace MarsRover.Models.Plateaus
{
    public class RectangularPlateau : IPlateau
    {
        public Coordinates MaximumCoordinates { get; private set; }
        public List<Coordinates> ObstaclesCoordinates { get; private set; }

        public RectangularPlateau(Coordinates maximumCoordinates, List<Coordinates> obstacles)
        {
            if (maximumCoordinates.X < 0 || maximumCoordinates.Y < 0)
                throw new ArgumentException("Coordinate X,Y components cannot be negative", nameof(maximumCoordinates));

            MaximumCoordinates = maximumCoordinates;
            ObstaclesCoordinates = obstacles.Where(item => IsCoordinateValid(item)).ToList();
        }
        public RectangularPlateau(Coordinates maximumCoordinates) : this(maximumCoordinates, new List<Coordinates>())
        {
        }

        public bool IsCoordinateValid(Coordinates coordinates)
        {
            if (coordinates.X < 0 || coordinates.X > MaximumCoordinates.X)
                return false;

            if (coordinates.Y < 0 || coordinates.Y > MaximumCoordinates.Y)
                return false;

            if (ObstaclesCoordinates is not null && ObstaclesCoordinates.Contains(coordinates))
                return false;

            return true;
        }
    }
}
