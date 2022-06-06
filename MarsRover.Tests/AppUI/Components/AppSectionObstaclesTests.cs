using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.MapPrinters;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Components;
internal class AppSectionObstaclesTests
{
    private IPositionStringConverter positionStringConverter;
    private IInstructionReader instructionReader;
    private AppController appController;
    private IMapPrinter mapPrinter;

    [SetUp]
    public void Setup()
    {
        positionStringConverter = new StandardPositionStringConverter();
        instructionReader = new StandardInstructionReader();
        appController = new AppController(instructionReader);

        PlateauBase plateau = new RectangularPlateau(new(10, 5));
        appController.ConnectPlateau(plateau);

        mapPrinter = new MapPrinter();
    }

    [Test]
    public void AskForObstaclesUntilEmptyInput_With_UserInput_Empty_String_Should_Not_Add_Obstacles_To_Plateau()
    {
        List<string> userInputs = new() { "" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        AppSectionObstacles.AskForObstaclesUntilEmptyInput(positionStringConverter, appController, mapPrinter);

        appController.Plateau!.ObstaclesContainer.ObstacleCoordinates.Count.Should().Be(0);
    }

    [Test]
    public void AskForObstaclesUntilEmptyInput_With_UserInput_CoordinatesStrings_With_Empty_String_At_The_End_Should_Add_Obstacles_To_Plateau_At_Coordinates()
    {
        List<string> userInputs = new() { "1 2", "2 3", "5 5", "" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));
        List<Coordinates> expectedObstacles = new() { new(1, 2), new(2, 3), new(5, 5) };

        AppSectionObstacles.AskForObstaclesUntilEmptyInput(positionStringConverter, appController, mapPrinter);
        List<Coordinates> actualObstacles = appController.Plateau!.ObstaclesContainer.ObstacleCoordinates.ToList();

        actualObstacles.Should().Equal(expectedObstacles);
    }
}
