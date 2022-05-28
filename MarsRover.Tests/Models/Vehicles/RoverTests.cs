using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Models.Vehicles
{
    internal class RoverTests
    {
        readonly List<string> invalidInstructions = new() { "asjdkfl", "lmr", "LM!" };
        readonly List<string> validInstructions = new() { "", "LMRL", "L M R L", "L M LL RR M R" };
        readonly List<Coordinates> obstacles = new() { new(2, 3), new(5, 5) };

        PlateauBase plateau;
        PlateauBase plateauWithObstacles;

        [SetUp]
        public void Setup()
        {
            plateau = new RectangularPlateau(new(5, 5));
            plateauWithObstacles = new RectangularPlateau(new(5, 5));
            foreach (Coordinates obstacle in obstacles)
            {
                plateauWithObstacles.AddObstacle(obstacle);
            }
        }

        [Test]
        public void Constructor_With_Null_String_Input_Should_Throw_Exception()
        {
            Rover rover;
            Action act = () => rover = new Rover(null, plateau);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Constructor_With_Null_Plateau_Input_Should_Throw_Exception()
        {
            Rover rover;
            Action act = () => rover = new Rover("1 2 N", null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Constructor_With_Invalidly_Formatted_InitialPosition_String_Should_Throw_Exception()
        {
            Rover rover;
            Action act;

            act = () => rover = new Rover("123", plateau);
            act.Should().Throw<ArgumentException>();

            act = () => rover = new Rover("", plateau);
            act.Should().Throw<ArgumentException>();

            act = () => rover = new Rover("1 1 Z", plateau);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_With_InitialPosition_Outside_Of_Plateau_Should_Throw_Exception()
        {
            Rover rover;
            Action act;

            act = () => rover = new Rover("-10 0 W", plateau);
            act.Should().Throw<ArgumentException>();

            act = () => rover = new Rover("2 -5 N", plateau);
            act.Should().Throw<ArgumentException>();

            act = () => rover = new Rover("2 50 E", plateau);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_With_InitialPosition_On_Obstacle_Of_Plateau_Should_Throw_Exception()
        {
            Rover rover;
            Action act;

            act = () => rover = new Rover("2 3 W", plateauWithObstacles);
            act.Should().Throw<ArgumentException>();

            act = () => rover = new Rover("5 5 N", plateauWithObstacles);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_With_Valid_InitialPosition_Should_Succeed()
        {
            Rover rover;
            Action act;

            act = () => rover = new Rover("3 2 S", plateau);
            act.Should().NotThrow();

            act = () => rover = new Rover("1 1 W", plateau);
            act.Should().NotThrow();
        }

        [Test]
        public void Constructor_With_Explicit_PositionStringConverter_Should_Succeed()
        {
            Rover rover;
            IPositionStringConverter positionStringConverter = new PositionStringConverter();
            Action act;

            act = () => rover = new Rover("3 2 S", plateau, positionStringConverter);
            act.Should().NotThrow();
        }

        [Test]
        public void Constructor_With_Null_PositionStringConverter_Should_Throw_Exception()
        {
            Rover rover;
            IPositionStringConverter positionStringConverter = null;
            Action act;

            act = () => rover = new Rover("3 2 S", plateau, positionStringConverter);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GetPosition_After_Successful_Construction_Should_Return_InitialPosition()
        {
            string initialPosition = "1 2 N";
            Rover rover = new Rover(initialPosition, plateau);

            rover.GetPosition().Should().Be(initialPosition);
        }

        [Test]
        public void SwapInstructionReader_With_Null_Input_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            IInstructionReader instructionReader = null;
            Action act;

            act = () => rover.SwapInstructionReader(instructionReader);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SwapInstructionReader_With_Valid_InstructionReader_Should_Succeed()
        {
            Rover rover = new Rover("1 2 N", plateau);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Action act;

            act = () => rover.SwapInstructionReader(instructionReader);
            act.Should().NotThrow();
        }

        [Test]
        public void SwapPositionStringConverter_With_Null_Input_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            IPositionStringConverter positionStringConverter = null;
            Action act;

            act = () => rover.SwapPositionStringConverter(positionStringConverter);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SwapPositionStringConverter_With_Valid_PositionStringConverter_Should_Succeed()
        {
            Rover rover = new Rover("1 2 N", plateau);
            IPositionStringConverter positionStringConverter = new PositionStringConverter();
            Action act;

            act = () => rover.SwapPositionStringConverter(positionStringConverter);
            act.Should().NotThrow();
        }

        [Test]
        public void ApplyMoveInstruction_With_Null_Instruction_String_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            act = () => rover.ApplyMoveInstruction(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void ApplyMoveInstruction_With_Invalid_Instruction_String_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            foreach (string invalidInstruction in invalidInstructions)
            {
                act = () => rover.ApplyMoveInstruction(invalidInstruction);
                act.Should().Throw<ArgumentException>();
            }
        }

        [Test]
        public void ApplyMoveInstruction_With_Valid_Instruction_String_Should_Succeed()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            foreach (string validInstruction in validInstructions)
            {
                act = () => rover.ApplyMoveInstruction(validInstruction);
                act.Should().NotThrow();
            }
        }

        [Test]
        public void GetPosition_After_ApplyMoveInstruction_Should_Return_Correct_New_Position()
        {
            Rover rover;

            rover = new Rover("1 2 N", plateau);
            rover.ApplyMoveInstruction("LMLMLMLMM");
            rover.GetPosition().Should().Be("1 3 N");

            rover = new Rover("3 3 E", plateau);
            rover.ApplyMoveInstruction("MMRMMRMRRM");
            rover.GetPosition().Should().Be("5 1 E");
        }

        [Test]
        public void ApplyMoveInstruction_With_Instruction_That_Moves_Outside_Of_Plateau_Should_Throw_Exception_And_Not_Modify_Position()
        {
            Rover rover;
            Action act;

            rover = new Rover("1 2 N", plateau);
            act = () => rover.ApplyMoveInstruction("MMMMMMMM");
            act.Should().Throw<ArgumentException>();
            rover.GetPosition().Should().Be("1 2 N");

            rover = new Rover("3 3 E", plateau);
            act = () => rover.ApplyMoveInstruction("LLMMMMM");
            act.Should().Throw<ArgumentException>();
            rover.GetPosition().Should().Be("3 3 E");
        }

        [Test]
        public void ApplyMoveInstruction_With_Instruction_That_Moves_Into_Obstacle_On_Plateau_Should_Throw_Exception_And_Not_Modify_Position()
        {
            Rover rover;
            Action act;

            rover = new Rover("1 2 N", plateauWithObstacles);
            act = () => rover.ApplyMoveInstruction("MRM LLLRLRLM");
            act.Should().Throw<ArgumentException>();
            rover.GetPosition().Should().Be("1 2 N");

            rover = new Rover("3 3 E", plateauWithObstacles);
            act = () => rover.ApplyMoveInstruction("LMMRMM LRLRLM");
            act.Should().Throw<ArgumentException>();
            rover.GetPosition().Should().Be("3 3 E");
        }

        [Test]
        public void TeleportToPosition_With_Null_String_Input_Should_Throw_Exception()
        {
            Rover rover;
            Action act;
            rover = new Rover("1 2 N", plateau);

            string newPosition = null;
            act = () => rover.TeleportToPosition(newPosition);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeleportToPosition_With_Invalidly_Formatted_Position_String_Input_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            act = () => rover.TeleportToPosition("123");
            act.Should().Throw<ArgumentException>();

            act = () => rover.TeleportToPosition("");
            act.Should().Throw<ArgumentException>();

            act = () => rover.TeleportToPosition("1 1 Z");
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void TeleportToPosition_With_InitialPosition_Outside_Of_Plateau_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            act = () => rover.TeleportToPosition("-10 0 W");
            act.Should().Throw<ArgumentException>();

            act = () => rover.TeleportToPosition("2 -5 N");
            act.Should().Throw<ArgumentException>();

            act = () => rover.TeleportToPosition("2 50 E");
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void TeleportToPosition_With_InitialPosition_On_Obstacle_Of_Plateau_Should_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateauWithObstacles);
            Action act;

            act = () => rover.TeleportToPosition("2 3 W");
            act.Should().Throw<ArgumentException>();

            act = () => rover.TeleportToPosition("5 5 N");
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void TeleportToPosition_With_Valid_InitialPosition_Should_Succeed_And_Updates_Position()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            act = () => rover.TeleportToPosition("3 2 S");
            act.Should().NotThrow();
            rover.GetPosition().Should().Be("3 2 S");

            act = () => rover.TeleportToPosition("1 1 W");
            act.Should().NotThrow();
            rover.GetPosition().Should().Be("1 1 W");
        }

        [Test]
        public void TakePhotoAndSendToStation_Should_Not_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            act = () => rover.TakePhotoAndSendToStation();
            act.Should().NotThrow();
        }

        [Test]
        public void CollectSample_Should_Not_Throw_Exception()
        {
            Rover rover = new Rover("1 2 N", plateau);
            Action act;

            act = () => rover.CollectSample();
            act.Should().NotThrow();
        }
    }
}
