using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;

public static class AskUser
{
    public static PlateauBase AskUserToMakePlateau(CommandHandler commandHandler,
        Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        return AskUserHelpers.ExecuteUntilNoException(
            () => AppSectionPlateau.AskForPlateau(commandHandler, plateauMakers));
    }

    public static void AskUserToMakeObstacles(IPositionStringConverter positionStringConverter,
        CommandHandler commandHandler, PlateauBase plateau)
    {
        AppSectionObstacles.AskForObstaclesUntilEmptyInput(positionStringConverter, commandHandler, plateau);
    }

    public static void AskUserToCreateNewVehicleOrConnectToExistingVehicle(
        IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        commandHandler.ResetRecentPath();
        AskUserHelpers.ClearScreenAndPrintMap(commandHandler);

        AskUserHelpers.ExecuteUntilNoException(
            () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(
                positionStringConverter, commandHandler, plateau, vehicleMakers));

        AskUserHelpers.ClearScreenAndPrintMap(commandHandler);
        Console.WriteLine($"Connected to [{commandHandler.GetVehicle()!.GetType().Name}] " +
            $"at [{commandHandler.GetPositionString()}]");
    }

    public static void AskUserForMovementInstructionAndSendToVehicle(
        IInstructionReader instructionReader, CommandHandler commandHandler, PlateauBase plateau)
    {
        var message = AppSectionInstruction.AskForInstructionAndSendToVehicle(instructionReader, commandHandler);

        AskUserHelpers.ClearScreenAndPrintMap(commandHandler);
        Console.WriteLine(message);
    }
}
