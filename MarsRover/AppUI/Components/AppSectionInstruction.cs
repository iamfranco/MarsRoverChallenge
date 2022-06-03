using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Instructions;

namespace MarsRover.AppUI.Components;

internal static class AppSectionInstruction
{
    public static string AskForInstructionAndSendToVehicle(IInstructionReader instructionReader, AppController appController)
    {
        string instructionString = AskUserHelpers.AskUntilValidStringInput(
            $"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
            instructionReader.IsValidInstruction);

        return appController.SendMoveInstruction(instructionString);
    }
}
