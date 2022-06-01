using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public abstract class PlateauBase
    {
        private List<Coordinates> _obstacleCoordinates = new();
        private List<VehicleBase> _vehicles = new();

        public ObstaclesContainer ObstaclesContainer { get; }
        public VehiclesContainer VehiclesContainer { get; }

        public PlateauBase()
        {
            ObstaclesContainer = new ObstaclesContainer(IsCoordinateValidInPlateau);
            VehiclesContainer = new VehiclesContainer(IsCoordinateValidInPlateau);
        }

        public ReadOnlyCollection<Coordinates> ObstacleCoordinates => _obstacleCoordinates.AsReadOnly();

        public void AddObstacle(Coordinates obstacle)
        {
            if (IsCoordinateValidInPlateau(obstacle))
                _obstacleCoordinates.Add(obstacle);
        }

        public void RemoveObstacle(Coordinates obstacle) => _obstacleCoordinates.Remove(obstacle);

        public ReadOnlyCollection<VehicleBase> GetVehicles() => _vehicles.AsReadOnly();

        public VehicleBase? GetVehicleAtCoordinates(Coordinates coordinates)
        {
            return _vehicles.FirstOrDefault(vehicle => vehicle.Position.Coordinates.Equals(coordinates));
        }

        public void AddVehicle(VehicleBase vehicle)
        {
            if (IsCoordinateValidInPlateau(vehicle.Position.Coordinates))
                _vehicles.Add(vehicle);
        }

        public void RemoveVehicle(VehicleBase vehicle) => _vehicles.Remove(vehicle);

        public bool IsCoordinateValidInPlateau(Coordinates coordinates)
        {
            if (!IsCoordinateWithinPlateauBoundary(coordinates))
                return false;

            if (ObstaclesContainer.ObstacleCoordinates.Contains(coordinates))
                return false;

            if (VehiclesContainer.Vehicles.Select(vehicle => vehicle.Position.Coordinates).Contains(coordinates))
                return false;

            return true;
        }
        public abstract void PrintMap(List<Position> recentPath);
        protected abstract bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates);
    }
}