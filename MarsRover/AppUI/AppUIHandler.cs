using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;

public class AppUIHandler
{
    private readonly IPositionStringConverter _positionStringConverter;
    private readonly AppController _appController;
    private readonly MapPrinter _mapPrinter;

    public AppUIHandler(
        IPositionStringConverter positionStringConverter,
        AppController appController,
        MapPrinter mapPrinter)
    {
        if (positionStringConverter is null)
            throw new ArgumentNullException(nameof(positionStringConverter));

        if (appController is null)
            throw new ArgumentNullException(nameof(appController));

        if (mapPrinter is null)
            throw new ArgumentNullException(nameof(mapPrinter));

        _positionStringConverter = positionStringConverter;
        _appController = appController;
        _mapPrinter = mapPrinter;
    }

    public void AskUserToMakePlateau(Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        if (plateauMakers is null)
            throw new ArgumentException($"{nameof(plateauMakers)} cannot be null");

        if (plateauMakers.Count == 0)
            throw new ArgumentException($"{nameof(plateauMakers)} cannot be empty");

        AppUIHelpers.ExecuteUntilNoException(() => AppSectionPlateau.AskForPlateau(_appController, plateauMakers));

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
    }

    public void AskUserToMakeObstacles()
    {
        AppSectionObstacles.AskForObstaclesUntilEmptyInput(_positionStringConverter, _appController, _mapPrinter);
    }

    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle(Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        _appController.DisconnectVehicle();
        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);

        AppUIHelpers.ExecuteUntilNoException(
            () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(
                _positionStringConverter, _appController, vehicleMakers));

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
        Console.WriteLine($"Connected to [{_appController.Vehicle!.GetType().Name}] " +
            $"at [{_positionStringConverter.ToPositionString(_appController.Vehicle!.Position)}]");
    }

    public void AskUserForMovementInstructionAndSendToVehicle()
    {
        var message = AppSectionInstruction.AskForInstructionAndSendToVehicle(_positionStringConverter, _appController);

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
        Console.WriteLine(message);
    }
}
