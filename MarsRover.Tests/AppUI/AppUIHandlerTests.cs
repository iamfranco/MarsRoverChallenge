using MarsRover.AppUI;
using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI;
internal class AppUIHandlerTests
{
    IPositionStringConverter positionStringConverter;
    AppController appController;
    MapPrinter mapPrinter;
    AppUIHandler appUIHandler;
    Dictionary<string, Func<PlateauBase>> plateauMakers;
    Dictionary<string, Func<Position, VehicleBase>> vehicleMakers;

    [SetUp]
    public void Setup()
    {
        IInstructionReader instructionReader = new StandardInstructionReader();

        positionStringConverter = new StandardPositionStringConverter();
        appController = new AppController(instructionReader);
        mapPrinter = new MapPrinter();

        appUIHandler = new AppUIHandler(positionStringConverter, appController, mapPrinter);

        plateauMakers = new()
        {
            {
                "Rectangular Plateau", () =>
                {
                    string maximumCoordinatesString = AppUIHelpers.AskUntilValidStringInput(
                        $"Enter Maximum Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                        positionStringConverter.IsValidCoordinateString);

                    Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
                    return new RectangularPlateau(maximumCoordinates);
                }
            },
            {
                "Circular Plateau", () =>
                {
                    string radiusString = AppUIHelpers.AskUntilValidStringInput(
                        $"Enter Radius (eg \"5\"): ",
                        s => int.TryParse(s, out _));

                    return new CircularPlateau(int.Parse(radiusString));
                }
            },
        };

        vehicleMakers = new()
        {
            { "Rover", position => new Rover(position) },
            { "Wall E", position => new WallE(position) }
        };
    }

    [Test]
    public void Construct_With_Null_PositionStringConverter_Should_Throw_Exception()
    {
        Action act = () => appUIHandler = new AppUIHandler(null, appController, mapPrinter);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Construct_With_Null_AppController_Should_Throw_Exception()
    {
        Action act = () => appUIHandler = new AppUIHandler(positionStringConverter, null, mapPrinter);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Construct_With_Null_MapPrinter_Should_Throw_Exception()
    {
        Action act = () => appUIHandler = new AppUIHandler(positionStringConverter, appController, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AskUserToMakePlateau_With_Null_Maker_Should_Throw_Exception()
    {
        Action act = () => appUIHandler.AskUserToMakePlateau(null);
        act.Should().Throw<ArgumentException>().WithMessage("plateauMakers cannot be null");
    }

    [Test]
    public void AskUserToMakePlateau_With_Empty_Maker_Should_Throw_Exception()
    {

        Action act = () => appUIHandler.AskUserToMakePlateau(new());
        act.Should().Throw<ArgumentException>().WithMessage("plateauMakers cannot be empty");
    }

    [Test]
    public void AskUserToMakePlateau_With_UserInput_1_Then_5_8_Then_AppController_Plateau_Should_Return_RectangularPlateau_Of_Size_5_by_8()
    {
        List<string> userInputs = new() { "1", "5 8" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        appUIHandler.AskUserToMakePlateau(plateauMakers);

        appController.Plateau.Should().NotBeNull();
        appController.Plateau.GetType().Name.Should().Be(nameof(RectangularPlateau));
        appController.Plateau.MaximumCoordinates.Should().Be(new Coordinates(5, 8));
    }

    [Test]
    public void AskUserToMakeObstacles_Before_ConnectingPlateau_Should_Throw_Exception()
    {
        Action act = () => appUIHandler.AskUserToMakeObstacles();
        act.Should().Throw<Exception>().WithMessage("Plateau not connected, cannot add obstacle");
    }

    [Test]
    public void AskUserToMakeObstacles_With_UserInput_4_5_Then_ajkdjslkfjd_Then1_2_Then_EmptyString_Then_Plateau_Should_Have_Obstacles_Only_On_4_5_And_1_2()
    {
        List<string> userInputs = new() { "4 5", "ajkdjslkfjd", "1 2", "" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        appController.ConnectPlateau(new RectangularPlateau(new(10, 10)));

        appUIHandler.AskUserToMakeObstacles();

        List<Coordinates> expectedObstacles = new() { new(4, 5), new(1, 2) };
        List<Coordinates> actualObstacles = appController.Plateau.ObstaclesContainer.ObstacleCoordinates.ToList();
        actualObstacles.Should().BeEquivalentTo(expectedObstacles);
    }

    [Test]
    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle_With_Null_VehicleMakers_Should_Throw_Exception()
    {
        appController.ConnectPlateau(new RectangularPlateau(new(10, 10)));

        Action act = () => appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(null);
        act.Should().Throw<ArgumentException>().WithMessage("vehicleMakers cannot be null");
    }

    [Test]
    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle_Before_ConnectingPlateau_Should_Throw_Exception()
    {
        Action act = () => appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(vehicleMakers);
        act.Should().Throw<Exception>().WithMessage("Plateau not connected, cannot add vehicle or connect to vehicle");
    }

    [Test]
    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle_With_UserInputs_4_5_S_Then_1_Should_Create_Vehicle_On_Plateau_At_4_5_S_Of_Type_Rover()
    {
        List<string> userInputs = new() { "4 5 S", "1" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        appController.ConnectPlateau(new RectangularPlateau(new(10, 10)));

        appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(vehicleMakers);

        appController.Vehicle.Position.Should().Be(new Position(new(4, 5), Direction.South));
        appController.Vehicle.GetType().Name.Should().Be("Rover");
    }

    [Test]
    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle_With_UserInputs_4_5_On_Plateau_With_A_Vehicle_On_4_5_Should_Connect_To_It()
    {
        VehicleBase vehicle = new Rover(new(new(4, 5), Direction.South));

        appController.ConnectPlateau(new RectangularPlateau(new(10, 10)));
        appController.AddVehicleToPlateau(vehicle);
        appController.AddVehicleToPlateau(new WallE(new(new(0, 0), Direction.North)));
        appController.AddVehicleToPlateau(new Rover(new(new(2, 4), Direction.East)));

        List<string> userInputs = new() { "4 5" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(vehicleMakers);

        appController.Vehicle.Position.Should().Be(new Position(new(4, 5), Direction.South));
        appController.Vehicle.GetType().Name.Should().Be("Rover");
    }
}
