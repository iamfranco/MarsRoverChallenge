namespace MarsRover.Models.Instructions
{
    public interface IInstructionReader
    {
        bool IsValidInstruction(string? instruction);
        List<SingularInstruction> EvaluateInstruction(string? instruction);
    }
}