using MarsRover.Controllers;

namespace MarsRover.AppUI.MapPrinters;
public interface IMapPrinter
{
    void PrintMap(AppController appController);
}