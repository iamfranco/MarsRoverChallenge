using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.MapPrinters;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;

namespace MarsRover.AppUI.Components;

public static class AppSectionObstacles
{
    public static void AskForObstaclesUntilEmptyInput(
        IPositionStringConverter positionStringConverter, AppController appController, IMapPrinter mapPrinter)
    {
        while (true)
        {
            try
            {
                string obstacleCoordinatesString = AppUIHelpers.AskUntilValidStringInput($"Enter Obstacle Coordinate " +
                    $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
                    (s) => string.IsNullOrEmpty(s) || positionStringConverter.IsValidCoordinateString(s));

                if (string.IsNullOrEmpty(obstacleCoordinatesString))
                    break;

                appController.AddObstacleToPlateau(positionStringConverter.ToCoordinates(obstacleCoordinatesString));
                AppUIHelpers.ClearScreenAndPrintMap(appController, mapPrinter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
