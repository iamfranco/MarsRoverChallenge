using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        public Position Position { get; private set; }

        public VehicleBase(Position initialPosition)
        {
            Position = initialPosition;
        }

        public void ApplyMoveInstruction(List<SingularInstruction> instruction, PlateauBase plateau)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            foreach (SingularInstruction singularInstruction in instruction)
            {
                Coordinates nextCoordinates = Position.Coordinates;
                Direction nextDirection = Position.Direction;

                if (singularInstruction is SingularInstruction.TurnLeft)
                    nextDirection = nextDirection.GetLeftTurn();

                if (singularInstruction is SingularInstruction.TurnRight)
                    nextDirection = nextDirection.GetRightTurn();

                if (singularInstruction is SingularInstruction.MoveForward)
                    nextCoordinates += nextDirection.GetMovementVector();

                if (!plateau.IsCoordinateValidInPlateau(nextCoordinates))
                    return;

                Position = new Position(nextCoordinates, nextDirection);
            }
        }
    }
}