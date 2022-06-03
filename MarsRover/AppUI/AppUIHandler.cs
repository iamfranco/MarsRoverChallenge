using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;

public class AppUIHandler
{
    private readonly IPositionStringConverter _positionStringConverter;
    private readonly IInstructionReader _instructionReader;
    private readonly AppController _appController;

    public AppUIHandler(
        IPositionStringConverter positionStringConverter,
        IInstructionReader instructionReader,
        AppController appController)
    {
        _positionStringConverter = positionStringConverter;
        _instructionReader = instructionReader;
        _appController = appController;
    }

    public PlateauBase AskUserToMakePlateau(Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        return AskUserHelpers.ExecuteUntilNoException(
            () => AppSectionPlateau.AskForPlateau(_appController, plateauMakers));
    }

    public void AskUserToMakeObstacles(PlateauBase plateau)
    {
        AppSectionObstacles.AskForObstaclesUntilEmptyInput(_positionStringConverter, _appController, plateau);
    }

    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle(
        PlateauBase plateau,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        _appController.ResetRecentPath();
        AskUserHelpers.ClearScreenAndPrintMap(_appController);

        AskUserHelpers.ExecuteUntilNoException(
            () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(
                _positionStringConverter, _appController, plateau, vehicleMakers));

        AskUserHelpers.ClearScreenAndPrintMap(_appController);
        Console.WriteLine($"Connected to [{_appController.GetVehicle()!.GetType().Name}] " +
            $"at [{_appController.GetPositionString()}]");
    }

    public void AskUserForMovementInstructionAndSendToVehicle()
    {
        var message = AppSectionInstruction.AskForInstructionAndSendToVehicle(_instructionReader, _appController);

        AskUserHelpers.ClearScreenAndPrintMap(_appController);
        Console.WriteLine(message);
    }
}
