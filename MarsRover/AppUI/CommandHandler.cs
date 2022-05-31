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

        public List<Position> RecentPath { get; private set; } = new();

        public CommandHandler(IInstructionReader instructionReader, IPositionStringConverter positionStringConverter)
        {
            _instructionReader = instructionReader;
            _positionStringConverter = positionStringConverter;
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

        public string RequestPosition()
        {
            if (_vehicle is null)
                return "No Vehicle Connected";

            return _positionStringConverter.ToPositionString(_vehicle.Position);
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

            RecentPath = new() { _vehicle.Position };

            foreach (SingularInstruction instructionItem in instruction)
            {
                Coordinates nextCoordinate = RecentPath.Last().Coordinates;
                Direction nextDirection = RecentPath.Last().Direction;

                if (instructionItem is SingularInstruction.TurnLeft)
                    nextDirection = nextDirection.GetLeftTurn();

                if (instructionItem is SingularInstruction.TurnRight)
                    nextDirection = nextDirection.GetRightTurn();

                if (instructionItem is SingularInstruction.MoveForward)
                    nextCoordinate += nextDirection.GetMovementVector();

                //if (!_vehicle.Plateau.IsCoordinateValidInPlateau(nextCoordinate))
                //    return (false, $"Instruction will move vehicle into invalid coordinate {nextCoordinate}");

                RecentPath.Add(new Position(nextCoordinate, nextDirection));
            }

            //_vehicle.ApplyMoveInstruction(instruction);

            return (true, "Instruction successfully sent.");
        }
    }
}
