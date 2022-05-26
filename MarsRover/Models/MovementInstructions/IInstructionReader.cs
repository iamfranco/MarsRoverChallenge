namespace MarsRover.Models.MovementInstructions
{
    public interface IInstructionReader
    {
        List<SingularInstruction> EvaluateInstruction(string instruction);
    }
}
