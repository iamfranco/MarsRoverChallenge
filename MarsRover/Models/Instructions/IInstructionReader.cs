﻿using MarsRover.Models.Instructions.Elementals;

namespace MarsRover.Models.Instructions;

public interface IInstructionReader
{
    string ExampleInstructionString { get; }
    bool IsValidInstruction(string? instruction);
    List<SingularInstruction> EvaluateInstruction(string? instruction);
}
