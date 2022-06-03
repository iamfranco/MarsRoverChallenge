using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI.Components;

internal static class AppSectionVehicle
{
    public static bool AskForPositionOrCoordinatesToCreateOrConnectVehicle(
        IPositionStringConverter positionStringConverter,
        AppController appController,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        string positionOrCoordinatesString = AskForPositionOrCoordinatesString(positionStringConverter);

        if (positionStringConverter.IsValidPositionString(positionOrCoordinatesString))
        {
            CreateVehicleAndConnectToIt(positionStringConverter,
                appController, vehicleMakers, positionOrCoordinatesString);
        }
        else
        {
            var initialCoordinates = positionStringConverter.ToCoordinates(positionOrCoordinatesString);
            appController.ConnectToVehicleAtCoordinates(initialCoordinates);
        }
        return true;
    }

    private static void CreateVehicleAndConnectToIt(IPositionStringConverter positionStringConverter,
        AppController appController,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers,
        string positionOrCoordinatesString)
    {
        var initialPosition = positionStringConverter.ToPosition(positionOrCoordinatesString);
        if (!appController.IsCoordinateValidInPlateau(initialPosition.Coordinates))
        {
            throw new ArgumentException($"{positionOrCoordinatesString} is on " +
                $"Coordinates {initialPosition.Coordinates}, which is not valid on Plateau");
        }

        var vehicleWithKnownPositionMakers = MakerMenu.GetMakersWithKnownPosition(vehicleMakers, initialPosition);

        var selectedVehicleMaker = MakerMenu.AskUserToSelectMaker(
            groupName: "vehicle",
            makers: vehicleWithKnownPositionMakers);

        var vehicle = AppUIHelpers.ExecuteUntilNoException(selectedVehicleMaker);
        appController.AddVehicleToPlateau(vehicle);
    }

    private static string AskForPositionOrCoordinatesString(IPositionStringConverter positionStringConverter)
    {
        return AppUIHelpers.AskUntilValidStringInput(
            $"Enter Position (eg \"{positionStringConverter.ExamplePositionString}\") to add new Vehicle, or " +
            $"\nEnter Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\") to connect with existing vehicle: ",
            (input) => positionStringConverter.IsValidPositionString(input) ||
                positionStringConverter.IsValidCoordinateString(input));
    }
}
