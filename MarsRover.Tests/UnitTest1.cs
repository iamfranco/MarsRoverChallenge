using FluentAssertions;
using MarsRover.Models.MovementInstructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RectangularPlateau_MaximumCoordinates_Should_Match_Constructor_Input_Coordinates()
        {
            Coordinates expectedCoordinates = new(2, 3);
            RectangularPlateau plateau = new(expectedCoordinates);

            plateau.MaximumCoordinates.Should().Be(expectedCoordinates);
        }

        [Test]
        public void RectangularPlateau_Construct_With_Negative_X_Component_Coordinates_Should_Throw_Exception()
        {
            Coordinates invalidCoordinate = new(-2, 3);
            IPlateau plateau;

            Action act = () => plateau = new RectangularPlateau(invalidCoordinate);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void RectangularPlateau_Construct_With_Negative_Y_Component_Coordinates_Should_Throw_Exception()
        {
            Coordinates invalidCoordinate = new(2, -3);
            IPlateau plateau;

            Action act = () => plateau = new RectangularPlateau(invalidCoordinate);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void RectangularPlateau_ObstaclesCoordinates_Should_Return_Coordinates_Of_Specified_Obstacles()
        {
            Coordinates plateauSize = new(10, 10);
            List<Coordinates> obstacles = new()
            {
                new (1, 2),
                new (2, 4),
                new (4, 10)
            };

            RectangularPlateau plateau = new RectangularPlateau(plateauSize, obstacles);

            plateau.ObstaclesCoordinates.Count.Should().Be(obstacles.Count);
            for (int i = 0; i < obstacles.Count; i++)
            {
                plateau.ObstaclesCoordinates[i].Should().Be(obstacles[i]);
            }
        }

        [Test]
        public void RectangularPlateau_ObstaclesCoordinates_Should_Not_Include_Coordinates_Outside_Of_Plateau()
        {
            List<Coordinates> obstacles = new()
            {
                new (101, 2),
                new (5, 5),
                new (102, 4),
                new (2, 4),
                new (4, 100)
            };

            RectangularPlateau plateau = new RectangularPlateau(new(10, 10), obstacles);
            plateau.ObstaclesCoordinates.Count.Should().Be(2);
            plateau.ObstaclesCoordinates[0].Should().Be(new Coordinates(5, 5));
            plateau.ObstaclesCoordinates[1].Should().Be(new Coordinates(2, 4));

            RectangularPlateau plateau2 = new RectangularPlateau(new(3, 3), obstacles);
            plateau2.ObstaclesCoordinates.Count.Should().Be(0);
        }

        [Test]
        public void Rover_GetPosition_Should_Return_Initial_Position()
        {
            string initialPosition = "2 3 N";
            IPlateau plateau = new RectangularPlateau(new Coordinates(5, 5));
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new(initialPosition, plateau, instructionReader);

            rover.Position.Should().Be(initialPosition);
        }

        [Test]
        public void Rover_GetPosition_After_MovementInstructions_Should_Return_Correct_Position()
        {
            Coordinates plateauSize = new(5, 5);
            IPlateau plateau = new RectangularPlateau(plateauSize);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new("1 2 N", plateau, instructionReader);

            rover.ApplyMoveInstruction("LMLMLMLMM");
            rover.Position.Should().Be("1 3 N");

            rover.TeleportToPosition("3 3 E");
            rover.ApplyMoveInstruction("MMRMMRMRRM");
            rover.Position.Should().Be("5 1 E");
        }

        [Test]
        public void Rover_GetPosition_After_Empty_String_MovementInstructions_Should_Return_Initial_Position()
        {
            Coordinates plateauSize = new(5, 5);
            string initialPosition = "3 3 E";
            string instruction = "";
            IPlateau plateau = new RectangularPlateau(plateauSize);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new(initialPosition, plateau, instructionReader);

            rover.ApplyMoveInstruction(instruction);

            rover.Position.Should().Be(initialPosition);
        }

        [Test]
        public void Rover_GetPosition_After_MovementInstructions_With_Whitespaces_Should_Return_Correct_Position()
        {
            Coordinates plateauSize = new(5, 5);
            string initialPosition = "3 3 E";
            string instruction = "MM R MM R M RR M";
            string expectedPosition = "5 1 E";
            IPlateau plateau = new RectangularPlateau(plateauSize);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new(initialPosition, plateau, instructionReader);

            rover.ApplyMoveInstruction(instruction);

            rover.Position.Should().Be(expectedPosition);
        }

        [Test]
        public void Rover_ApplyMoveInstruction_With_Invalid_Instruction_String_Should_Throw_Exception()
        {
            Coordinates plateauSize = new(5, 5);
            string initialPosition = "3 3 E";
            string instruction = "MM R MM R M RR M? !";
            IPlateau plateau = new RectangularPlateau(plateauSize);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new(initialPosition, plateau, instructionReader);

            Action act = () => rover.ApplyMoveInstruction(instruction);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Rover_ApplyMoveInstruction_With_Instruction_String_That_Moves_Past_Obstacle_Should_Throw_Exception()
        {
            Coordinates plateauSize = new(5, 5);
            string initialPosition = "0 0 E";
            string instruction = "LMMMR";

            List<Coordinates> obstacles = new() { new(0, 1) };
            IPlateau plateau = new RectangularPlateau(plateauSize, obstacles);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new(initialPosition, plateau, instructionReader);

            Action act = () => rover.ApplyMoveInstruction(instruction);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Rover_ApplyMoveInstruction_With_Null_String_Instruction_Should_Throw_Exception()
        {
            Coordinates plateauSize = new(5, 5);
            string initialPosition = "0 0 E";
            string instruction = null;

            IPlateau plateau = new RectangularPlateau(plateauSize);
            IInstructionReader instructionReader = new StandardInstructionReader();
            Rover rover = new(initialPosition, plateau, instructionReader);

            Action act = () => rover.ApplyMoveInstruction(instruction);
            act.Should().Throw<ArgumentException>();
        }
    }
}