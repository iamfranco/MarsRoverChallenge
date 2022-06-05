using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.MapPrinters;
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
    private readonly IMapPrinter _mapPrinter;

    public AppUIHandler(
        IPositionStringConverter positionStringConverter,
        AppController appController,
        IMapPrinter mapPrinter)
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
        if (_appController.Plateau is null)
            throw new Exception("Plateau not connected, cannot add obstacle");

        AppSectionObstacles.AskForObstaclesUntilEmptyInput(_positionStringConverter, _appController, _mapPrinter);
    }

    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle(Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        if (vehicleMakers is null)
            throw new ArgumentException("vehicleMakers cannot be null");

        if (_appController.Plateau is null)
            throw new Exception("Plateau not connected, cannot add vehicle or connect to vehicle");

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
        if (_appController.Plateau is null)
            throw new Exception("Plateau not connected, cannot send movement instruction");

        if (_appController.Vehicle is null)
            throw new Exception("Vehicle not connected, cannot send movement instruction");

        string message = AppSectionInstruction.AskForInstructionAndSendToVehicle(_positionStringConverter, _appController);

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
        Console.WriteLine(message);
    }
}
