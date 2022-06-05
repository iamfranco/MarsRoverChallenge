using MarsRover.AppUI.MapPrinters;
using MarsRover.Controllers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;

namespace MarsRover.Tests.AppUI.MapPrinters;
internal class MapPrinterTests
{
    private IMapPrinter mapPrinter;
    private AppController appController;
    [SetUp]
    public void Setup()
    {
        mapPrinter = new MapPrinter();

        IInstructionReader instructionReader = new StandardInstructionReader();
        appController = new AppController(instructionReader);
    }

    [Test]
    public void PrintMap_With_Null_AppController_Should_Throw_Exception()
    {
        Action act = () => mapPrinter.PrintMap(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void PrintMap_With_AppController_With_No_Plateau_Connected_Should_Throw_Exception()
    {
        Action act = () => mapPrinter.PrintMap(appController);
        act.Should().Throw<Exception>();
    }

    [Test]
    public void PrintMap_With_AppController_With_Plateau_Connected_Should_Succeed()
    {
        PlateauBase plateau = new RectangularPlateau(new(10, 8));
        appController.ConnectPlateau(plateau);

        Action act = () => mapPrinter.PrintMap(appController);
        act.Should().NotThrow();
    }
}
