using MarsRover.AppUI.Helpers;
using MarsRover.Models.Instructions;

namespace MarsRover.AppUI.Components;

internal static class AppSectionInstruction
{
    public static string AskForInstructionAndSendToVehicle(IInstructionReader instructionReader, CommandHandler commandHandler)
    {
        string instructionString = AskUserHelpers.AskUntilValidStringInput(
            $"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
            instructionReader.IsValidInstruction);

        return commandHandler.SendMoveInstruction(instructionString);
    }
}
