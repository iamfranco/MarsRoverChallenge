using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Models.Plateaus
{
    internal class RectangularPlateauTests
    {
        [Test]
        public void Constructor_Should_Throw_Exception_For_Input_PlateauSize_With_Negative_Components()
        {
            RectangularPlateau plateau;
            Coordinates plateauSize;
            Action act;

            plateauSize = new(-5, 8);
            act = () => plateau = new(plateauSize);
            act.Should().Throw<ArgumentException>();

            plateauSize = new(3, -4);
            act = () => plateau = new(plateauSize);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_Should_NotThrow_Exception_For_Input_PlateauSize_With_Only_Positive_Components()
        {
            RectangularPlateau plateau;
            Coordinates plateauSize;
            Action act;

            plateauSize = new(5, 8);
            act = () => plateau = new(plateauSize);
            act.Should().NotThrow();

            plateauSize = new(3, 1);
            act = () => plateau = new(plateauSize);
            act.Should().NotThrow();
        }

        [Test]
        public void ObstacleCoordinates_Should_Return_Empty_List_By_Default()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.ObstacleCoordinates.Count.Should().Be(0);
        }

        [Test]
        public void AddObstacle_Then_ObstacleCoordinates_Should_Return_AddedObstacleCoordinate()
        {
            RectangularPlateau plateau = new(new(5, 5));
            List<Coordinates> obstacleCoordinates = new()
            {
                new(1, 3),
                new(2, 2),
                new(3, 4)
            };

            foreach (Coordinates obstacle in obstacleCoordinates)
            {
                plateau.AddObstacle(obstacle);
            }

            plateau.ObstacleCoordinates.Count.Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                plateau.ObstacleCoordinates[i].Should().Be(obstacleCoordinates[i]);
            }
        }

        [Test]
        public void AddObstacle_Then_ObstacleCoordinates_Should_Not_Have_Duplicate_Obstacles()
        {
            RectangularPlateau plateau = new(new(5, 5));

            plateau.AddObstacle(new(1, 3));
            plateau.ObstacleCoordinates.Count.Should().Be(1);

            plateau.AddObstacle(new(1, 3));
            plateau.ObstacleCoordinates.Count.Should().Be(1);

            plateau.AddObstacle(new(2, 2));
            plateau.ObstacleCoordinates.Count.Should().Be(2);

            plateau.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 3));
            plateau.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 2));
        }

        [Test]
        public void AddObstacle_On_Invalid_Plateau_Coordinate_Should_Not_Change_Obstacles()
        {
            RectangularPlateau plateau = new(new(5, 5));

            plateau.AddObstacle(new(10, 10));
            plateau.ObstacleCoordinates.Count.Should().Be(0);
        }

        [Test]
        public void RemoveObstacle_Then_ObstacleCoordinates_Should_Not_Include_Removed_ObstacleCoordinate()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(1, 2));
            plateau.AddObstacle(new(2, 4));

            plateau.ObstacleCoordinates.Count.Should().Be(2);

            plateau.RemoveObstacle(new(1, 2));
            plateau.ObstacleCoordinates.Count.Should().Be(1);
            plateau.ObstacleCoordinates.Contains(new Coordinates(1, 2)).Should().Be(false);
        }

        [Test]
        public void RemoveObstacle_With_NonExistent_Obstacle_Should_Not_Change_ObstacleCoordinates()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(1, 2));
            plateau.AddObstacle(new(2, 4));

            plateau.ObstacleCoordinates.Count.Should().Be(2);
            plateau.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 2));
            plateau.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 4));

            plateau.RemoveObstacle(new(2, 2));
            plateau.ObstacleCoordinates.Count.Should().Be(2);
            plateau.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 2));
            plateau.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 4));
        }

        [Test]
        public void GetVehicles_Should_Return_Empty_List_Of_VehicleBase_By_Default()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.GetVehicles().Count.Should().Be(0);
        }

        [Test]
        public void AddVehicle_With_Vehicle_In_Plateau_Then_GetVehicles_Should_Contain_Added_Vehicle()
        {
            RectangularPlateau plateau = new(new(5, 5));
            Rover rover1 = new(new(new(1, 1), Direction.North));
            Rover rover2 = new(new(new(2, 3), Direction.South));

            plateau.AddVehicle(rover1);
            plateau.AddVehicle(rover2);

            List<VehicleBase> vehicleList = plateau.GetVehicles().ToList();
            vehicleList.Count.Should().Be(2);
            vehicleList.Should().Contain(rover1);
            vehicleList.Should().Contain(rover2);
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
        public void GetVehicleAtPosition_With_Position_Where_There_Is_No_Vehicle_Should_Return_Null()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(3, 3));
            Rover rover1 = new(new(new(1, 1), Direction.North));
            Rover rover2 = new(new(new(2, 3), Direction.South));
            plateau.AddVehicle(rover1);
            plateau.AddVehicle(rover2);

            VehicleBase? vehicle = plateau.GetVehicleAtPosition(new Position(new(4, 4), Direction.North));
            vehicle.Should().Be(null);
        }

        [Test]
        public void GetVehicleAtPosition_With_Position_Where_There_Is_Vehicle_Should_Return_Vehicle()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(3, 3));
            Rover rover = new(new(new(1, 1), Direction.North));
            Rover roverClone = new(new(new(1, 1), Direction.North));
            plateau.AddVehicle(rover);

            VehicleBase? vehicle = plateau.GetVehicleAtPosition(new Position(new(1, 1), Direction.North));
            vehicle.Should().Be(rover);
            vehicle.Should().NotBe(roverClone);
        }

        [Test]
        public void RemoveVehicle_Then_GetVehicle_Should_Not_Contain_Removed_Vehicle()
        {
            RectangularPlateau plateau = new(new(5, 5));
            Rover rover1 = new(new(new(1, 1), Direction.North));
            Rover rover2 = new(new(new(2, 3), Direction.South));

            plateau.AddVehicle(rover1);
            plateau.AddVehicle(rover2);
            plateau.RemoveVehicle(rover1);
            List<VehicleBase> vehicleList = plateau.GetVehicles().ToList();

            vehicleList.Count.Should().Be(1);
            vehicleList.Should().NotContain(rover1);
            vehicleList.Should().Contain(rover2);
        }

        [Test]
        public void RemoveVehicle_With_Vehicle_Not_Already_In_Plateau_Should_Not_Change_GetVehicles_Value()
        {
            RectangularPlateau plateau = new(new(5, 5));
            Rover rover1 = new(new(new(1, 1), Direction.North));
            Rover rover2 = new(new(new(2, 3), Direction.South));

            plateau.AddVehicle(rover1);
            plateau.AddVehicle(rover2);
            List<VehicleBase> originalVehicleList = plateau.GetVehicles().ToList();

            Rover rover3 = new(new(new(1, 4), Direction.East));
            plateau.RemoveVehicle(rover3);
            List<VehicleBase> vehicleList = plateau.GetVehicles().ToList();

            vehicleList.Count.Should().Be(originalVehicleList.Count);
            vehicleList.Should().BeEquivalentTo(originalVehicleList);
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
