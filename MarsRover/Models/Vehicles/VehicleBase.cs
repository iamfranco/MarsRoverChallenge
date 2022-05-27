using MarsRover.Helpers;
using MarsRover.Models.MovementInstructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        private Coordinates _coordinates;
        private Direction _direction;
        private readonly IPlateau _plateau;
        private readonly IInstructionReader _instructionReader;

        public VehicleBase(string initialPosition, IPlateau plateau, IInstructionReader instructionReader)
        {
            _plateau = plateau;
            _instructionReader = instructionReader;

            TeleportToPosition(initialPosition);
        }

        public string Position => PositionStringConverter.ToPositionString(_coordinates, _direction);

        public void ApplyMoveInstruction(string instruction)
        {
            List<SingularInstruction> instructionList = _instructionReader.EvaluateInstruction(instruction);
            Coordinates nextCoordinate = _coordinates;
            Direction nextDirection = _direction.Clone();

            foreach (SingularInstruction singularInstruction in instructionList)
            {
                if (singularInstruction is SingularInstruction.TurnLeft)
                    nextDirection.TurnLeft();

                if (singularInstruction is SingularInstruction.TurnRight)
                    nextDirection.TurnRight();

                if (singularInstruction is SingularInstruction.MoveForward)
                    nextCoordinate += nextDirection.Coordinates;

                Guard.ThrowIfCoordinateInvalidForPlateau(nextCoordinate, _plateau);
            }

            _coordinates = nextCoordinate;
            _direction = nextDirection;
        }

        public void TeleportToPosition(string position)
        {
            Guard.ThrowIfNull(position);

            (Coordinates coordinates, Direction direction) = PositionStringConverter.ToCoordinatesDirection(position);

            Guard.ThrowIfCoordinateInvalidForPlateau(coordinates, _plateau);

            _coordinates = coordinates;
            _direction = direction;
        } 
    }
}
