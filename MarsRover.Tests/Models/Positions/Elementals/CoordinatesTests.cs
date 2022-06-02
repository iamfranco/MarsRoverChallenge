using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Tests.Models.Positions.Elementals;

internal class CoordinatesTests
{
    [Test]
    public void Constructor_With_Valid_X_Y_Inputs_Should_Succeed()
    {
        Coordinates coordinates;
        Action act;

        act = () => coordinates = new Coordinates(5, 4);
        act.Should().NotThrow();

        act = () => coordinates = new Coordinates(-1, 3);
        act.Should().NotThrow();

        act = () => coordinates = new Coordinates(2, -1);
        act.Should().NotThrow();

        act = () => coordinates = new Coordinates(0, 0);
        act.Should().NotThrow();
    }

    [Test]
    public void X_Should_Return_Constructor_X_Input()
    {
        Coordinates coordinates;

        coordinates = new Coordinates(5, 4);
        coordinates.X.Should().Be(5);

        coordinates = new Coordinates(-1, 4);
        coordinates.X.Should().Be(-1);
    }

    [Test]
    public void Y_Should_Return_Constructor_Y_Input()
    {
        Coordinates coordinates;

        coordinates = new Coordinates(5, -2);
        coordinates.Y.Should().Be(-2);

        coordinates = new Coordinates(-1, 4);
        coordinates.Y.Should().Be(4);
    }

    [Test]
    public void Coordinate_Plus_AnotherCoordinate_Should_Return_Coordinate_Of_Vector_Sum()
    {
        Coordinates coordinatesA = new(3, 2);
        Coordinates coordinatesB = new(5, -3);

        Coordinates expectedResult = new(8, -1);
        var actualResult = coordinatesA + coordinatesB;

        actualResult.Should().Be(expectedResult);
    }
}
