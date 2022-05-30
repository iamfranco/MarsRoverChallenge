using MarsRover.Models.Positions;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public abstract class PlateauBase
    {
        private List<Coordinates> _obstacleCoordinates = new();

        public ReadOnlyCollection<Coordinates> ObstacleCoordinates => _obstacleCoordinates.AsReadOnly();

        public void AddObstacle(Coordinates obstacle)
        {
            if (IsCoordinateValidInPlateau(obstacle))
                _obstacleCoordinates.Add(obstacle);
        }

        public void RemoveObstacle(Coordinates obstacle) => _obstacleCoordinates.Remove(obstacle);
        
        public abstract bool IsCoordinateValidInPlateau(Coordinates coordinates);
        public abstract void PrintMap(List<Position> recentPath);
    }
}