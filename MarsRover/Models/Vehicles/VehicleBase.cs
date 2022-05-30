using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        public Coordinates Coordinates { get; private set; }
        public Direction Direction { get; private set; }
        public PlateauBase Plateau { get; private set; }

        public VehicleBase(Coordinates initialCoordinates, Direction initialDirection, PlateauBase plateau)
        {
            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            if (!plateau.IsCoordinateValidInPlateau(initialCoordinates))
                throw new ArgumentException($"{nameof(initialCoordinates)} {initialCoordinates} is not valid in plateau", nameof(initialCoordinates));

            Coordinates = initialCoordinates;
            Direction = initialDirection;
            Plateau = plateau;
        }

        public void ApplyMoveInstruction(List<SingularInstruction> instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

            foreach (SingularInstruction singularInstruction in instruction)
            {
                Coordinates nextCoordinates = Coordinates;
                Direction nextDirection = Direction;

                if (singularInstruction is SingularInstruction.TurnLeft)
                    nextDirection = nextDirection.GetLeftTurn();

                if (singularInstruction is SingularInstruction.TurnRight)
                    nextDirection = nextDirection.GetRightTurn();

                if (singularInstruction is SingularInstruction.MoveForward)
                    nextCoordinates += nextDirection.GetMovementVector();

                if (!Plateau.IsCoordinateValidInPlateau(nextCoordinates))
                    return;

                Coordinates = nextCoordinates;
                Direction = nextDirection;
            }
        }
    }
}