using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.Controllers;

public class AppController
{
    public IInstructionReader InstructionReader { get; }
    public PlateauBase? Plateau { get; private set; }
    public VehicleBase? Vehicle { get; private set; }
    public List<Position> RecentPath { get; private set; } = new();

    public AppController(IInstructionReader instructionReader)
    {
        if (instructionReader is null)
            throw new ArgumentNullException(nameof(instructionReader));

        InstructionReader = instructionReader;
    }

    public void ConnectPlateau(PlateauBase plateau)
    {
        if (plateau is null)
            throw new ArgumentNullException(nameof(plateau));

        Plateau = plateau;
        Vehicle = null;
        ResetRecentPath();
    }

    public void AddObstacleToPlateau(Coordinates obstacle)
    {
        if (Plateau is null)
            throw new Exception("Plateau not connected, cannot add obstacle");

        Plateau.ObstaclesContainer.AddObstacle(obstacle);
    }

    public bool IsCoordinateValidInPlateau(Coordinates coordinates)
    {
        if (Plateau is null)
            throw new Exception("Plateau not connected");

        return Plateau.IsCoordinateValidInPlateau(coordinates);
    }

    public void AddVehicleToPlateau(VehicleBase vehicle)
    {
        if (vehicle is null)
            throw new ArgumentNullException(nameof(vehicle));

        if (Plateau is null)
            throw new Exception("Plateau not connected, cannot add vehicle");

        Plateau.VehiclesContainer.AddVehicle(vehicle);
        SetVehicle(vehicle);
    }

    public void ConnectToVehicleAtCoordinates(Coordinates coordinates)
    {
        if (Plateau is null)
            throw new Exception("Plateau not connected, cannot add vehicle");

        if (Plateau.VehiclesContainer.Vehicles.Count == 0)
            throw new Exception("Plateau has no vehicles, please create a vehicle first");

        VehicleBase? vehicle = Plateau.VehiclesContainer.GetVehicleAtCoordinates(coordinates);
        if (vehicle is null)
            throw new ArgumentException($"Coordinates {coordinates} does not match any vehicle's coordinates on plateau");

        SetVehicle(vehicle);
    }

    public void DisconnectVehicle()
    {
        Vehicle = null;
        ResetRecentPath();
    }

    public VehicleMovementStatus SendMoveInstruction(string instructionString)
    {
        if (Plateau is null)
            throw new Exception("Plateau not connected, cannot send instruction");

        if (Vehicle is null)
            throw new Exception("Vehicle not connected, cannot send instruction");

        if (string.IsNullOrEmpty(instructionString))
            return VehicleMovementStatus.NoMovement;

        if (!InstructionReader.IsValidInstruction(instructionString))
            throw new ArgumentException($"Instruction [{instructionString}] is not in correct format");

        List<SingularInstruction> instruction = InstructionReader.EvaluateInstruction(instructionString);

        (RecentPath, bool isEmergencyStopUsed) = Vehicle.ApplyMoveInstruction(instruction, Plateau);
        if (isEmergencyStopUsed)
            return VehicleMovementStatus.DangerAhead;

        return VehicleMovementStatus.ReachedDestination;
    }

    private void ResetRecentPath() => RecentPath = Vehicle is null ? new() : new() { Vehicle.Position };

    private void SetVehicle(VehicleBase vehicle)
    {
        Vehicle = vehicle;
        ResetRecentPath();
    }
}
