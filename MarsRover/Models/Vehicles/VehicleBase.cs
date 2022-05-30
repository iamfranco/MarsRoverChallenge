using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        private Coordinates _coordinates;
        private DirectionEnum _direction;
        public PlateauBase Plateau { get; private set; }

        public VehicleBase(Coordinates initialCoordinates, DirectionEnum initialDirection, PlateauBase plateau)
        {
            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            if (!plateau.IsCoordinateValidInPlateau(initialCoordinates))
                throw new ArgumentException($"{nameof(initialCoordinates)} {initialCoordinates} is not valid in plateau", nameof(initialCoordinates));

            _coordinates = initialCoordinates;
            _direction = initialDirection;
            Plateau = plateau;
        }

        public Coordinates Coordinates => _coordinates;
        public DirectionEnum Direction => _direction;

        public void ApplyMoveInstruction(List<SingularInstruction> instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

            foreach (SingularInstruction singularInstruction in instruction)
            {
                Coordinates nextCoordinates = _coordinates;
                DirectionEnum nextDirection = _direction;

                if (singularInstruction is SingularInstruction.TurnLeft)
                    nextDirection = nextDirection.GetLeftTurn();

                if (singularInstruction is SingularInstruction.TurnRight)
                    nextDirection = nextDirection.GetRightTurn();

                if (singularInstruction is SingularInstruction.MoveForward)
                    nextCoordinates += nextDirection.GetMovementVector();

                if (!Plateau.IsCoordinateValidInPlateau(nextCoordinates))
                    return;

                _coordinates = nextCoordinates;
                _direction = nextDirection;
            }
        }
    }
}