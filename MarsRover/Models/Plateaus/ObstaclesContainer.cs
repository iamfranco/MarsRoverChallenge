using MarsRover.Models.Positions;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public class ObstaclesContainer
    {
        private readonly Func<Coordinates, bool> _coordinateValidateFunc;
        private readonly List<Coordinates> _obstacleCoordinates;

        public ObstaclesContainer(Func<Coordinates, bool> coordinateValidateFunc)
        {
            if (coordinateValidateFunc is null)
                throw new ArgumentNullException(nameof(coordinateValidateFunc));

            _coordinateValidateFunc = coordinateValidateFunc;
            _obstacleCoordinates = new();
        }

        public ReadOnlyCollection<Coordinates> ObstacleCoordinates => _obstacleCoordinates.AsReadOnly();

        public void AddObstacle(Coordinates obstacle)
        {
            if (!_coordinateValidateFunc(obstacle))
                throw new ArgumentException($"Coordinates {obstacle} is not available on plateau");

            _obstacleCoordinates.Add(obstacle);
        }

        public void RemoveObstacle(Coordinates obstacle)
        {
            _obstacleCoordinates.Remove(obstacle);
        }
    }
}
