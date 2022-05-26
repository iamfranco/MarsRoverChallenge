namespace MarsRover.Models.MovementInstructions
{
    public class StandardInstructionReader : IInstructionReader
    {
        private readonly Dictionary<char, SingularInstruction> _symbolInstruction = new()
        {
            {'L', SingularInstruction.TurnLeft },
            {'R', SingularInstruction.TurnRight },
            {'M', SingularInstruction.MoveForward }
        };

        public List<SingularInstruction> EvaluateInstruction(string instruction)
        {
            if (instruction is null)
                throw new ArgumentException("Instruction cannot be null", nameof(instruction));

            instruction = instruction.Replace(" ", string.Empty);

            if (!IsValidInstruction(instruction))
                throw new ArgumentException("Instruction contains invalid character", nameof(instruction));

            List<SingularInstruction> instructionList = new();
            foreach (char c in instruction)
                instructionList.Add(_symbolInstruction[c]);

            return instructionList;
        }

        private bool IsValidInstruction(string instruction) => instruction.All(item => _symbolInstruction.ContainsKey(item));
    }
}
