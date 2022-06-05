using MarsRover.AppUI;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.MapPrinters;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI;
internal class ConsoleAppTests
{
    private AppUIHandler appUIHandler;
    private Dictionary<string, Func<PlateauBase>> plateauMakers;
    private Dictionary<string, Func<Position, VehicleBase>> vehicleMakers;
    private AppController appController;

    [SetUp]
    public void Setup()
    {
        IPositionStringConverter positionStringConverter = new StandardPositionStringConverter();
        IInstructionReader instructionReader = new StandardInstructionReader();
        MapPrinter mapPrinter = new MapPrinter();

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

        appController = new(instructionReader);
        appUIHandler = new(positionStringConverter, appController, mapPrinter);
    }

    [Test]
    public void Run_With_Null_AppUIHandler_Should_Throw_Exception()
    {
        Action act = () => ConsoleApp.Run(null, plateauMakers, vehicleMakers);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Run_With_Null_PlateauMakers_Should_Throw_Exception()
    {
        Action act = () => ConsoleApp.Run(appUIHandler, null, vehicleMakers);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Run_With_Null_vehicleMakers_Should_Throw_Exception()
    {
        Action act = () => ConsoleApp.Run(appUIHandler, plateauMakers, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Run_With_UserInputs_1_Then_5_5_Then_1_2_N_Then_1_Then_LMLMLMLMM_Then_Vehicle_Should_Be_In_1_3_N()
    {
        List<string> userInputs = new() { "1", "5 5", "", "1 2 N", "1", "LMLMLMLMM" };
        List<ConsoleKeyInfo> keyInfos = new() { new('q', ConsoleKey.Q, false, false, false) };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs, keyInfos));

        ConsoleApp.Run(appUIHandler, plateauMakers, vehicleMakers);
        appController.Vehicle.Position.Should().Be(new Position(new(1, 3), Direction.North));
        appController.Vehicle.GetType().Name.Should().Be("Rover");
    }

    [Test]
    public void Run_With_UserInputs_1_Then_5_5_Then_3_3_E_Then_1_Then_MMRMMRMRRM_Then_Vehicle_Should_Be_In_5_1_E()
    {
        List<string> userInputs = new() { "1", "5 5", "", "3 3 E", "1", "MMRMMRMRRM" };
        List<ConsoleKeyInfo> keyInfos = new() { new('q', ConsoleKey.Q, false, false, false) };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs, keyInfos));

        ConsoleApp.Run(appUIHandler, plateauMakers, vehicleMakers);
        appController.Vehicle.Position.Should().Be(new Position(new(5, 1), Direction.East));
        appController.Vehicle.GetType().Name.Should().Be("Rover");
    }
}
