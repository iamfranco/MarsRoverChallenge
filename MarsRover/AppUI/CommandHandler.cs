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
        private VehicleBase? _vehicle;
        private PlateauBase? _plateau;

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

            RecentPath = new();
            _vehicle = null;
            _plateau = plateau;
        }

        public VehicleBase? GetVehicle() => _vehicle;

        public (bool status, string message) AddVehicleToPlateau(VehicleBase vehicle)
        {
            if (vehicle is null)
                throw new ArgumentNullException(nameof(vehicle));

            if (_plateau is null)
                return (false, "Plateau Not Connected");

            if (!_plateau.IsCoordinateValidInPlateau(vehicle.Position.Coordinates))
                return (false, "Vehicle Is On Invalid Coordinates For Plateau");

            _plateau.AddVehicle(vehicle);
            return ConnectVehicleSuccessfully(vehicle);
        }

        public (bool status, string message) ConnectToVehicleAtPosition(Position position)
        {
            if (_plateau is null)
                return (false, "Plateau Not Connected");

            if (_plateau.GetVehicles().Count == 0)
                return (false, "Plateau Has No Vehicles");

            VehicleBase? vehicle = _plateau.GetVehicleAtPosition(position);
            if (vehicle is null)
                return (false, "Position Does Not Match Any Vehicle's Position On Plateau");

            return ConnectVehicleSuccessfully(vehicle);
        }

        private (bool status, string message) ConnectVehicleSuccessfully(VehicleBase vehicle)
        {
            RecentPath = new() { vehicle.Position };
            _vehicle = vehicle;
            return (true, "Vehicle Connected");
        }

        public (bool status, string message) SendMoveInstruction(string instructionString)
        {
            if (_plateau is null)
                return (false, "Plateau Not Connected");

            if (_vehicle is null)
                return (false, "Vehicle Not Connected");
            
            if (string.IsNullOrEmpty(instructionString))
                return (false, "Empty Instruction");

            if (!_instructionReader.IsValidInstruction(instructionString))
                return (false, "Instruction not in correct format");

            List<SingularInstruction> instruction = _instructionReader.EvaluateInstruction(instructionString);

            (RecentPath, bool isEmergencyStopUsed) = _vehicle.ApplyMoveInstruction(instruction, _plateau);
            if (isEmergencyStopUsed)
            {
                string lastPositionString = _positionStringConverter.ToPositionString(RecentPath.Last());
                return (true, $"Vehicle sensed danger ahead, so stopped at [{lastPositionString}] instead of applying full instruction.");
            }

            return (true, "Instruction successfully sent.");
        }

        public string GetPositionString()
        {
            if (_vehicle is null)
                return "Vehicle Not Connected";

            return _positionStringConverter.ToPositionString(_vehicle.Position);
        }
    }
}
