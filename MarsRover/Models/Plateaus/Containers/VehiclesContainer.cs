using System.Collections.ObjectModel;
using MarsRover.Models.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.Models.Plateaus.Containers;

public class VehiclesContainer
{
    private readonly List<VehicleBase> _vehicles;
    private readonly Func<Coordinates, bool> _coordinateValidateFunc;

    public VehiclesContainer(Func<Coordinates, bool> coordinateValidateFunc)
    {
        if (coordinateValidateFunc is null)
            throw new ArgumentNullException(nameof(coordinateValidateFunc));

        _vehicles = new();
        _coordinateValidateFunc = coordinateValidateFunc;
    }

    public ReadOnlyCollection<VehicleBase> Vehicles => _vehicles.AsReadOnly();

    public VehicleBase? GetVehicleAtCoordinates(Coordinates coordinates)
    {
        return _vehicles.FirstOrDefault(vehicle => vehicle.Position.Coordinates.Equals(coordinates));
    }

    public void AddVehicle(VehicleBase vehicle)
    {
        if (!_coordinateValidateFunc(vehicle.Position.Coordinates))
            throw new ArgumentException($"Vehicle ({vehicle.GetType()}) has invalid " +
                $"coordinates {vehicle.Position.Coordinates} for plateau");

        _vehicles.Add(vehicle);
    }

    public void RemoveVehicle(VehicleBase vehicle) => _vehicles.Remove(vehicle);
}
