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
    public void InputReaderForTest_Construct_With_Null_KeyInfos_Should_Throw_Exception()
    {
        List<string> inputs = new() { "some input" };
        InputReaderForTest inputReader;

        Action act = () => inputReader = new InputReaderForTest(inputs, null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void SetInputReader_With_Null_Input_Should_Throw_Exception()
    {
        Action act = () => InputReaderContainer.SetInputReader(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void GetUserInput_Should_Return_UserInputs_Correctly_In_Correct_Order()
    {
        List<string> inputs = new() { "input 1", "2", "three", "IV" };
        InputReaderForTest inputReader = new InputReaderForTest(inputs);
        InputReaderContainer.SetInputReader(inputReader);

        foreach (string input in inputs)
        {
            InputReaderContainer.GetUserInput("Enter something: ").Should().Be(input);
        }
    }

    [Test]
    public void ReadKey_With_Keys_Set_Should_Return_Keys_In_Correct_Order()
    {
        List<string> userInputs = new();
        List<ConsoleKeyInfo> keyInfos = new()
        {
            new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false),
            new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false)
        };

        InputReaderForTest inputReader = new InputReaderForTest(userInputs, keyInfos);
        InputReaderContainer.SetInputReader(inputReader);

        foreach (ConsoleKeyInfo keyInfo in keyInfos)
        {
            InputReaderContainer.ReadKey().Should().Be(keyInfo);
        }
    }

    [Test]
    public void GetUserInput_And_ReadKey_Mixing_Together_Should_Return_UserInputs_And_KeyInfos_Correctly_In_Correct_Order()
    {
        List<string> userInputs = new() { "input 1", "second input", "3" };
        List<ConsoleKeyInfo> keyInfos = new()
        {
            new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false),
            new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
            new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false)
        };

        InputReaderForTest inputReader = new InputReaderForTest(userInputs, keyInfos);
        InputReaderContainer.SetInputReader(inputReader);

        for (int i = 0; i < userInputs.Count; i++)
        {
            var userInput = userInputs[i];
            var keyInfo = keyInfos[i];

            InputReaderContainer.GetUserInput("Enter something: ").Should().Be(userInput);
            InputReaderContainer.ReadKey().Should().Be(keyInfo);
        }
    }
}
