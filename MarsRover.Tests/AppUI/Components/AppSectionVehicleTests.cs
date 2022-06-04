using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Components;
internal class AppSectionVehicleTests
{
    private IPositionStringConverter positionStringConverter;
    private AppController appController;
    private Dictionary<string, Func<Position, VehicleBase>> vehicleMakers;

    [SetUp]
    public void Setup()
    {
        IInstructionReader instructionReader = new StandardInstructionReader();
        PlateauBase plateau = new RectangularPlateau(new(10, 10));

        positionStringConverter = new StandardPositionStringConverter();
        appController = new AppController(instructionReader);
        appController.ConnectPlateau(plateau);

        vehicleMakers = new()
        {
            {"Rover", position => new Rover(position) },
            {"Wall-E", position => new WallE(position) }
        };
    }

    [Test]
    public void AskForPositionOrCoordinatesToCreateOrConnectVehicle_With_UserInput_1_2_N_Then_2_Should_Create_New_WallE_Vehicle_At_Position_1_2_N()
    {
        List<string> userInput = new() { "1 2 N", "2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInput));

        AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(positionStringConverter, appController, vehicleMakers);

        appController.Vehicle!.Position.Should().Be(new Position(new(1, 2), Direction.North));
        appController.Vehicle.GetType().Name.Should().Be("WallE");
    }

    [Test]
    public void AskForPositionOrCoordinatesToCreateOrConnectVehicle_With_UserInput_10000_10000_N_Should_Throw_Exception()
    {
        List<string> userInput = new() { "10000 10000 N", "2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInput));

        Action act = () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(positionStringConverter, appController, vehicleMakers);

        act.Should().Throw<ArgumentException>().WithMessage("10000 10000 N is on Coordinates (10000, 10000), which is not valid on Plateau");
    }

    [Test]
    public void AskForPositionOrCoordinatesToCreateOrConnectVehicle_With_UserInput_1_2_Should_Connect_To_Existing_Vehicle_At_Coordinates_1_2()
    {
        VehicleBase rover = new Rover(new Position(new(1, 2), Direction.North));
        appController.AddVehicleToPlateau(rover);
        VehicleBase wallE = new WallE(new Position(new(5, 6), Direction.East));
        appController.AddVehicleToPlateau(wallE);

        List<string> userInput = new() { "1 2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInput));

        AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(positionStringConverter, appController, vehicleMakers);

        appController.Vehicle!.Position.Should().Be(new Position(new(1, 2), Direction.North));
        appController.Vehicle.GetType().Name.Should().Be(nameof(Rover));
    }

    [Test]
    public void AskForPositionOrCoordinatesToCreateOrConnectVehicle_With_UserInput_Coordinates_Where_Coordinates_Has_No_Vehicle_Should_Throw_Exception()
    {
        VehicleBase wallE = new WallE(new Position(new(5, 6), Direction.East));
        appController.AddVehicleToPlateau(wallE);

        List<string> userInput = new() { "1 2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInput));

        Action act = () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(positionStringConverter, appController, vehicleMakers);

        act.Should().Throw<Exception>().WithMessage("Coordinates (1, 2) does not match any vehicle's coordinates on plateau");
    }

    [Test]
    public void AskForPositionOrCoordinatesToCreateOrConnectVehicle_With_UserInputs_For_CreatingVehicle_Should_Use_First_Valid_Inputs()
    {
        string firstValidPositionString = "1 2 N";
        string firstValidVehicleNumber = "2";
        List<string> userInput = new() { "jaskdjfslkjskls", firstValidPositionString, "4 5 N", "-1", "5", firstValidVehicleNumber, "1" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInput));

        AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(positionStringConverter, appController, vehicleMakers);

        appController.Vehicle!.Position.Should().Be(new Position(new(1, 2), Direction.North));
        appController.Vehicle.GetType().Name.Should().Be("WallE");
    }

    [Test]
    public void AskForPositionOrCoordinatesToCreateOrConnectVehicle_With_UserInput_For_ConnectingToExistingVehicle_Should_Use_First_Valid_Inputs()
    {
        VehicleBase rover = new Rover(new Position(new(1, 2), Direction.North));
        appController.AddVehicleToPlateau(rover);
        VehicleBase wallE = new WallE(new Position(new(5, 6), Direction.East));
        appController.AddVehicleToPlateau(wallE);

        string firstValidCoordinatesString = "1 2";
        List<string> userInput = new() { "jaskdjfslkjskls", firstValidCoordinatesString, "4 5 N" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInput));

        AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(positionStringConverter, appController, vehicleMakers);

        appController.Vehicle!.Position.Should().Be(new Position(new(1, 2), Direction.North));
        appController.Vehicle.GetType().Name.Should().Be(nameof(Rover));
    }
}
