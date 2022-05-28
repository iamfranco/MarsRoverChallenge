using MarsRover.Models.Positions;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public class RectangularPlateau
    {
        private List<Coordinates> _obstacleCoordinates = new();
        public Coordinates PlateauSize { get; }

        public ReadOnlyCollection<Coordinates> ObstacleCoordinates => _obstacleCoordinates.AsReadOnly();

        public RectangularPlateau(Coordinates plateauSize)
        {
            if (!IsValidPlateauSize(plateauSize))
                throw new ArgumentException($"{nameof(plateauSize)} {plateauSize} cannot have negative components", nameof(plateauSize));

            PlateauSize = plateauSize;
        }

        public void AddObstacle(Coordinates obstacle)
        {
            _obstacleCoordinates.Add(obstacle);
            _obstacleCoordinates = _obstacleCoordinates.Distinct().ToList();
        }

        public void RemoveObstacle(Coordinates obstacle) => _obstacleCoordinates.Remove(obstacle);

        public bool IsCoordinateValidInPlateau(Coordinates coordinates)
        {
            if (coordinates.X < 0 || coordinates.X > PlateauSize.X)
                return false;

            if (coordinates.Y < 0 || coordinates.Y > PlateauSize.Y)
                return false;

            if (_obstacleCoordinates.Contains(coordinates))
                return false;

            return true;
        }
        private static bool IsValidPlateauSize(Coordinates plateauSize) => plateauSize.X >= 0 && plateauSize.Y >= 0;
    }
}
