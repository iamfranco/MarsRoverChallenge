using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Models.Plateaus
{
    internal class RectangularPlateauTests
    {
        [Test]
        public void Constructor_Should_Throw_Exception_For_Input_maximumCoordinates_With_Negative_Components()
        {
            RectangularPlateau plateau;
            Coordinates maximumCoordinates;
            Action act;

            maximumCoordinates = new(-5, 8);
            act = () => plateau = new(maximumCoordinates);
            act.Should().Throw<ArgumentException>();

            maximumCoordinates = new(3, -4);
            act = () => plateau = new(maximumCoordinates);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_Should_NotThrow_Exception_For_Input_maximumCoordinates_With_Only_Positive_Components()
        {
            RectangularPlateau plateau;
            Coordinates maximumCoordinates;
            Action act;

            maximumCoordinates = new(5, 8);
            act = () => plateau = new(maximumCoordinates);
            act.Should().NotThrow();

            maximumCoordinates = new(3, 1);
            act = () => plateau = new(maximumCoordinates);
            act.Should().NotThrow();
        }

        [Test]
        public void AddVehicle_With_Vehicle_On_Invalid_Coordinates_For_Plateau_Should_Not_Change_GetVehicles_Value()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(3, 3));
            Rover rover1 = new(new(new(1, 1), Direction.North));
            Rover rover2 = new(new(new(2, 3), Direction.South));
            Rover invalidRover1 = new(new(new(10, 10), Direction.South));
            Rover invalidRover2 = new(new(new(3, 3), Direction.East));

            plateau.AddVehicle(rover1);
            plateau.AddVehicle(rover2);
            List<VehicleBase> initialVehicleList = plateau.GetVehicles().ToList();

            plateau.AddVehicle(invalidRover1);
            plateau.AddVehicle(invalidRover2);

            List<VehicleBase> VehicleListAfterAddingInvalidVehicles = plateau.GetVehicles().ToList();

            VehicleListAfterAddingInvalidVehicles.Count.Should().Be(initialVehicleList.Count);
            VehicleListAfterAddingInvalidVehicles.Should().BeEquivalentTo(initialVehicleList);
        }


        [Test]
        public void IsCoordinateValidInPlateau_With_Coordinate_Outside_Of_Plateau_Should_Return_False()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.IsCoordinateValidInPlateau(new(-1, -2)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(-1, 3)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(3, 10)).Should().Be(false);
        }

        [Test]
        public void IsCoordinateValidInPlateau_With_Coordinate_Inside_Plateau_And_Not_On_Obstacle_Should_Return_True()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.IsCoordinateValidInPlateau(new(4, 3)).Should().Be(true);
            plateau.IsCoordinateValidInPlateau(new(1, 5)).Should().Be(true);
            plateau.IsCoordinateValidInPlateau(new(0, 0)).Should().Be(true);
        }

        [Test]
        public void IsCoordinateValidInPlateau_With_Coordinate_On_Obstacle_Should_Return_False()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(1, 2));
            plateau.AddObstacle(new(4, 3));

            plateau.IsCoordinateValidInPlateau(new(1, 2)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(4, 3)).Should().Be(false);
        }

        [Test]
        public void IsCoordinateValidInPlateau_With_Coordinate_On_Vehicles_Should_Return_False()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddVehicle(new Rover(new(new(1, 2), Direction.North)));
            plateau.AddVehicle(new Rover(new(new(4, 3), Direction.East)));

            plateau.IsCoordinateValidInPlateau(new(1, 2)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(4, 3)).Should().Be(false);
        }
    }
}
