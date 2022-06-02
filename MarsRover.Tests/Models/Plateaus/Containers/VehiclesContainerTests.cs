using MarsRover.Models.Plateaus.Containers;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Models.Plateaus.Containers;

internal class VehiclesContainerTests
{
    private Func<Coordinates, bool> coordinateValidateFunc;
    private VehiclesContainer vehiclesContainer;
    [SetUp]
    public void Setup()
    {
        // simulate PlateauBase's coordinate validation method
        coordinateValidateFunc = (coordinates) => coordinates.X >= 0 && coordinates.Y >= 0;

        vehiclesContainer = new VehiclesContainer(coordinateValidateFunc);
    }

    [Test]
    public void Constructor_With_Null_Predicate_Should_Throw_Exception()
    {
        Action act = () => vehiclesContainer = new VehiclesContainer(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Vehicles_Should_Return_Empty_Collection_Of_VehicleBase_By_Default()
    {
        vehiclesContainer.Vehicles.Count.Should().Be(0);
    }

    [Test]
    public void AddVehicle_With_Vehicle_In_Valid_Coordinates_Then_Vehicles_Should_Return_Added_Vehicle()
    {
        List<VehicleBase> roverList = new()
        {
            new Rover(new(new(1, 1), Direction.North)),
            new Rover(new(new(2, 3), Direction.South))
        };

        foreach (var rover in roverList)
        {
            vehiclesContainer.AddVehicle(rover);
        }

        var vehicleList = vehiclesContainer.Vehicles.ToList();
        vehicleList.Count.Should().Be(2);
        vehicleList.Should().BeEquivalentTo(roverList);
    }

    [Test]
    public void AddVehicle_On_Invalid_Coordinate_Should_Throw_Exception_And_Not_Modify_Vehicles()
    {
        VehicleBase rover1 = new Rover(new(new(-100, -100), Direction.North));
        Action act = () => vehiclesContainer.AddVehicle(rover1);

        act.Should().Throw<ArgumentException>();
        vehiclesContainer.Vehicles.Count.Should().Be(0);
    }

    [Test]
    public void GetVehicleAtCoordinates_With_Position_Where_There_Is_No_Vehicle_Should_Return_Null()
    {
        Rover rover1 = new(new(new(1, 1), Direction.North));
        Rover rover2 = new(new(new(2, 3), Direction.South));
        vehiclesContainer.AddVehicle(rover1);
        vehiclesContainer.AddVehicle(rover2);

        var vehicle = vehiclesContainer.GetVehicleAtCoordinates(new Coordinates(4, 4));
        vehicle.Should().Be(null);
    }

    [Test]
    public void GetVehicleAtCoordinates_With_Position_Where_There_Is_Vehicle_Should_Return_Vehicle()
    {
        Rover rover = new(new(new(1, 1), Direction.North));
        Rover roverClone = new(new(new(1, 1), Direction.North));
        vehiclesContainer.AddVehicle(rover);

        var vehicle = vehiclesContainer.GetVehicleAtCoordinates(new(1, 1));
        vehicle.Should().Be(rover);
        vehicle.Should().NotBe(roverClone);
    }

    [Test]
    public void RemoveVehicle_Then_Vehicles_Should_Not_Contain_Removed_Vehicle()
    {
        Rover rover1 = new(new(new(1, 1), Direction.North));
        Rover rover2 = new(new(new(2, 3), Direction.South));

        vehiclesContainer.AddVehicle(rover1);
        vehiclesContainer.AddVehicle(rover2);
        vehiclesContainer.RemoveVehicle(rover1);
        var vehicleList = vehiclesContainer.Vehicles.ToList();

        vehicleList.Count.Should().Be(1);
        vehicleList.Should().NotContain(rover1);
        vehicleList.Should().Contain(rover2);
    }

    [Test]
    public void RemoveVehicle_With_Vehicle_Not_Already_In_Vehicles_Should_Not_Change_Vehicles_Value()
    {
        Rover rover1 = new(new(new(1, 1), Direction.North));
        Rover rover2 = new(new(new(2, 3), Direction.South));

        vehiclesContainer.AddVehicle(rover1);
        vehiclesContainer.AddVehicle(rover2);
        var originalVehicleList = vehiclesContainer.Vehicles.ToList();

        Rover rover3 = new(new(new(1, 4), Direction.East));
        vehiclesContainer.RemoveVehicle(rover3);
        var vehicleList = vehiclesContainer.Vehicles.ToList();

        vehicleList.Count.Should().Be(originalVehicleList.Count);
        vehicleList.Should().BeEquivalentTo(originalVehicleList);
    }
}
