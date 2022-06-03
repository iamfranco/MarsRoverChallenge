using MarsRover.AppUI.Helpers;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.AppUI.Components;

internal static class AppSectionObstacles
{
    public static void AskForObstaclesUntilEmptyInput(
        IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau)
    {
        while (true)
        {
            try
            {
                var obstacleCoordinates = AskUserHelpers.AskUntilValidStringInput($"Enter Obstacle Coordinate " +
                    $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
                    (s) => string.IsNullOrEmpty(s) || positionStringConverter.IsValidCoordinateString(s));

                if (string.IsNullOrEmpty(obstacleCoordinates))
                    break;

                plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
                AskUserHelpers.ClearScreenAndPrintMap(commandHandler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
