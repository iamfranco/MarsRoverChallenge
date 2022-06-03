using MarsRover.Models.Elementals;

namespace MarsRover.Tests.Models.Elementals;

internal class DirectionExtensionsTests
{
    [Test]
    public void GetMovementVector_Should_Return_Correct_Movement_Vector()
    {
        var north = Direction.North;
        north.GetMovementVector().Should().Be(new Coordinates(0, 1));

        var south = Direction.South;
        south.GetMovementVector().Should().Be(new Coordinates(0, -1));

        var east = Direction.East;
        east.GetMovementVector().Should().Be(new Coordinates(1, 0));

        var west = Direction.West;
        west.GetMovementVector().Should().Be(new Coordinates(-1, 0));
    }

    [Test]
    public void GetLeftTurn_Should_Modify_Direction_To_CounterClockwise_Direction()
    {
        var direction = Direction.North;

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
        var direction = Direction.North;

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
