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

        public string Position => $"{_coordinates.X} {_coordinates.Y} {_direction.Char}";

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

                if (!_plateau.IsCoordinateValid(nextCoordinate))
                    throw new ArgumentException($"Instruction will lead to invalid coordinate {nextCoordinate}", nameof(instruction));
            }

            _coordinates = nextCoordinate;
            _direction = nextDirection;
        }

        public void TeleportToPosition(string position)
        {
            if (position is null)
                throw new ArgumentNullException("position cannot be null", nameof(position));

            (Coordinates coordinates, Direction direction) = PositionUtilities.GetCoordinatesDirectionFromPosition(position);
            if (!_plateau.IsCoordinateValid(coordinates))
                throw new ArgumentException("initial position cannot be outside of plateau", nameof(position));

            _coordinates = coordinates;
            _direction = direction;
        } 
    }
}
