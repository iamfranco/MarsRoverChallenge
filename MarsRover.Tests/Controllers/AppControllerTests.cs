using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Controllers;

internal class AppControllerTests
{
    private readonly IInstructionReader instructionReader = new StandardInstructionReader();
    private AppController appController;
    private PlateauBase plateau;

    [SetUp]
    public void Setup()
    {
        appController = new AppController(instructionReader);
        plateau = new RectangularPlateau(new(5, 5));
    }

    [Test]
    public void Constructor_With_Null_InstructionReader_Should_Throw_Exception()
    {
        Action act = () => appController = new AppController(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void InstructionReader_After_Successful_Construction_Should_Return_InstructionReader()
    {
        appController = new AppController(instructionReader);
        appController.InstructionReader.Should().Be(instructionReader);
    }

    [Test]
    public void ConnectPlateau_With_Null_Plateau_Should_Throw_Exception()
    {
        Action act = () => appController.ConnectPlateau(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ConnectPlateau_With_Valid_Plateau_Should_Succeed()
    {
        Action act = () => appController.ConnectPlateau(plateau);
        act.Should().NotThrow();
    }

    [Test]
    public void GetPlateau_Should_Return_Null_By_Default()
    {
        var plateau = appController.Plateau;
        plateau.Should().Be(null);
    }

    [Test]
    public void GetPlateau_After_Successful_ConnectPlateau_Should_Return_Plateau()
    {
        appController.ConnectPlateau(plateau);
        appController.Plateau.Should().Be(plateau);
    }

    [Test]
    public void AddObstacleToPlateau_Before_ConnectPlateau_Should_Throw_Exception()
    {
        Coordinates obstacle = new Coordinates(2, 3);

        Action act = () => appController.AddObstacleToPlateau(obstacle);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void AddObstacleToPlateau_After_Successful_ConnectPlateau_On_Invalid_Coordinates_Should_Throw_Exception()
    {
        Coordinates obstacle = new Coordinates(-100, -100);
        appController.ConnectPlateau(plateau);

        Action act = () => appController.AddObstacleToPlateau(obstacle);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddObstacleToPlateau_After_Successful_ConnectPlateau_Should_Add_Obstacle_To_Plateau()
    {
        Coordinates obstacle = new Coordinates(2, 3);
        appController.ConnectPlateau(plateau);

        appController.AddObstacleToPlateau(obstacle);

        plateau.ObstaclesContainer.ObstacleCoordinates.Should().Contain(obstacle);
    }

    [Test]
    public void IsCoordinateValidInPlateau_Before_ConnectPlateau_Should_Throw_Exception()
    {
        Action act = () => appController.IsCoordinateValidInPlateau(new Coordinates(2, 3));

        act.Should().Throw<Exception>();
    }

    [Test]
    public void IsCoordinateValidInPlateau_On_Invalid_Coordinates_Should_Return_False()
    {
        appController.ConnectPlateau(plateau);
        bool actualResult = appController.IsCoordinateValidInPlateau(new Coordinates(-100, 3));

        actualResult.Should().Be(false);
    }

    [Test]
    public void IsCoordinateValidInPlateau_On_valid_Coordinates_Should_Return_True()
    {
        appController.ConnectPlateau(plateau);
        bool actualResult = appController.IsCoordinateValidInPlateau(new Coordinates(2, 3));

        actualResult.Should().Be(true);
    }

    [Test]
    public void ConnectPlateau_Then_AddVehicleToPlateau_Then_ConnectPlateau_Then_GetVehicle_Should_Return_Null()
    {
        appController.ConnectPlateau(plateau);
        var rover = new Rover(new Position(new(1, 2), Direction.North));
        appController.AddVehicleToPlateau(rover);

        appController.Vehicle!.Should().Be(rover);

        appController.ConnectPlateau(plateau);

        appController.Vehicle.Should().Be(null);
    }

    [Test]
    public void GetVehicle_Should_Return_Null_By_Default()
    {
        var vehicle = appController.Vehicle;
        vehicle.Should().Be(null);
    }

    [Test]
    public void GetVehicle_After_Successful_AddVehicleToPlateau_Should_Return_Vehicle()
    {
        appController.ConnectPlateau(plateau);
        Rover rover = new(new(new(1, 2), Direction.North));

        appController.AddVehicleToPlateau(rover);
        var vehicle = appController.Vehicle;

        vehicle.Should().Be(rover);
    }

    [Test]
    public void GetVehicle_After_Successful_ConnectToVehicleAtCoordinates_Should_Return_Vehicle()
    {
        appController.ConnectPlateau(plateau);
        Coordinates coordinates = new(1, 2);
        var position = new Position(coordinates, Direction.North);
        var rover = new Rover(position);

        plateau.VehiclesContainer.AddVehicle(rover);
        appController.ConnectPlateau(plateau);

        appController.ConnectToVehicleAtCoordinates(coordinates);
        var vehicle = appController.Vehicle;

        vehicle.Should().Be(rover);
    }

    [Test]
    public void AddVehicleToPlateau_With_Null_Vehicle_Should_Throw_Exception()
    {
        Action act = () => appController.AddVehicleToPlateau(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AddVehicleToPlateau_Before_ConnectPlateau_Should_Throw_Exception()
    {
        var rover = new Rover(new Position(new(1, 2), Direction.North));

        Action act = () => appController.AddVehicleToPlateau(rover);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void AddVehicleToPlateau_With_Vehicle_On_Invalid_Coordinates_For_Plateau_Should_Return_False_For_Status()
    {
        var rover = new Rover(new Position(new(100, 200), Direction.North));
        appController.ConnectPlateau(plateau);

        Action act = () => appController.AddVehicleToPlateau(rover);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddVehicleToPlateau_With_Vehicle_On_Valid_Coordinates_For_Plateau_Then_Plateau_Should_Have_New_Vehicle()
    {
        var rover = new Rover(new Position(new(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);

        appController.AddVehicleToPlateau(rover);

        plateau.VehiclesContainer.Vehicles.Should().Contain(rover);
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_Before_ConnectPlateau_Should_Throw_Exception()
    {
        Coordinates coordinates = new(1, 2);

        Action act = () => appController.ConnectToVehicleAtCoordinates(coordinates);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_On_Plateau_With_No_Vehicle_Should_Throw_Exception()
    {
        Coordinates coordinates = new(1, 2);
        appController.ConnectPlateau(plateau);
        Action act = () => appController.ConnectToVehicleAtCoordinates(coordinates);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_With_Position_That_Does_Not_Match_Any_Vehicle_On_Plateau_Should_Throw_Exception()
    {
        Coordinates coordinates = new(1, 2);
        var rover = new Rover(new(new(4, 3), Direction.South));
        plateau.VehiclesContainer.AddVehicle(rover);
        appController.ConnectPlateau(plateau);

        Action act = () => appController.ConnectToVehicleAtCoordinates(coordinates);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_With_Position_That_Matches_With_Vehicle_On_Plateau_Should_Succeed_And_GetVehicle_Should_Return_Vehicle()
    {
        Coordinates coordinates = new(1, 2);
        var rover = new Rover(new(new(4, 3), Direction.South));
        var rover2 = new Rover(new(coordinates, Direction.North));
        var rover3 = new Rover(new(new(3, 3), Direction.West));
        plateau.VehiclesContainer.AddVehicle(rover);
        plateau.VehiclesContainer.AddVehicle(rover2);
        plateau.VehiclesContainer.AddVehicle(rover3);
        appController.ConnectPlateau(plateau);

        Action act = () => appController.ConnectToVehicleAtCoordinates(coordinates);
        act.Should().NotThrow();

        appController.Vehicle.Should().Be(rover2);
    }

    [Test]
    public void DisconnectVehicle_Then_Vehicle_Should_Return_Null()
    {
        var rover = new Rover(new Position(new(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(rover);

        appController.DisconnectVehicle();

        appController.Vehicle.Should().Be(null);

        appController.DisconnectVehicle();

        appController.Vehicle.Should().Be(null);
    }

    [Test]
    public void DisconnectVehicle_Then_RecentPath_Should_Return_Empty_List()
    {
        var rover = new Rover(new Position(new(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(rover);

        appController.DisconnectVehicle();

        appController.RecentPath.Count.Should().Be(0);
    }

    [Test]
    public void SendMoveInstruction_Without_ConnectPlateau_Should_Throw_Exception()
    {
        var instruction = "RMMLM";

        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void SendMoveInstruction_Without_Connecting_Vehicle_Should_Throw_Exception()
    {
        var instruction = "RMMLM";
        appController.ConnectPlateau(plateau);

        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void SendMoveInstruction_With_Null_Instruction_Should_Succeed_And_Not_Modify_Vehicle_Position()
    {
        string instruction = null;
        var originalPosition = new Position(new Coordinates(1, 2), Direction.North);
        VehicleBase vehicle = new Rover(originalPosition);

        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);

        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        appController.Vehicle!.Position.Should().Be(originalPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Empty_Instruction_String_Should_Succeed_And_Not_Modify_Vehicle_Position()
    {
        var instruction = "";
        var originalPosition = new Position(new Coordinates(1, 2), Direction.North);
        VehicleBase vehicle = new Rover(originalPosition);

        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        appController.Vehicle!.Position.Should().Be(originalPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Invalidly_Formatted_Instruction_String_Should_Throw_Exception_And_Not_Modify_Vehicle_Position()
    {
        var instruction = "LM!LM";
        var originalPosition = new Position(new Coordinates(1, 2), Direction.North);
        VehicleBase vehicle = new Rover(originalPosition);

        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().Throw<ArgumentException>();
        appController.Vehicle!.Position.Should().Be(originalPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Instruction_String_Which_Move_Into_Invalid_Coordinates_Of_Plateau_Should_Succeed_And_Modify_Vehicle_Position_To_Just_Before_Invalid_Coordinates()
    {
        PlateauBase plateauWithOneObstacle = new RectangularPlateau(new(5, 5));
        plateauWithOneObstacle.ObstaclesContainer.AddObstacle(new(2, 4));

        var instruction = "RMLMMM";
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        var expectedPosition = new Position(new Coordinates(2, 3), Direction.North);

        appController.ConnectPlateau(plateauWithOneObstacle);
        appController.AddVehicleToPlateau(vehicle);
        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        appController.Vehicle!.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Valid_Instruction_String_Should_Succeed_And_Modify_Vehicle_Position()
    {
        var instruction = "MRM";
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        var expectedPosition = new Position(new Coordinates(2, 3), Direction.East);

        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        Action act = () => appController.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        appController.Vehicle!.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void RecentPath_Should_Return_Empty_List_By_Default()
    {
        appController.RecentPath.Count.Should().Be(0);
    }

    [Test]
    public void RecentPath_After_ConnectPlateau_Should_Return_Empty_List()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);
        appController.RecentPath.Count.Should().Be(0);

        appController.AddVehicleToPlateau(vehicle);
        appController.SendMoveInstruction("MML");
        appController.SendMoveInstruction("RRMM");

        appController.ConnectPlateau(plateau);
        appController.RecentPath.Count.Should().Be(0);
    }

    [Test]
    public void RecentPath_After_AddVehicleToPlateau_Should_Return_List_Of_Just_One_Vehicle_Position()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));

        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);

        appController.RecentPath.Count.Should().Be(1);
        appController.RecentPath[0].Should().Be(vehicle.Position);
    }

    [Test]
    public void RecentPath_After_ConnectToVehicleAtCoordinates_Should_Return_List_Of_Just_One_Vehicle_Position()
    {
        var coordinates = new Coordinates(1, 2);
        var position = new Position(coordinates, Direction.North);
        VehicleBase vehicle = new Rover(position);
        plateau.VehiclesContainer.AddVehicle(vehicle);

        appController.ConnectPlateau(plateau);
        appController.ConnectToVehicleAtCoordinates(coordinates);

        appController.RecentPath.Count.Should().Be(1);
        appController.RecentPath[0].Should().Be(vehicle.Position);
    }

    [Test]
    public void RecentPath_After_Successful_SendMoveInstruction_Should_Return_Instructions_Travel_Path()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));

        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        appController.SendMoveInstruction("MRM");

        List<Position> expectedResult = new()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East),
            new(new(2, 3), Direction.East)
        };

        var actualResult = appController.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().Equal(expectedResult);
    }

    [Test]
    public void RecentPath_After_Multiple_Successful_SendMoveInstruction_Should_Return_Last_Instructions_Travel_Path()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        appController.SendMoveInstruction("MMLL");
        appController.SendMoveInstruction("MMLL");
        appController.SendMoveInstruction("MRMR");

        List<Position> expectedResult = new()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East),
            new(new(2, 3), Direction.East),
            new(new(2, 3), Direction.South)
        };

        var actualResult = appController.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().Equal(expectedResult);
    }

    [Test]
    public void RecentPath_After_Invalidly_Formatted_Instruction_To_SendMoveInstruction_Should_Return_Last_Successful_Instruction_Travel_Path()
    {
        Action act;
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        appController.SendMoveInstruction("MMLL");
        appController.SendMoveInstruction("MMLL");
        appController.SendMoveInstruction("MRMR");
        act = () => appController.SendMoveInstruction("ML!!?");
        act.Should().Throw<ArgumentException>();

        List<Position> expectedResult = new()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East),
            new(new(2, 3), Direction.East),
            new(new(2, 3), Direction.South)
        };

        var actualResult = appController.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().Equal(expectedResult);
    }

    [Test]
    public void RecentPath_After_SendMoveInstruction_With_Instruction_That_Leads_To_Obstacle_Should_Return_Path_Up_Until_Obstacle()
    {
        PlateauBase plateauWithObstacle = new RectangularPlateau(new(5, 5));
        plateauWithObstacle.ObstaclesContainer.AddObstacle(new(2, 4));

        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        appController.ConnectPlateau(plateauWithObstacle);
        appController.AddVehicleToPlateau(vehicle);
        appController.SendMoveInstruction("MML");
        appController.SendMoveInstruction("RRMM");

        List<Position> expectedResult = new()
        {
            new(new(1, 4), Direction.West),
            new(new(1, 4), Direction.North),
            new(new(1, 4), Direction.East)
        };

        var actualResult = appController.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().Equal(expectedResult);
    }

    [Test]
    public void RecentPath_After_Re_AddVehicleToPlateau_Should_Return_List_With_New_Vehicle_Position()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        VehicleBase vehicle2 = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        appController.ConnectPlateau(plateau);
        appController.AddVehicleToPlateau(vehicle);
        appController.SendMoveInstruction("MML");
        appController.SendMoveInstruction("RRMM");

        appController.AddVehicleToPlateau(vehicle2);

        appController.RecentPath.Count.Should().Be(1);
        appController.RecentPath[0].Should().Be(vehicle2.Position);
    }
}
