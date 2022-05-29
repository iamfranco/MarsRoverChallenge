using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        private Coordinates _coordinates;
        private Direction _direction;
        public PlateauBase Plateau { get; private set; }

        public VehicleBase(Coordinates initialCoordinates, Direction initialDirection, PlateauBase plateau)
        {
            if (initialDirection is null)
                throw new ArgumentNullException(nameof(initialDirection));

            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            if (!plateau.IsCoordinateValidInPlateau(initialCoordinates))
                throw new ArgumentException($"{nameof(initialCoordinates)} {initialCoordinates} is not valid in plateau", nameof(initialCoordinates));

            _coordinates = initialCoordinates;
            _direction = initialDirection;
            Plateau = plateau;
        }

        public Coordinates Coordinates => _coordinates;
        public Direction Direction => _direction;

        public void ApplyMoveInstruction(List<SingularInstruction> instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));


            foreach (SingularInstruction singularInstruction in instruction)
            {
                Coordinates nextCoordinates = _coordinates;
                Direction nextDirection = _direction.Clone();

                if (singularInstruction is SingularInstruction.TurnLeft)
                    nextDirection.TurnLeft();

                if (singularInstruction is SingularInstruction.TurnRight)
                    nextDirection.TurnRight();

                if (singularInstruction is SingularInstruction.MoveForward)
                    nextCoordinates += nextDirection.MovementVector;

                TeleportToPosition(nextCoordinates, nextDirection);
            }
        }

        public void TeleportToPosition(Coordinates coordinates, Direction direction)
        {
            if (direction is null)
                throw new ArgumentNullException(nameof(direction));

            if (!Plateau.IsCoordinateValidInPlateau(coordinates))
                return;

            _coordinates = coordinates;
            _direction = direction;
        }
    }
}