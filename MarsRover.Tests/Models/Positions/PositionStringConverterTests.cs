using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Positions
{
    internal class PositionStringConverterTests
    {
        PositionStringConverter positionStringConverter;

        readonly List<string> validPositionStrings = new() { "1 2 N", "5 4 S", "-5 4 E", "0 -4 W" };
        readonly List<string> validCoordinateStrings = new() { "1 2", "5 4", "-5 4", "0 -4" };
        readonly List<Coordinates> coordinatesForValidStrings = new() { new(1, 2), new(5, 4), new(-5, 4), new(0, -4) };
        readonly List<Direction> directionForValidStrings = new() { Direction.North, Direction.South, Direction.East, Direction.West };

        readonly List<string> invalidPositionStrings = new() { "", " ", "--1 2 N", "54 S", "-5-4 E", "0 -4 WW" };
        readonly List<string> invalidCoordinateStrings = new() { "--1 2", "54", "-5-4", "0- 4" };

        [SetUp]
        public void Setup()
        {
            positionStringConverter = new();
        }

        [Test]
        public void IsValidPositionString_Should_Return_False_For_Null_Input()
        {
            positionStringConverter.IsValidPositionString(null)
                .Should().Be(false);
        }

        [Test]
        public void IsValidPositionString_Should_Return_False_For_Invalid_PositionString()
        {
            foreach (string invalidPositionString in invalidPositionStrings)
            {
                positionStringConverter.IsValidPositionString(invalidPositionString)
                    .Should().Be(false);
            }
        }

        [Test]
        public void IsValidPositionString_Should_Return_True_For_Valid_PositionString()
        {
            foreach (string validPositionString in validPositionStrings)
            {
                positionStringConverter.IsValidPositionString(validPositionString)
                    .Should().Be(true);
            }
        }

        [Test]
        public void IsValidCoordinateString_Should_Return_False_For_Null_Input()
        {
            positionStringConverter.IsValidCoordinateString(null)
                .Should().Be(false);
        }

        [Test]
        public void IsValidCoordinateString_Should_Return_False_For_Empty_String_Input()
        {
            positionStringConverter.IsValidCoordinateString("")
                .Should().Be(false);
        }

        [Test]
        public void IsValidCoordinateString_Should_Return_False_For_Invalid_CoordinateString()
        {
            foreach (string invalidCoordinateString in invalidCoordinateStrings)
            {
                positionStringConverter.IsValidCoordinateString(invalidCoordinateString)
                    .Should().Be(false);
            }
        }

        [Test]
        public void IsValidCoordinateString_Should_Return_True_For_Valid_CoordinateString()
        {
            foreach (string validCoordinateString in validCoordinateStrings)
            {
                positionStringConverter.IsValidCoordinateString(validCoordinateString)
                    .Should().Be(true);
            }
        }

        [Test]
        public void ToCoordinatesDirection_Should_Throw_Exception_For_Null_Input()
        {
            Action act;

            act = () => positionStringConverter.ToCoordinatesDirection(null);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ToCoordinatesDirection_Should_Throw_Exception_For_Invalid_PositionString()
        {
            Action act;

            foreach (string invalidCoordinateString in invalidCoordinateStrings)
            {
                act = () => positionStringConverter.ToCoordinatesDirection(invalidCoordinateString);
                act.Should().Throw<ArgumentException>();
            }
        }

        [Test]
        public void ToCoordinatesDirection_Should_Return_Correct_Coordinates_And_Direction_For_valid_PositionString()
        {
            for (int i = 0; i < validPositionStrings.Count; i++)
            {
                string positionString = validPositionStrings[i];
                Coordinates expectedCoordinates = coordinatesForValidStrings[i];
                Direction expectedDirection = directionForValidStrings[i];

                (Coordinates actualCoordinates, Direction actualDirection) =
                    positionStringConverter.ToCoordinatesDirection(positionString);

                actualCoordinates.Should().Be(expectedCoordinates);
                actualDirection.Should().Be(expectedDirection);
            }
        }

        [Test]
        public void ToCoordinates_Should_Throw_Exception_For_Null_Input()
        {
            Action act;

            act = () => positionStringConverter.ToCoordinates(null);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ToCoordinates_Should_Throw_Exception_Invalid_CoordinateString()
        {
            Action act;

            foreach (string invalidCoordinateString in invalidCoordinateStrings)
            {
                act = () => positionStringConverter.ToCoordinates(invalidCoordinateString);
                act.Should().Throw<ArgumentException>();
            }
        }

        [Test]
        public void ToCoordinates_Should_Return_Correct_Coordinates_For_valid_CoordinateString()
        {
            for (int i = 0; i < validCoordinateStrings.Count; i++)
            {
                string coordinateString = validCoordinateStrings[i];
                Coordinates expectedCoordinates = coordinatesForValidStrings[i];

                Coordinates actualCoordinates = positionStringConverter.ToCoordinates(coordinateString);

                actualCoordinates.Should().Be(expectedCoordinates);
            }
        }

        [Test]
        public void ToPositionString_Should_Return_Correct_Position_String_For_Coordinates_And_Direction()
        {
            positionStringConverter.ToPositionString(new Coordinates(1, 2), Direction.North)
                .Should().Be("1 2 N");

            positionStringConverter.ToPositionString(new Coordinates(5, -30), Direction.East)
                .Should().Be("5 -30 E");
        }
    }
}