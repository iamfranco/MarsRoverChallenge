using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Positions
{
    internal class DirectionTests
    {
        [Test]
        public void Constructor_With_Null_String_Input_Should_Throw_Exception()
        {
            Direction direction;

            Action act = () => direction = new(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Constructor_With_Empty_String_Input_Should_Throw_Exception()
        {
            Direction direction;

            Action act = () => direction = new("");
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Constructor_With_Valid_String_Input_Should_Succeed()
        {
            Direction direction;
            Action act;

            act = () => direction = new("North");
            act.Should().NotThrow();

            act = () => direction = new("East");
            act.Should().NotThrow();

            act = () => direction = new("South");
            act.Should().NotThrow();

            act = () => direction = new("West");
            act.Should().NotThrow();
        }

        [Test]
        public void Constructor_With_Invalid_String_Input_Should_Throw_Exception()
        {
            Direction direction;
            Action act;

            act = () => direction = new("Northh");
            act.Should().Throw<ArgumentOutOfRangeException>();

            act = () => direction = new("Eastsdf");
            act.Should().Throw<ArgumentOutOfRangeException>();

            act = () => direction = new("Souuth");
            act.Should().Throw<ArgumentOutOfRangeException>();

            act = () => direction = new("!West");
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Constructor_With_CaseInsensitive_Valid_String_Input_Should_Succeed()
        {
            Direction direction;
            Action act;

            act = () => direction = new("NORTH");
            act.Should().NotThrow();

            act = () => direction = new("NorTH");
            act.Should().NotThrow();

            act = () => direction = new("north");
            act.Should().NotThrow();
        }

        [Test]
        public void Name_Should_Match_LowerCase_Of_Constructor_Input()
        {
            string expectedResultSouth = "SOUTH";
            Direction direction = new(expectedResultSouth);
            direction.Name.Should().Be(expectedResultSouth.ToLower());

            string expectedResultNorth = "NoRtH";
            direction = new(expectedResultNorth);
            direction.Name.Should().Be(expectedResultNorth.ToLower());
        }

        [Test]
        public void MovementVector_Should_Match_Correct_Coordinates_For_Constructor_Input()
        {
            Direction direction;

            direction = new("north");
            direction.MovementVector.Should().Be(new Coordinates(0, 1));

            direction = new("south");
            direction.MovementVector.Should().Be(new Coordinates(0, -1));

            direction = new("east");
            direction.MovementVector.Should().Be(new Coordinates(1, 0));

            direction = new("west");
            direction.MovementVector.Should().Be(new Coordinates(-1, 0));
        }

        [Test]
        public void TurnLeft_Should_Turn_Direction_CounterClockwise()
        {
            Direction direction = new("north");

            direction.TurnLeft();
            direction.Name.Should().Be("west");

            direction.TurnLeft();
            direction.Name.Should().Be("south");

            direction.TurnLeft();
            direction.Name.Should().Be("east");

            direction.TurnLeft();
            direction.Name.Should().Be("north");
        }

        [Test]
        public void TurnRight_Should_Turn_Direction_Clockwise()
        {
            Direction direction = new("north");

            direction.TurnRight();
            direction.Name.Should().Be("east");

            direction.TurnRight();
            direction.Name.Should().Be("south");

            direction.TurnRight();
            direction.Name.Should().Be("west");

            direction.TurnRight();
            direction.Name.Should().Be("north");
        }

        [Test]
        public void TurnLeft_Then_TurnRight_Should_Not_Change_Direction()
        {
            Direction direction = new("north");

            direction.TurnLeft();
            direction.TurnRight();
            direction.TurnLeft();
            direction.TurnRight();

            direction.Name.Should().Be("north");
        }

        [Test]
        public void Clone_Should_Copy_Value_But_Not_Reference()
        {
            Direction direction = new("north");
            Direction directionCopy = direction.Clone();

            directionCopy.Name.Should().Be(direction.Name);

            directionCopy.TurnLeft();
            directionCopy.Name.Should().NotBe(direction.Name);
        }
    }
}
