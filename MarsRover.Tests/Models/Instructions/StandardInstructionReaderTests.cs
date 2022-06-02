using MarsRover.Models.Instructions;

namespace MarsRover.Tests.Models.Instructions;

internal class StandardInstructionReaderTests
{
    private readonly List<string> invalidInstructions = new() { "asjdkfl", "lmr", "LM!" };
    private readonly List<string> validInstructions = new() { "", "LMRL", "L MMM R MMM", "LL MM LL RR R" };
    private StandardInstructionReader standardInstructionReader;
    [SetUp]
    public void Setup()
    {
        standardInstructionReader = new();
    }

    [Test]
    public void IsValidInstruction_Should_Return_False_For_Null_String_Input()
    {
        standardInstructionReader.IsValidInstruction(null)
            .Should().Be(false);
    }

    [Test]
    public void IsValidInstruction_Should_Return_False_For_Invalid_Instruction_String()
    {
        foreach (string invalidInstruction in invalidInstructions)
        {
            standardInstructionReader.IsValidInstruction(invalidInstruction)
                .Should().Be(false);
        }
    }

    [Test]
    public void IsValidInstruction_Should_Return_True_For_Valid_Instruction_String()
    {
        foreach (string validInstruction in validInstructions)
        {
            standardInstructionReader.IsValidInstruction(validInstruction)
                .Should().Be(true);
        }
    }

    [Test]
    public void EvaluateInstruction_Should_Throw_Exception_For_Null_String_Input()
    {
        Action act = () => standardInstructionReader.EvaluateInstruction(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void EvaluateInstruction_Should_Throw_Exception_For_Invalid_Instruction_String()
    {
        Action act;

        foreach (string invalidInstruction in invalidInstructions)
        {
            act = () => standardInstructionReader.EvaluateInstruction(invalidInstruction);
            act.Should().Throw<ArgumentException>();
        }
    }

    [Test]
    public void EvaluateInstruction_Should_Return_Empty_List_For_Empty_String_Instruction()
    {
        string validInstruction = "";
        List<SingularInstruction> expectedResult = new();

        List<SingularInstruction> actualResult = standardInstructionReader.EvaluateInstruction(validInstruction);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void EvaluateInstruction_Should_Return_List_Of_SingularInstruction_Corresponding_To_Valid_Input_Instruction_String()
    {
        string validInstruction = "LMMRM";
        List<SingularInstruction> expectedResult = new()
        {
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward
        };

        List<SingularInstruction> actualResult = standardInstructionReader.EvaluateInstruction(validInstruction);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void EvaluateInstruction_Should_Return_List_Of_SingularInstruction_Corresponding_To_Valid_Input_Instruction_String_With_Spaces()
    {
        string validInstruction = " L MM R M ";
        List<SingularInstruction> expectedResult = new()
        {
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward
        };

        List<SingularInstruction> actualResult = standardInstructionReader.EvaluateInstruction(validInstruction);
        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public void ExampleInstructionString_Should_Return_LMRMMRM()
    {
        standardInstructionReader.ExampleInstructionString.Should().Be("LMRMMRM");
    }
}
