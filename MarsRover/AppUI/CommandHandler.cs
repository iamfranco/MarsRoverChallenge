using MarsRover.Models.Instructions;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI
{
    public class CommandHandler
    {
        private readonly IInstructionReader _instructionReader;
        private readonly IPositionStringConverter _positionStringConverter;
        private VehicleBase? _vehicle;

        public List<(Coordinates, DirectionEnum)> RecentPath { get; private set; } = new();

        public CommandHandler(IInstructionReader instructionReader, IPositionStringConverter positionStringConverter)
        {
            _instructionReader = instructionReader;
            _positionStringConverter = positionStringConverter;
        }

        public string RequestPosition()
        {
            if (_vehicle is null)
                return "No Vehicle Connected";

            return _positionStringConverter.ToPositionString(_vehicle.Coordinates, _vehicle.Direction);
        }

        public void ConnectVehicle(VehicleBase vehicle)
        {
            if (vehicle is null)
                return;

            _vehicle = vehicle;
            RecentPath = new();
        }

        public void DisconnectVehicle()
        {
            _vehicle = null;
            RecentPath = new();
        }

        public (bool status, string message) SendMoveInstruction(string instructionString)
        {
            if (_vehicle is null)
                return (false, "Instruction is null");
            
            if (string.IsNullOrEmpty(instructionString))
                return (false, "Empty Instruction");

            if (!_instructionReader.IsValidInstruction(instructionString))
                return (false, "Instruction not in correct format");

            List<SingularInstruction> instruction = _instructionReader.EvaluateInstruction(instructionString);

            RecentPath = new() { (_vehicle.Coordinates, _vehicle.Direction) };

            foreach (SingularInstruction instructionItem in instruction)
            {
                (Coordinates nextCoordinate, DirectionEnum nextDirection) = RecentPath.Last();

                if (instructionItem is SingularInstruction.TurnLeft)
                    nextDirection = nextDirection.GetLeftTurn();

                if (instructionItem is SingularInstruction.TurnRight)
                    nextDirection = nextDirection.GetRightTurn();

                if (instructionItem is SingularInstruction.MoveForward)
                    nextCoordinate += nextDirection.GetMovementVector();

                if (!_vehicle.Plateau.IsCoordinateValidInPlateau(nextCoordinate))
                    return (false, $"Instruction will move vehicle into invalid coordinate {nextCoordinate}");

                RecentPath.Add((nextCoordinate, nextDirection));
            }

            _vehicle.ApplyMoveInstruction(instruction);

            return (true, "Instruction successfully sent.");
        }
    }
}
