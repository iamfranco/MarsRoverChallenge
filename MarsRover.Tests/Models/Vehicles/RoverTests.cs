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

            roverOnPlateau = new Rover(new Coordinates(1, 2), Direction.North, plateau);
        }

        [Test]
        public void Constructor_With_Null_Plateau_Input_Should_Throw_Exception()
        {
            Rover rover;
            Coordinates initialCoordinates = new(1, 2);
            Direction initialDirection = Direction.North;
            PlateauBase plat = null;

            Action act = () => rover = new Rover(initialCoordinates, initialDirection, plat);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Constructor_With_InitialPosition_Invalid_On_Plateau_Should_Throw_Exception()
        {
            Rover rover;
            Coordinates initialCoordinates = obstacles[0];
            Direction initialDirection = Direction.North;

            Action act = () => rover = new Rover(initialCoordinates, initialDirection, plateauWithObstacles);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Coordinates_After_Successful_Construction_Should_Return_InitialPosition()
        {
            Coordinates coordinates = new(1, 2);
            Rover rover = new Rover(coordinates, Direction.North, plateau);

            rover.Coordinates.Should().Be(coordinates);
        }

        [Test]
        public void Direction_After_Successful_Construction_Should_Return_InitialPosition()
        {
            Direction direction = Direction.North;
            Rover rover = new Rover(new(1, 2), direction, plateau);

            rover.Direction.Should().Be(direction);
        }

        [Test]
        public void ApplyMoveInstruction_With_Null_Instruction_Should_Throw_Exception()
        {
            Action act;

            act = () => roverOnPlateau.ApplyMoveInstruction(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ApplyMoveInstruction_With_Valid_Instruction_String_Should_Succeed()
        {
            Action act;

            foreach (List<SingularInstruction> validInstruction in validInstructions)
            {
                act = () => roverOnPlateau.ApplyMoveInstruction(validInstruction);
                act.Should().NotThrow();
            }
        }

        [Test]
        public void GetPosition_After_ApplyMoveInstruction_Should_Return_Correct_New_Position()
        {
            Rover rover;

            rover = new Rover(new Coordinates(1, 2), Direction.North, plateau);
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
            });
            rover.Coordinates.Should().Be(new Coordinates(1, 3));
            rover.Direction.Should().Be(Direction.North);

            rover = new Rover(new Coordinates(3, 3), Direction.East, plateau);
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
            });
            rover.Coordinates.Should().Be(new Coordinates(5, 1));
            rover.Direction.Should().Be(Direction.East);
        }

        [Test]
        public void ApplymoveInstruction_Into_Invalid_Coordinate_For_Plateau_Should_Have_Position_Stop_Before_Invalid_Coordinate()
        {
            PlateauBase plateauWithOneObstacle = new RectangularPlateau(new(5, 5));
            plateauWithOneObstacle.AddObstacle(new(2, 3));

            Rover rover = new Rover(new Coordinates(1, 2), Direction.North, plateauWithObstacles);

            List<SingularInstruction> instruction = new()
            {
                SingularInstruction.MoveForward,
                SingularInstruction.TurnRight,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
                SingularInstruction.MoveForward,
            };

            rover.ApplyMoveInstruction(instruction);

            rover.Coordinates.Should().Be(new Coordinates(1, 3));
            rover.Direction.Should().Be(Direction.East);
        }

        [Test]
        public void TakePhotoAndSendToStation_Should_Not_Throw_Exception()
        {
            Rover rover = new Rover(new Coordinates(1, 2), Direction.North, plateau);
            Action act;

            act = () => rover.TakePhotoAndSendToStation();
            act.Should().NotThrow();
        }

        [Test]
        public void CollectSample_Should_Not_Throw_Exception()
        {
            Rover rover = new Rover(new Coordinates(1, 2), Direction.North, plateau);
            Action act;

            act = () => rover.CollectSample();
            act.Should().NotThrow();
        }
    }
}
