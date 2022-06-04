using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI.Components;

public static class AppSectionInstruction
{
    public static string AskForInstructionAndSendToVehicle(
        IPositionStringConverter positionStringConverter, AppController appController)
    {
        IInstructionReader instructionReader = appController.InstructionReader;

        string instructionString = AppUIHelpers.AskUntilValidStringInput(
            $"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
            instructionReader.IsValidInstruction);

        VehicleMovementStatus vehicleMovementStatus = appController.SendMoveInstruction(instructionString);

        VehicleBase vehicle = appController.Vehicle!;
        string vehicleTypeName = vehicle.GetType().Name;
        string positionString = positionStringConverter.ToPositionString(vehicle.Position);

        return GetMessageStringToPrint(vehicleMovementStatus, instructionString, vehicleTypeName, positionString);
    }

    private static string GetMessageStringToPrint(
        VehicleMovementStatus vehicleMovementStatus,
        string instructionString,
        string vehicleTypeName, string positionString)
    {
        string message = "";

        if (vehicleMovementStatus is VehicleMovementStatus.NoMovement)
            message = $"Instruction is empty, [{vehicleTypeName}] is in the same Position: [{positionString}]";

        if (vehicleMovementStatus is VehicleMovementStatus.DangerAhead)
            message = $"[{vehicleTypeName}] sensed danger ahead, so stopped at [{positionString}] " +
                $"instead of applying full instruction [{instructionString}]";

        if (vehicleMovementStatus is VehicleMovementStatus.ReachedDestination)
            message = $"[{vehicleTypeName}] reached Position: [{positionString}] after fully applying " +
                $"instruction [{instructionString}]";

        return message;
    }
}
