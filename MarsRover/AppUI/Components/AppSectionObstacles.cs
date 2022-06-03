using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.AppUI.Components;

internal static class AppSectionObstacles
{
    public static void AskForObstaclesUntilEmptyInput(
        IPositionStringConverter positionStringConverter, AppController appController, MapPrinter mapPrinter, PlateauBase plateau)
    {
        while (true)
        {
            try
            {
                var obstacleCoordinatesString = AskUserHelpers.AskUntilValidStringInput($"Enter Obstacle Coordinate " +
                    $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
                    (s) => string.IsNullOrEmpty(s) || positionStringConverter.IsValidCoordinateString(s));

                if (string.IsNullOrEmpty(obstacleCoordinatesString))
                    break;

                plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinatesString));
                AskUserHelpers.ClearScreenAndPrintMap(appController, mapPrinter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
