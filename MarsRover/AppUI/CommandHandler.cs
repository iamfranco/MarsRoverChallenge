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

        public List<(Coordinates, Direction)> RecentPath { get; private set; } = new();

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
        }

        public void DisconnectVehicle()
        {
            _vehicle = null;
        }

        public bool SendMoveInstruction(string instructionString)
        {
            if (_vehicle is null)
                return false;
            
            if (string.IsNullOrEmpty(instructionString))
                return false;

            if (!_instructionReader.IsValidInstruction(instructionString))
                return false;

            List<SingularInstruction> instruction = _instructionReader.EvaluateInstruction(instructionString);

            RecentPath = new() { (_vehicle.Coordinates, _vehicle.Direction.Clone()) };

            foreach (SingularInstruction instructionItem in instruction)
            {
                (Coordinates nextCoordinate, Direction nextDirection) = RecentPath.Last();
                nextDirection = nextDirection.Clone();

                if (instructionItem is SingularInstruction.TurnLeft)
                    nextDirection.TurnLeft();

                if (instructionItem is SingularInstruction.TurnRight)
                    nextDirection.TurnRight();

                if (instructionItem is SingularInstruction.MoveForward)
                    nextCoordinate += nextDirection.MovementVector;

                if (!_vehicle.Plateau.IsCoordinateValidInPlateau(nextCoordinate))
                    return false;

                RecentPath.Add((nextCoordinate, nextDirection));
            }

            _vehicle.ApplyMoveInstruction(instruction);

            return true;
        }
    }
}
