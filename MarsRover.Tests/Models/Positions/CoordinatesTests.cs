using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Positions
{
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
        public void Coordinate_Plus_AnotherCoordinate_Should_Return_Coordinate_Of_Vector_Sum()
        {
            Coordinates coordinatesA = new(3, 2);
            Coordinates coordinatesB = new(5, -3);

            Coordinates expectedResult = new(8, -1);
            Coordinates actualResult = coordinatesA + coordinatesB;

            actualResult.Should().Be(expectedResult);
        }
    }
}
