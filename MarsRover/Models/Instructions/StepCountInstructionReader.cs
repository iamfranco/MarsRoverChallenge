using System.Text.RegularExpressions;
using MarsRover.Models.Instructions.Elementals;

namespace MarsRover.Models.Instructions;
internal class StepCountInstructionReader : IInstructionReader
{
    private readonly Regex _instructionRegex = new(@"^((L|R|M)\d*\s*)*$");

    private readonly Dictionary<char, SingularInstruction> _singularInstructions = new()
    {
        {'L', SingularInstruction.TurnLeft },
        {'R', SingularInstruction.TurnRight },
        {'M', SingularInstruction.MoveForward }
    };

    public string ExampleInstructionString => "L M3 R M2 L2";

    public bool IsValidInstruction(string? instruction) => instruction is not null && _instructionRegex.IsMatch(instruction);

    public List<SingularInstruction> EvaluateInstruction(string? instruction)
    {
        if (instruction is null)
            throw new ArgumentNullException(nameof(instruction), "instruction cannot be null");

        if (!IsValidInstruction(instruction))
            throw new ArgumentException($"Instruction [{instruction}] is not in correct format (eg {ExampleInstructionString})");

        instruction = instruction.Replace(" ", "");

        List<string> instructionList = new();
        string workingString = "";
        foreach (char c in instruction)
        {
            if ("LRM".Contains(c))
            {
                if (workingString != "")
                {
                    if (workingString.Length == 1)
                        workingString += "1";

                    instructionList.Add(workingString);
                }

                workingString = c.ToString();
            }
            else
            {
                workingString += c;
            }
        }
        instructionList.Add(workingString);

        List<SingularInstruction> result = new();

        foreach (string item in instructionList)
        {
            char symbol = item[0];
            int quantity = int.Parse(item[1..]);

            for (int i=0; i< quantity; i++)
            {
                result.Add(_singularInstructions[symbol]);
            }
        }

        return result;
    }

}
