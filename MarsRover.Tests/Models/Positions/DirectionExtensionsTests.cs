using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Positions;

internal class DirectionExtensionsTests
{
    [Test]
    public void GetMovementVector_Should_Return_Correct_Movement_Vector()
    {
        Direction north = Direction.North;
        north.GetMovementVector().Should().Be(new Coordinates(0, 1));

        Direction south = Direction.South;
        south.GetMovementVector().Should().Be(new Coordinates(0, -1));

        Direction east = Direction.East;
        east.GetMovementVector().Should().Be(new Coordinates(1, 0));

        Direction west = Direction.West;
        west.GetMovementVector().Should().Be(new Coordinates(-1, 0));
    }

    [Test]
    public void GetLeftTurn_Should_Modify_Direction_To_CounterClockwise_Direction()
    {
        Direction direction = Direction.North;

        direction = direction.GetLeftTurn();
        direction.Should().Be(Direction.West);

        direction = direction.GetLeftTurn();
        direction.Should().Be(Direction.South);

        direction = direction.GetLeftTurn();
        direction.Should().Be(Direction.East);

        direction = direction.GetLeftTurn();
        direction.Should().Be(Direction.North);
    }

    [Test]
    public void GetRightTurn_Should_Modify_Direction_To_CounterClockwise_Direction()
    {
        Direction direction = Direction.North;

        direction = direction.GetRightTurn();
        direction.Should().Be(Direction.East);

        direction = direction.GetRightTurn();
        direction.Should().Be(Direction.South);

        direction = direction.GetRightTurn();
        direction.Should().Be(Direction.West);

        direction = direction.GetRightTurn();
        direction.Should().Be(Direction.North);
    }
}
