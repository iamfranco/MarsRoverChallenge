using MarsRover.AppUI;
using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;

namespace MarsRover.Tests.AppUI;
internal class AppUIHandlerTests
{
    IPositionStringConverter positionStringConverter;
    AppController appController;
    MapPrinter mapPrinter;
    AppUIHandler appUIHandler;
    Dictionary<string, Func<PlateauBase>> plateauMakers;

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
        plateauMakers.Clear();

        Action act = () => appUIHandler.AskUserToMakePlateau(plateauMakers);
        act.Should().Throw<ArgumentException>().WithMessage("plateauMakers cannot be empty");
    }
}
