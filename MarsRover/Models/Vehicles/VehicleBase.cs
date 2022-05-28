using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public abstract class VehicleBase
    {
        private Coordinates _coordinates;
        private Direction _direction;
        private PlateauBase _plateau;
        private IPositionStringConverter _positionStringConverter;
        private IInstructionReader _instructionReader;

        public VehicleBase(string initialPosition, PlateauBase plateau, IPositionStringConverter positionStringConverter)
        {
            if (positionStringConverter is null)
                throw new ArgumentNullException(nameof(positionStringConverter));

            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            _plateau = plateau;
            _positionStringConverter = positionStringConverter;
            _instructionReader = new StandardInstructionReader();

            TeleportToPosition(initialPosition, out _coordinates, out _direction);
        }

        public VehicleBase(string initialPosition, PlateauBase plateau)
            : this(initialPosition, plateau, new PositionStringConverter())
        {
        }

        public void ApplyMoveInstruction(string instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

            if (!_instructionReader.IsValidInstruction(instruction))
                throw new ArgumentException($"Instruction {instruction} is not in correct format", nameof(instruction));

            List<SingularInstruction> instructionItems = _instructionReader.EvaluateInstruction(instruction);

            Coordinates newCoordinates = _coordinates;
            Direction newDirection = _direction.Clone();

            foreach (SingularInstruction singularInstruction in instructionItems)
            {
                if (singularInstruction is SingularInstruction.TurnLeft)
                    newDirection.TurnLeft();

                if (singularInstruction is SingularInstruction.TurnRight)
                    newDirection.TurnRight();

                if (singularInstruction is SingularInstruction.MoveForward)
                    newCoordinates += newDirection.MovementVector;

                if (!_plateau.IsCoordinateValidInPlateau(newCoordinates))
                    throw new ArgumentException($"Instruction {instruction} leads to invalid coordinate {newCoordinates}", nameof(instruction));
            }

            _coordinates = newCoordinates;
            _direction = newDirection;
        }

        public string GetPosition() => _positionStringConverter.ToPositionString(_coordinates, _direction);

        public void SwapInstructionReader(IInstructionReader instructionReader)
        {
            if (instructionReader is null)
                throw new ArgumentNullException(nameof(instructionReader));

            _instructionReader = instructionReader;
        }

        public void SwapPositionStringConverter(IPositionStringConverter positionStringConverter)
        {
            if (positionStringConverter is null)
                throw new ArgumentNullException(nameof(positionStringConverter));

            _positionStringConverter = positionStringConverter;
        }

        public void TeleportToPosition(string position, out Coordinates coordinates, out Direction direction)
        {
            if (position is null)
                throw new ArgumentNullException(nameof(position));

            if (!_positionStringConverter.IsValidPositionString(position))
                throw new ArgumentException($"Input {position} is not in correct format for {nameof(position)} (eg \"1 2 N\")", nameof(position));

            (coordinates, direction) = _positionStringConverter.ToCoordinatesDirection(position);

            if (!_plateau.IsCoordinateValidInPlateau(coordinates))
                throw new ArgumentException($"Position {position} is not on a valid coordinate of plateau", nameof(position));
        }
        public void TeleportToPosition(string position) => TeleportToPosition(position, out _coordinates, out _direction);
    }
}