using System.Text.RegularExpressions;

namespace MarsRover.Models.Instructions
{
    public class StandardInstructionReader : IInstructionReader
    {
        private readonly Regex _instructionRegex = new(@"^(L|R|M|\s)*$");

        private readonly Dictionary<char, SingularInstruction> _singularInstructions = new()
        {
            {'L', SingularInstruction.TurnLeft },
            {'R', SingularInstruction.TurnRight },
            {'M', SingularInstruction.MoveForward }
        };

        public bool IsValidInstruction(string? instruction) => instruction is not null && _instructionRegex.IsMatch(instruction);

        public List<SingularInstruction> EvaluateInstruction(string? instruction)
        {
            if (instruction is null)
                throw new ArgumentNullException(nameof(instruction), "instruction cannot be null");

            if (!IsValidInstruction(instruction))
                throw new ArgumentException($"Instruction {instruction} is not in correct format (eg MRMMLMM)", nameof(instruction));

            instruction = instruction.Replace(" ", "");

            List<SingularInstruction> result = new();

            foreach (char symbol in instruction)
            {
                result.Add(_singularInstructions[symbol]);
            }

            return result;
        }
    }
}
