using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.AppUI;

internal class CommandHandlerTests
{
    private IInstructionReader instructionReader = new StandardInstructionReader();
    private IPositionStringConverter positionStringConverter = new StandardPositionStringConverter();
    private MapPrinter mapPrinter = new MapPrinter();
    private CommandHandler commandHandler;
    private PlateauBase plateau;

    [SetUp]
    public void Setup()
    {
        commandHandler = new CommandHandler(instructionReader, positionStringConverter, mapPrinter);
        plateau = new RectangularPlateau(new(5, 5));
    }

    [Test]
    public void Constructor_With_Null_InstructionReader_Should_Throw_Exception()
    {
        Action act = () => commandHandler = new CommandHandler(null, positionStringConverter, mapPrinter);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_With_Null_PositionStringConverter_Should_Throw_Exception()
    {
        Action act = () => commandHandler = new CommandHandler(instructionReader, null, mapPrinter);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_With_Null_MapPrinter_Should_Throw_Exception()
    {
        Action act = () => commandHandler = new CommandHandler(instructionReader, positionStringConverter, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void MapPrinter_Should_Return_Constructor_Input_MapPrinter()
    {
        commandHandler = new CommandHandler(instructionReader, positionStringConverter, mapPrinter);
        commandHandler.MapPrinter.Should().Be(mapPrinter);
    }

    [Test]
    public void ConnectPlateau_With_Null_Plateau_Should_Throw_Exception()
    {
        Action act = () => commandHandler.ConnectPlateau(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ConnectPlateau_With_Valid_Plateau_Should_Succeed()
    {
        Action act = () => commandHandler.ConnectPlateau(plateau);
        act.Should().NotThrow();
    }

    [Test]
    public void GetPlateau_Should_Return_Null_By_Default()
    {
        PlateauBase? plateau = commandHandler.GetPlateau();
        plateau.Should().Be(null);
    }

    [Test]
    public void GetPlateau_After_Successful_ConnectPlateau_Should_Return_Plateau()
    {
        commandHandler.ConnectPlateau(plateau);
        commandHandler.GetPlateau().Should().Be(plateau);
    }

    [Test]
    public void ConnectPlateau_Then_AddVehicleToPlateau_Then_ConnectPlateau_Then_GetVehicle_Should_Return_Null()
    {
        commandHandler.ConnectPlateau(plateau);
        Rover rover = new Rover(new Position(new(1, 2), Direction.North));
        commandHandler.AddVehicleToPlateau(rover);

        commandHandler.GetVehicle()!.Should().Be(rover);

        commandHandler.ConnectPlateau(plateau);

        commandHandler.GetVehicle().Should().Be(null);
    }

    [Test]
    public void GetVehicle_Should_Return_Null_By_Default()
    {
        VehicleBase? vehicle = commandHandler.GetVehicle();
        vehicle.Should().Be(null);
    }

    [Test]
    public void GetVehicle_After_Successful_AddVehicleToPlateau_Should_Return_Vehicle()
    {
        commandHandler.ConnectPlateau(plateau);
        Rover rover = new(new(new(1, 2), Direction.North));

        commandHandler.AddVehicleToPlateau(rover);
        VehicleBase? vehicle = commandHandler.GetVehicle();

        vehicle.Should().Be(rover);
    }

    [Test]
    public void GetVehicle_After_Successful_ConnectToVehicleAtCoordinates_Should_Return_Vehicle()
    {
        commandHandler.ConnectPlateau(plateau);
        Coordinates coordinates = new(1, 2);
        Position position = new Position(coordinates, Direction.North);
        Rover rover = new Rover(position);

        plateau.VehiclesContainer.AddVehicle(rover);
        commandHandler.ConnectPlateau(plateau);

        commandHandler.ConnectToVehicleAtCoordinates(coordinates);
        VehicleBase? vehicle = commandHandler.GetVehicle();

        vehicle.Should().Be(rover);
    }

    [Test]
    public void AddVehicleToPlateau_With_Null_Vehicle_Should_Throw_Exception()
    {
        Action act = () => commandHandler.AddVehicleToPlateau(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AddVehicleToPlateau_Before_ConnectPlateau_Should_Throw_Exception()
    {
        Rover rover = new Rover(new Position(new(1, 2), Direction.North));

        Action act = () => commandHandler.AddVehicleToPlateau(rover);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void AddVehicleToPlateau_With_Vehicle_On_Invalid_Coordinates_For_Plateau_Should_Return_False_For_Status()
    {
        Rover rover = new Rover(new Position(new(100, 200), Direction.North));
        commandHandler.ConnectPlateau(plateau);

        Action act = () => commandHandler.AddVehicleToPlateau(rover);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddVehicleToPlateau_With_Vehicle_On_Valid_Coordinates_For_Plateau_Then_Plateau_Should_Have_New_Vehicle()
    {
        Rover rover = new Rover(new Position(new(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);

        commandHandler.AddVehicleToPlateau(rover);

        plateau.VehiclesContainer.Vehicles.Should().Contain(rover);
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_Before_ConnectPlateau_Should_Throw_Exception()
    {
        Coordinates coordinates = new(1, 2);

        Action act = () => commandHandler.ConnectToVehicleAtCoordinates(coordinates);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_On_Plateau_With_No_Vehicle_Should_Throw_Exception()
    {
        Coordinates coordinates = new(1, 2);
        commandHandler.ConnectPlateau(plateau);
        Action act = () => commandHandler.ConnectToVehicleAtCoordinates(coordinates);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_With_Position_That_Does_Not_Match_Any_Vehicle_On_Plateau_Should_Throw_Exception()
    {
        Coordinates coordinates = new(1, 2);
        Rover rover = new Rover(new(new(4, 3), Direction.South));
        plateau.VehiclesContainer.AddVehicle(rover);
        commandHandler.ConnectPlateau(plateau);

        Action act = () => commandHandler.ConnectToVehicleAtCoordinates(coordinates);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ConnectToVehicleAtCoordinates_With_Position_That_Matches_With_Vehicle_On_Plateau_Should_Succeed_And_GetVehicle_Should_Return_Vehicle()
    {
        Coordinates coordinates = new(1, 2);
        Rover rover = new Rover(new(new(4, 3), Direction.South));
        Rover rover2 = new Rover(new(coordinates, Direction.North));
        Rover rover3 = new Rover(new(new(3, 3), Direction.West));
        plateau.VehiclesContainer.AddVehicle(rover);
        plateau.VehiclesContainer.AddVehicle(rover2);
        plateau.VehiclesContainer.AddVehicle(rover3);
        commandHandler.ConnectPlateau(plateau);

        Action act = () => commandHandler.ConnectToVehicleAtCoordinates(coordinates);
        act.Should().NotThrow();

        commandHandler.GetVehicle().Should().Be(rover2);
    }

    [Test]
    public void SendMoveInstruction_Without_ConnectPlateau_Should_Throw_Exception()
    {
        string instruction = "RMMLM";

        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void SendMoveInstruction_Without_Connecting_Vehicle_Should_Throw_Exception()
    {
        string instruction = "RMMLM";
        commandHandler.ConnectPlateau(plateau);

        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().Throw<Exception>();
    }

    [Test]
    public void SendMoveInstruction_With_Null_Instruction_Should_Succeed_And_Not_Modify_Vehicle_Position()
    {
        string instruction = null;
        Position originalPosition = new Position(new Coordinates(1, 2), Direction.North);
        VehicleBase vehicle = new Rover(originalPosition);

        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);

        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        commandHandler.GetVehicle()!.Position.Should().BeEquivalentTo(originalPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Empty_Instruction_String_Should_Succeed_And_Not_Modify_Vehicle_Position()
    {
        string instruction = "";
        Position originalPosition = new Position(new Coordinates(1, 2), Direction.North);
        VehicleBase vehicle = new Rover(originalPosition);

        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        commandHandler.GetVehicle()!.Position.Should().BeEquivalentTo(originalPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Invalidly_Formatted_Instruction_String_Should_Throw_Exception_And_Not_Modify_Vehicle_Position()
    {
        string instruction = "LM!LM";
        Position originalPosition = new Position(new Coordinates(1, 2), Direction.North);
        VehicleBase vehicle = new Rover(originalPosition);

        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().Throw<ArgumentException>();
        commandHandler.GetVehicle()!.Position.Should().BeEquivalentTo(originalPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Instruction_String_Which_Move_Into_Invalid_Coordinates_Of_Plateau_Should_Succeed_And_Modify_Vehicle_Position_To_Just_Before_Invalid_Coordinates()
    {
        PlateauBase plateauWithOneObstacle = new RectangularPlateau(new(5, 5));
        plateauWithOneObstacle.ObstaclesContainer.AddObstacle(new(2, 4));

        string instruction = "RMLMMM";
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        Position expectedPosition = new Position(new Coordinates(2, 3), Direction.North);

        commandHandler.ConnectPlateau(plateauWithOneObstacle);
        commandHandler.AddVehicleToPlateau(vehicle);
        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        commandHandler.GetVehicle()!.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void SendMoveInstruction_With_Valid_Instruction_String_Should_Succeed_And_Modify_Vehicle_Position()
    {
        string instruction = "MRM";
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        Position expectedPosition = new Position(new Coordinates(2, 3), Direction.East);

        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        Action act = () => commandHandler.SendMoveInstruction(instruction);

        act.Should().NotThrow();
        commandHandler.GetVehicle()!.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void GetPositionString_Before_Connecting_Vehicle_Should_Return_Vehicle_Not_Connected()
    {
        commandHandler.GetPositionString().Should().Be("Vehicle not connected");
    }

    [Test]
    public void GetPositionString_Should_Return_Vehicle_Position()
    {
        Rover rover = new Rover(new(new(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(rover);

        commandHandler.GetPositionString().Should().Be("1 2 N");
    }

    [Test]
    public void RecentPath_Should_Return_Empty_List_By_Default()
    {
        commandHandler.RecentPath.Count.Should().Be(0);
    }

    [Test]
    public void RecentPath_After_ConnectPlateau_Should_Return_Empty_List()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);
        commandHandler.RecentPath.Count.Should().Be(0);

        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MML");
        commandHandler.SendMoveInstruction("RRMM");

        commandHandler.ConnectPlateau(plateau);
        commandHandler.RecentPath.Count.Should().Be(0);
    }

    [Test]
    public void RecentPath_After_AddVehicleToPlateau_Should_Return_List_Of_Just_One_Vehicle_Position()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));

        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);

        commandHandler.RecentPath.Count.Should().Be(1);
        commandHandler.RecentPath[0].Should().Be(vehicle.Position);
    }

    [Test]
    public void RecentPath_After_ConnectToVehicleAtCoordinates_Should_Return_List_Of_Just_One_Vehicle_Position()
    {
        Coordinates coordinates = new Coordinates(1, 2);
        Position position = new Position(coordinates, Direction.North);
        VehicleBase vehicle = new Rover(position);
        plateau.VehiclesContainer.AddVehicle(vehicle);

        commandHandler.ConnectPlateau(plateau);
        commandHandler.ConnectToVehicleAtCoordinates(coordinates);

        commandHandler.RecentPath.Count.Should().Be(1);
        commandHandler.RecentPath[0].Should().Be(vehicle.Position);
    }

    [Test]
    public void RecentPath_After_Successful_SendMoveInstruction_Should_Return_Instructions_Travel_Path()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));

        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MRM");

        List<Position> expectedResult = new()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East),
            new(new(2, 3), Direction.East)
        };

        List<Position> actualResult = commandHandler.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void RecentPath_After_Multiple_Successful_SendMoveInstruction_Should_Return_Last_Instructions_Travel_Path()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MMLL");
        commandHandler.SendMoveInstruction("MMLL");
        commandHandler.SendMoveInstruction("MRMR");

        List<Position> expectedResult = new()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East),
            new(new(2, 3), Direction.East),
            new(new(2, 3), Direction.South)
        };

        List<Position> actualResult = commandHandler.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void RecentPath_After_Invalidly_Formatted_Instruction_To_SendMoveInstruction_Should_Return_Last_Successful_Instruction_Travel_Path()
    {
        Action act;
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MMLL");
        commandHandler.SendMoveInstruction("MMLL");
        commandHandler.SendMoveInstruction("MRMR");
        act = () => commandHandler.SendMoveInstruction("ML!!?");
        act.Should().Throw<ArgumentException>();

        List<Position> expectedResult = new()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East),
            new(new(2, 3), Direction.East),
            new(new(2, 3), Direction.South)
        };

        List<Position> actualResult = commandHandler.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void RecentPath_After_SendMoveInstruction_With_Instruction_That_Leads_To_Obstacle_Should_Return_Path_Up_Until_Obstacle()
    {
        PlateauBase plateauWithObstacle = new RectangularPlateau(new(5, 5));
        plateauWithObstacle.ObstaclesContainer.AddObstacle(new(2, 4));

        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateauWithObstacle);
        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MML");
        commandHandler.SendMoveInstruction("RRMM");

        List<Position> expectedResult = new()
        {
            new(new(1, 4), Direction.West),
            new(new(1, 4), Direction.North),
            new(new(1, 4), Direction.East)
        };

        List<Position> actualResult = commandHandler.RecentPath;

        actualResult.Count.Should().Be(expectedResult.Count);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void RecentPath_After_Re_AddVehicleToPlateau_Should_Return_List_With_New_Vehicle_Position()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        VehicleBase vehicle2 = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MML");
        commandHandler.SendMoveInstruction("RRMM");

        commandHandler.AddVehicleToPlateau(vehicle2);

        commandHandler.RecentPath.Count.Should().Be(1);
        commandHandler.RecentPath[0].Should().Be(vehicle2.Position);
    }

    [Test]
    public void ResetRecentPath_Before_ConnectingVehicle_Should_Set_RecentPath_To_Empty_List()
    {
        commandHandler.ConnectPlateau(plateau);
        commandHandler.ResetRecentPath();

        commandHandler.RecentPath.Count.Should().Be(0);
    }

    [Test]
    public void ResetRecentPath_With_Vehicle_Connected_Should_Set_ReachPath_To_List_Containing_Just_Vehicle_Position()
    {
        VehicleBase vehicle = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        commandHandler.ConnectPlateau(plateau);
        commandHandler.AddVehicleToPlateau(vehicle);
        commandHandler.SendMoveInstruction("MML");
        commandHandler.SendMoveInstruction("RRMM");

        commandHandler.ResetRecentPath();

        commandHandler.RecentPath.Count.Should().Be(1);
        commandHandler.RecentPath[0].Should().Be(vehicle.Position);
    }
}
