using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
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
    private readonly CommandHandler _commandHandler;

    public AppUIHandler(
        IPositionStringConverter positionStringConverter,
        IInstructionReader instructionReader,
        CommandHandler commandHandler)
    {
        _positionStringConverter = positionStringConverter;
        _instructionReader = instructionReader;
        _commandHandler = commandHandler;
    }

    public PlateauBase AskUserToMakePlateau(Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        return AskUserHelpers.ExecuteUntilNoException(
            () => AppSectionPlateau.AskForPlateau(_commandHandler, plateauMakers));
    }

    public void AskUserToMakeObstacles(PlateauBase plateau)
    {
        AppSectionObstacles.AskForObstaclesUntilEmptyInput(_positionStringConverter, _commandHandler, plateau);
    }

    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle(
        PlateauBase plateau,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        _commandHandler.ResetRecentPath();
        AskUserHelpers.ClearScreenAndPrintMap(_commandHandler);

        AskUserHelpers.ExecuteUntilNoException(
            () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(
                _positionStringConverter, _commandHandler, plateau, vehicleMakers));

        AskUserHelpers.ClearScreenAndPrintMap(_commandHandler);
        Console.WriteLine($"Connected to [{_commandHandler.GetVehicle()!.GetType().Name}] " +
            $"at [{_commandHandler.GetPositionString()}]");
    }

    public void AskUserForMovementInstructionAndSendToVehicle()
    {
        var message = AppSectionInstruction.AskForInstructionAndSendToVehicle(_instructionReader, _commandHandler);

        AskUserHelpers.ClearScreenAndPrintMap(_commandHandler);
        Console.WriteLine(message);
    }
}
