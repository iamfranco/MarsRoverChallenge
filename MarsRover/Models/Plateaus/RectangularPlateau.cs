using MarsRover.Helpers;
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
            Guard.ThrowIfBelowZero(maximumCoordinates.X);
            Guard.ThrowIfBelowZero(maximumCoordinates.Y);

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
