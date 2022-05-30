using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Positions
{
    internal class PositionTests
    {
        [Test]
        public void Constructor_With_Coordinates_And_Direction_Should_Succeed()
        {
            Position position;
            Action act;

            act = () => position = new Position(new Coordinates(1, 2), Direction.North);
            act.Should().NotThrow();

            act = () => position = new Position(new Coordinates(-5, 3), Direction.West);
            act.Should().NotThrow();

            act = () => position = new Position(new Coordinates(4, -1), Direction.North);
            act.Should().NotThrow();

            act = () => position = new Position(new Coordinates(0, 0), Direction.South);
            act.Should().NotThrow();
        }

        [Test]
        public void Coordinates_Should_Return_Constructor_Input_Coordinates()
        {
            Coordinates coordinates = new Coordinates(1, 2);
            Position position = new Position(coordinates, Direction.North);

            position.Coordinates.Should().Be(coordinates);
        }

        [Test]
        public void Direction_Should_Return_Constructor_Input_Direction()
        {
            Direction direction = Direction.North;
            Position position = new Position(new Coordinates(1, 2), direction);

            position.Direction.Should().Be(direction);
        }

        [Test]
        public void Equals_Of_Two_Instances_With_Different_Value_Should_Be_True()
        {
            Position positionA = new Position(new Coordinates(1, 2), Direction.North);
            Position positionB = new Position(new Coordinates(1, 2), Direction.North);

            positionA.Equals(positionB).Should().Be(true);
        }

        [Test]
        public void Equals_Of_Two_Instances_With_Different_Value_Should_Be_False()
        {
            Position positionA = new Position(new Coordinates(1, 2), Direction.North);
            Position positionB = new Position(new Coordinates(5, -1), Direction.East);

            positionA.Equals(positionB).Should().Be(false);
        }
    }
}
