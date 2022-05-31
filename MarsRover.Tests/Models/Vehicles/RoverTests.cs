using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Models.Vehicles
{
    internal class RoverTests
    {
        readonly List<List<SingularInstruction>> validInstructions = new()
        {
            new() { },
            new()
            {
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.TurnLeft
            },
            new()
            {
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward
            }
        };
        readonly List<Coordinates> obstacles = new() { new(2, 3), new(5, 5) };

        PlateauBase plateau;
        PlateauBase plateauWithObstacles;

        Rover roverOnPlateau;

        [SetUp]
        public void Setup()
        {
            plateau = new RectangularPlateau(new(5, 5));
            plateauWithObstacles = new RectangularPlateau(new(5, 5));
            foreach (Coordinates obstacle in obstacles)
            {
                plateauWithObstacles.AddObstacle(obstacle);
            }

            roverOnPlateau = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        }

        [Test]
        public void Position_After_Successful_Construction_Should_Return_InitialPosition()
        {
            Position initialPosition = new(new(1, 2), Direction.North);
            Rover rover = new Rover(initialPosition);

            rover.Position.Should().Be(initialPosition);
        }

        [Test]
        public void ApplyMoveInstruction_With_Null_Instruction_Should_Throw_Exception()
        {
            Action act;

            act = () => roverOnPlateau.ApplyMoveInstruction(null, plateau);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ApplyMoveInstruction_With_Null_Plateau_Should_Throw_Exception()
        {
            Action act;

            act = () => roverOnPlateau.ApplyMoveInstruction(validInstructions[0], null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ApplyMoveInstruction_With_Valid_Instruction_String_Should_Succeed()
        {
            Action act;

            foreach (List<SingularInstruction> validInstruction in validInstructions)
            {
                act = () => roverOnPlateau.ApplyMoveInstruction(validInstruction, plateau);
                act.Should().NotThrow();
            }
        }

        [Test]
        public void Position_After_ApplyMoveInstruction_Should_Return_Correct_New_Position()
        {
            Rover rover;

            rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
            rover.ApplyMoveInstruction(new()
            {
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnLeft,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward
            }, plateau);
            rover.Position.Should().Be(new Position(new(1, 3), Direction.North));

            rover = new Rover(new Position(new Coordinates(3, 3), Direction.East));
            rover.ApplyMoveInstruction(new()
            {
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.TurnRight,
                SingularInstruction.MoveForward
            }, plateau);
            rover.Position.Should().Be(new Position(new(5, 1), Direction.East));
        }

        [Test]
        public void ApplymoveInstruction_Into_Invalid_Coordinate_For_Plateau_Should_Have_Position_Stop_Before_Invalid_Coordinate()
        {
            PlateauBase plateauWithOneObstacle = new RectangularPlateau(new(5, 5));
            plateauWithOneObstacle.AddObstacle(new(2, 3));

            Rover rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));

            List<SingularInstruction> instruction = new()
            {
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
            };

            rover.ApplyMoveInstruction(instruction, plateauWithObstacles);

            rover.Position.Should().Be(new Position(new(1, 3), Direction.East));
        }

        [Test]
        public void TakePhotoAndSendToStation_Should_Not_Throw_Exception()
        {
            Rover rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
            Action act;

            act = () => rover.TakePhotoAndSendToStation();
            act.Should().NotThrow();
        }

        [Test]
        public void CollectSample_Should_Not_Throw_Exception()
        {
            Rover rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
            Action act;

            act = () => rover.CollectSample();
            act.Should().NotThrow();
        }
    }
}
