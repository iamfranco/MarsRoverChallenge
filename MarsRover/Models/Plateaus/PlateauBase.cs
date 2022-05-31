using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public abstract class PlateauBase
    {
        private List<Coordinates> _obstacleCoordinates = new();
        private List<VehicleBase> _vehicles = new();

        public ReadOnlyCollection<Coordinates> ObstacleCoordinates => _obstacleCoordinates.AsReadOnly();

        public void AddObstacle(Coordinates obstacle)
        {
            if (IsCoordinateValidInPlateau(obstacle))
                _obstacleCoordinates.Add(obstacle);
        }

        public void RemoveObstacle(Coordinates obstacle) => _obstacleCoordinates.Remove(obstacle);

        public ReadOnlyCollection<VehicleBase> GetVehicles() => _vehicles.AsReadOnly();

        public VehicleBase? GetVehicleAtPosition(Position position)
        {
            return _vehicles.FirstOrDefault(vehicle => vehicle.Position.Equals(position));
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

            if (_obstacleCoordinates.Contains(coordinates))
                return false;

            if (_vehicles.Select(vehicle => vehicle.Position.Coordinates).Contains(coordinates))
                return false;

            return true;
        }
        public abstract void PrintMap(List<Position> recentPath);
        protected abstract bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates);
    }
}