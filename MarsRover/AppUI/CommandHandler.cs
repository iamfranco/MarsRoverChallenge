using MarsRover.AppUI.Components;
using MarsRover.Models.Instructions;
using MarsRover.Models.Instructions.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;

public class CommandHandler
{
    private readonly IInstructionReader _instructionReader;
    private readonly IPositionStringConverter _positionStringConverter;
    private PlateauBase? _plateau;
    private VehicleBase? _vehicle;

    public MapPrinter MapPrinter { get; }

    public List<Position> RecentPath { get; private set; } = new();

    public CommandHandler(IInstructionReader instructionReader, IPositionStringConverter positionStringConverter, 
        MapPrinter mapPrinter)
    {
        if (instructionReader is null)
            throw new ArgumentNullException(nameof(instructionReader));

        if (positionStringConverter is null)
            throw new ArgumentNullException(nameof(positionStringConverter));

        if (mapPrinter is null)
            throw new ArgumentNullException(nameof(mapPrinter));

        _instructionReader = instructionReader;
        _positionStringConverter = positionStringConverter;
        MapPrinter = mapPrinter;
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

        VehicleBase? vehicle = _plateau.VehiclesContainer.GetVehicleAtCoordinates(coordinates);
        if (vehicle is null)
            throw new ArgumentException($"Coordinates {coordinates} does not match any vehicle's coordinates on plateau");

        SetVehicle(vehicle);
    }

    public string SendMoveInstruction(string instructionString)
    {
        if (_plateau is null)
            throw new Exception("Plateau not connected, cannot send instruction");

        if (_vehicle is null)
            throw new Exception("Vehicle not connected, cannot send instruction");

        if (string.IsNullOrEmpty(instructionString))
            return $"Instruction is empty, vehicle is in the same Position: [{GetPositionString()}]";

        if (!_instructionReader.IsValidInstruction(instructionString))
            throw new ArgumentException($"Instruction [{instructionString}] is not in correct format");

        List<SingularInstruction> instruction = _instructionReader.EvaluateInstruction(instructionString);

        (RecentPath, bool isEmergencyStopUsed) = _vehicle.ApplyMoveInstruction(instruction, _plateau);
        if (isEmergencyStopUsed)
            return $"Vehicle sensed danger ahead, so stopped at [{GetPositionString()}] instead of applying full instruction [{instructionString}]";

        return $"Instruction [{instructionString}] lead to Position: [{GetPositionString()}]";
    }

    public string GetPositionString()
    {
        if (_vehicle is null)
            return "Vehicle not connected";

        return _positionStringConverter.ToPositionString(_vehicle.Position);
    }

    public void ResetRecentPath() => RecentPath = (_vehicle is null) ? new() : new() { _vehicle.Position };

    private void SetVehicle(VehicleBase vehicle)
    {
        _vehicle = vehicle;
        ResetRecentPath();
    }
}
