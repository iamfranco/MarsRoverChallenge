using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI.Components;

public static class AppSectionVehicle
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
            Coordinates initialCoordinates = positionStringConverter.ToCoordinates(positionOrCoordinatesString);
            appController.ConnectToVehicleAtCoordinates(initialCoordinates);
        }
        return true;
    }

    private static void CreateVehicleAndConnectToIt(IPositionStringConverter positionStringConverter,
        AppController appController,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers,
        string positionOrCoordinatesString)
    {
        Position initialPosition = positionStringConverter.ToPosition(positionOrCoordinatesString);
        if (!appController.IsCoordinateValidInPlateau(initialPosition.Coordinates))
        {
            throw new ArgumentException($"{positionOrCoordinatesString} is on " +
                $"Coordinates {initialPosition.Coordinates}, which is not valid on Plateau");
        }

        Dictionary<string, Func<VehicleBase>> vehicleWithKnownPositionMakers =
            MakerMenu.GetMakersWithKnownPosition(vehicleMakers, initialPosition);

        Func<VehicleBase> selectedVehicleMaker = MakerMenu.AskUserToSelectMaker(
            groupName: "vehicle",
            makers: vehicleWithKnownPositionMakers);

        VehicleBase vehicle = AppUIHelpers.ExecuteUntilNoException(selectedVehicleMaker);
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
