using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        public Position Position { get; private set; }
        public PlateauBase Plateau { get; private set; }

        public VehicleBase(Position initialPosition, PlateauBase plateau)
        {
            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            if (!plateau.IsCoordinateValidInPlateau(initialPosition.Coordinates))
                throw new ArgumentException($"{nameof(initialPosition)} {initialPosition.Coordinates} is not valid in plateau", nameof(initialPosition));

            Position = initialPosition;
            Plateau = plateau;
        }

        public void ApplyMoveInstruction(List<SingularInstruction> instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

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

                if (!Plateau.IsCoordinateValidInPlateau(nextCoordinates))
                    return;

                Position = new Position(nextCoordinates, nextDirection);
            }
        }
    }
}