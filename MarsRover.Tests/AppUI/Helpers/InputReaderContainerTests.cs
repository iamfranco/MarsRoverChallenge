using MarsRover.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Helpers;
internal class InputReaderContainerTests
{
    [Test]
    public void InputReaderForTest_Construct_With_Null_Input_Should_Throw_Exception()
    {
        InputReaderForTest inputReader;
        Action act = () => inputReader = new InputReaderForTest(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void InputReaderForTest_Construct_With_Empty_List_Input_Should_Throw_Exception()
    {
        List<string> inputs = new();
        InputReaderForTest inputReader;

        Action act = () => inputReader = new InputReaderForTest(inputs);

        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void SetInputReader_With_Null_Input_Should_Throw_Exception()
    {
        Action act = () => InputReaderContainer.SetInputReader(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void GetUserInput_Should_Return_Input_In_Correct_Order()
    {
        List<string> inputs = new() { "input 1", "2", "three", "IV" };
        InputReaderForTest inputReader = new InputReaderForTest(inputs);
        InputReaderContainer.SetInputReader(inputReader);

        foreach (string input in inputs)
        {
            InputReaderContainer.GetUserInput("Enter something: ").Should().Be(input);
        }
    }
}
