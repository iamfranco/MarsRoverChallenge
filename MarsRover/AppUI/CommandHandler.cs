using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI
{
    public class CommandHandler
    {
        private readonly IInstructionReader _instructionReader;
        private readonly IPositionStringConverter _positionStringConverter;
        private PlateauBase? _plateau;
        private VehicleBase? _vehicle;

        public List<Position> RecentPath { get; private set; } = new();

        public CommandHandler(IInstructionReader instructionReader, IPositionStringConverter positionStringConverter)
        {
            if (instructionReader is null)
                throw new ArgumentNullException(nameof(instructionReader));

            if (positionStringConverter is null)
                throw new ArgumentNullException(nameof(positionStringConverter));

            _instructionReader = instructionReader;
            _positionStringConverter = positionStringConverter;
        }

        public void ConnectPlateau(PlateauBase plateau)
        {
            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            _plateau = plateau;
            _vehicle = null;
            ResetRecentPath();
        }

        public VehicleBase? GetVehicle() => _vehicle;

        public (bool status, string message) AddVehicleToPlateau(VehicleBase vehicle)
        {
            if (vehicle is null)
                throw new ArgumentNullException(nameof(vehicle));

            if (_plateau is null)
                return (false, "Plateau not connected");

            if (!_plateau.IsCoordinateValidInPlateau(vehicle.Position.Coordinates))
                return (false, $"Vehicle Coordinates {vehicle.Position.Coordinates} is not valid in plateau");

            _plateau.VehiclesContainer.AddVehicle(vehicle);
            return ConnectVehicleSuccessfully(vehicle);
        }

        public (bool status, string message) ConnectToVehicleAtCoordinates(Coordinates coordinates)
        {
            if (_plateau is null)
                return (false, "Plateau not connected");

            if (_plateau.VehiclesContainer.Vehicles.Count == 0)
                return (false, "Plateau has no vehicles, please create a vehicle first");

            VehicleBase? vehicle = _plateau.VehiclesContainer.GetVehicleAtCoordinates(coordinates);
            if (vehicle is null)
                return (false, $"Coordinates {coordinates} does not match any vehicle's coordinates on plateau");

            return ConnectVehicleSuccessfully(vehicle);
        }

        public (bool status, string message) SendMoveInstruction(string instructionString)
        {
            if (_plateau is null)
                return (false, "Plateau not connected");

            if (_vehicle is null)
                return (false, "Vehicle not connected");
            
            if (string.IsNullOrEmpty(instructionString))
                return (false, "Instruction is empty");

            if (!_instructionReader.IsValidInstruction(instructionString))
                return (false, "Instruction is not in correct format");

            List<SingularInstruction> instruction = _instructionReader.EvaluateInstruction(instructionString);

            (RecentPath, bool isEmergencyStopUsed) = _vehicle.ApplyMoveInstruction(instruction, _plateau);
            if (isEmergencyStopUsed)
                return (true, $"Vehicle sensed danger ahead, so stopped at [{GetPositionString()}] instead of applying full instruction [{instructionString}]");

            return (true, $"Instruction [{instructionString}] lead to Position: [{GetPositionString()}]");
        }

        public string GetPositionString()
        {
            if (_vehicle is null)
                return "Vehicle not connected";

            return _positionStringConverter.ToPositionString(_vehicle.Position);
        }

        public void ResetRecentPath() => RecentPath = (_vehicle is null) ? new() : new() { _vehicle.Position };

        private (bool status, string message) ConnectVehicleSuccessfully(VehicleBase vehicle)
        {
            _vehicle = vehicle;
            ResetRecentPath();
            return (true, "Vehicle Connected");
        }
    }
}
