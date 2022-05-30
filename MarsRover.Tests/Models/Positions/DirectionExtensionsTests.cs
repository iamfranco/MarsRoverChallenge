using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Positions
{
    internal class DirectionExtensionsTests
    {
        [Test]
        public void GetMovementVector_Should_Return_Correct_Movement_Vector()
        {
            DirectionEnum north = DirectionEnum.North;
            north.GetMovementVector().Should().Be(new Coordinates(0, 1));

            DirectionEnum south = DirectionEnum.South;
            south.GetMovementVector().Should().Be(new Coordinates(0, -1));

            DirectionEnum east = DirectionEnum.East;
            east.GetMovementVector().Should().Be(new Coordinates(1, 0));

            DirectionEnum west = DirectionEnum.West;
            west.GetMovementVector().Should().Be(new Coordinates(-1, 0));
        }

        [Test]
        public void GetLeftTurn_Should_Modify_Direction_To_CounterClockwise_Direction()
        {
            DirectionEnum direction = DirectionEnum.North;

            direction = direction.GetLeftTurn();
            direction.Should().Be(DirectionEnum.West);

            direction = direction.GetLeftTurn();
            direction.Should().Be(DirectionEnum.South);

            direction = direction.GetLeftTurn();
            direction.Should().Be(DirectionEnum.East);

            direction = direction.GetLeftTurn();
            direction.Should().Be(DirectionEnum.North);
        }

        [Test]
        public void GetRightTurn_Should_Modify_Direction_To_CounterClockwise_Direction()
        {
            DirectionEnum direction = DirectionEnum.North;

            direction = direction.GetRightTurn();
            direction.Should().Be(DirectionEnum.East);

            direction = direction.GetRightTurn();
            direction.Should().Be(DirectionEnum.South);

            direction = direction.GetRightTurn();
            direction.Should().Be(DirectionEnum.West);

            direction = direction.GetRightTurn();
            direction.Should().Be(DirectionEnum.North);
        }
    }
}
