using MarsRover.Controllers.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.Controllers;

public class AppController
{
    private readonly IInstructionReader _instructionReader;
    private PlateauBase? _plateau;
    private VehicleBase? _vehicle;

    public List<Position> RecentPath { get; private set; } = new();

    public AppController(IInstructionReader instructionReader)
    {
        if (instructionReader is null)
            throw new ArgumentNullException(nameof(instructionReader));

        _instructionReader = instructionReader;
    }

    public PlateauBase? GetPlateau() => _plateau;

    public void ConnectPlateau(PlateauBase plateau)
    {
        if (plateau is null)
            throw new ArgumentNullException(nameof(plateau));

        _plateau = plateau;
        _vehicle = null;
        ResetRecentPath();
    }

    public VehicleBase? GetVehicle() => _vehicle;

    public void AddVehicleToPlateau(VehicleBase vehicle)
    {
        if (vehicle is null)
            throw new ArgumentNullException(nameof(vehicle));

        if (_plateau is null)
            throw new Exception("Plateau not connected, cannot add vehicle");

        if (!_plateau.IsCoordinateValidInPlateau(vehicle.Position.Coordinates))
            throw new ArgumentException($"Vehicle Coordinates {vehicle.Position.Coordinates} is not valid in plateau");

        _plateau.VehiclesContainer.AddVehicle(vehicle);
        SetVehicle(vehicle);
    }

    public void ConnectToVehicleAtCoordinates(Coordinates coordinates)
    {
        if (_plateau is null)
            throw new Exception("Plateau not connected, cannot add vehicle");

        if (_plateau.VehiclesContainer.Vehicles.Count == 0)
            throw new Exception("Plateau has no vehicles, please create a vehicle first");

        var vehicle = _plateau.VehiclesContainer.GetVehicleAtCoordinates(coordinates);
        if (vehicle is null)
            throw new ArgumentException($"Coordinates {coordinates} does not match any vehicle's coordinates on plateau");

        SetVehicle(vehicle);
    }

    public VehicleMovementStatus SendMoveInstruction(string instructionString)
    {
        if (_plateau is null)
            throw new Exception("Plateau not connected, cannot send instruction");

        if (_vehicle is null)
            throw new Exception("Vehicle not connected, cannot send instruction");

        if (string.IsNullOrEmpty(instructionString))
            return VehicleMovementStatus.NoMovement;

        if (!_instructionReader.IsValidInstruction(instructionString))
            throw new ArgumentException($"Instruction [{instructionString}] is not in correct format");

        var instruction = _instructionReader.EvaluateInstruction(instructionString);

        (RecentPath, var isEmergencyStopUsed) = _vehicle.ApplyMoveInstruction(instruction, _plateau);
        if (isEmergencyStopUsed)
            return VehicleMovementStatus.DangerAhead;

        return VehicleMovementStatus.ReachedDestination;
    }

    public void ResetRecentPath() => RecentPath = _vehicle is null ? new() : new() { _vehicle.Position };

    private void SetVehicle(VehicleBase vehicle)
    {
        _vehicle = vehicle;
        ResetRecentPath();
    }
}
