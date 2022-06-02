using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public class VehiclesContainer
    {
        private readonly Func<Coordinates, bool> _coordinateValidateFunc;
        private readonly List<VehicleBase> _vehicles;

        public VehiclesContainer(Func<Coordinates, bool> coordinateValidateFunc)
        {
            if (coordinateValidateFunc is null)
                throw new ArgumentNullException(nameof(coordinateValidateFunc));

            _coordinateValidateFunc = coordinateValidateFunc;
            _vehicles = new();
        }

        public ReadOnlyCollection<VehicleBase> Vehicles => _vehicles.AsReadOnly();

        public void AddVehicle(VehicleBase vehicle)
        {
            if (!_coordinateValidateFunc(vehicle.Position.Coordinates))
                return;

            _vehicles.Add(vehicle);
        }

        public VehicleBase? GetVehicleAtCoordinates(Coordinates coordinates)
        {
            return _vehicles.FirstOrDefault(vehicle => vehicle.Position.Coordinates.Equals(coordinates));
        }

        public void RemoveVehicle(VehicleBase vehicle)
        {
            _vehicles.Remove(vehicle);
        }
    }
}
