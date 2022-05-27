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
        private readonly IPositionStringConverter _positionStringConverter;

        public VehicleBase(string initialPosition, IPlateau plateau, 
            IInstructionReader instructionReader, IPositionStringConverter positionStringConverter)
        {
            _plateau = plateau;
            _instructionReader = instructionReader;
            _positionStringConverter = positionStringConverter;

            TeleportToPosition(initialPosition);
        }

        public string Position => _positionStringConverter.ToPositionString(_coordinates, _direction);

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

            (Coordinates coordinates, Direction direction) = _positionStringConverter.ToCoordinatesDirection(position);

            Guard.ThrowIfCoordinateInvalidForPlateau(coordinates, _plateau);

            _coordinates = coordinates;
            _direction = direction;
        } 
    }
}
