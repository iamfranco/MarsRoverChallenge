using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.AppUI.Components;

internal static class AppSectionObstacles
{
    public static void AskForObstaclesUntilEmptyInput(
        IPositionStringConverter positionStringConverter, AppController appController, MapPrinter mapPrinter)
    {
        while (true)
        {
            try
            {
                var obstacleCoordinatesString = AppUIHelpers.AskUntilValidStringInput($"Enter Obstacle Coordinate " +
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
