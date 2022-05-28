using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public class Rover
    {
        private Coordinates _coordinates;
        private Direction _direction;
        private PlateauBase _plateau;
        private IPositionStringConverter _positionStringConverter;
        private IInstructionReader _instructionReader;

        public Rover(string initialPosition, PlateauBase plateau, IPositionStringConverter positionStringConverter)
        {
            _positionStringConverter = positionStringConverter;
            _instructionReader = new StandardInstructionReader();

            if (initialPosition is null)
                throw new ArgumentNullException(nameof(initialPosition));

            if (plateau is null)
                throw new ArgumentNullException(nameof(plateau));

            if (positionStringConverter is null)
                throw new ArgumentNullException(nameof(positionStringConverter));


            if (!_positionStringConverter.IsValidPositionString(initialPosition))
                throw new ArgumentException($"Input {initialPosition} is not in correct format for {nameof(initialPosition)} (eg \"1 2 N\")", nameof(initialPosition));

            (Coordinates coordinates, Direction direction) = _positionStringConverter.ToCoordinatesDirection(initialPosition);

            if (!plateau.IsCoordinateValidInPlateau(coordinates))
                throw new ArgumentException($"initialPosition {initialPosition} is not on a valid coordinate of plateau", nameof(initialPosition));

            (_coordinates, _direction) = _positionStringConverter.ToCoordinatesDirection(initialPosition);
            _plateau = plateau;
        }

        public Rover(string initialPosition, PlateauBase plateau)
            : this(initialPosition, plateau, new PositionStringConverter())
        {
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

        public void ApplyMoveInstruction(string instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction));

            if (!_instructionReader.IsValidInstruction(instruction))
                throw new ArgumentException($"Instruction {instruction} is not in correct format", nameof(instruction));

            List<SingularInstruction> instructionItems = _instructionReader.EvaluateInstruction(instruction);
            Coordinates newCoordinates = new(_coordinates.X, _coordinates.Y);

            foreach (SingularInstruction singularInstruction in instructionItems)
            {

            }
        }
    }
}
