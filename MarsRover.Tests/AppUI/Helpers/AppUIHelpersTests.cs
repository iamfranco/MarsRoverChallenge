using MarsRover.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Helpers;
internal class AppUIHelpersTests
{
    [Test]
    public void ExecuteUntilNoException_With_Func_That_Throws_No_Exception_Should_Return_Func_Return_Value()
    {
        string output = "some output";
        string func() => output;

        string actualResult = AppUIHelpers.ExecuteUntilNoException(func);
        actualResult.Should().Be(output);

        int output2 = 42;
        int func2() => output2;

        int actualResult2 = AppUIHelpers.ExecuteUntilNoException(func2);
        actualResult2.Should().Be(output2);
    }

    [Test]
    public void ExecuteUntilNoException_With_Func_That_Throws_Exception_Should_Repeat_Until_No_Exception()
    {
        int count = 2;
        string func()
        {
            if (count > 0)
            {
                count--;
                throw new Exception($"count is {count}");
            }
            return "no more exception";
        }

        string actualResult = AppUIHelpers.ExecuteUntilNoException(func);

        actualResult.Should().Be("no more exception");
        count.Should().Be(0);
    }

    [Test]
    public void ExecuteUntilNoException_With_Action_That_Throws_No_Exception_Should_Perform_Action()
    {
        int count = 0;
        void act()
        {
            count = 10;
        }

        AppUIHelpers.ExecuteUntilNoException(act);

        count.Should().Be(10);
    }

    [Test]
    public void ExecuteUntilNoException_With_Action_That_Throws_Exception_Should_Repeat_Until_No_Exception()
    {
        int count = 2;
        void act()
        {
            if (count > 0)
            {
                count--;
                throw new Exception($"count is {count}");
            }
        }

        AppUIHelpers.ExecuteUntilNoException(act);

        count.Should().Be(0);
    }

    [Test]
    public void AskUntilValidStringInput_Should_Return_First_UserInput_That_Returns_True_For_ValidationFunc()
    {
        string firstValidUserInput = "someInput";
        List<string> inputs = new() { "asjdkldsjf", "123", firstValidUserInput, "__A__", "sssssss"};
        bool validationFunc(string input) => input.StartsWith("s");

        InputReaderContainer.SetInputReader(new InputReaderForTest(inputs));

        string actualResult = AppUIHelpers.AskUntilValidStringInput("some prompt: ", validationFunc);

        actualResult.Should().Be(firstValidUserInput);
    }

    [Test]
    public void AskUntilValidIntInput_Should_Return_First_UserInput_That_Is_Between_MinValue_MaxValue_In_Integer()
    {
        string firstValidUserInput = "5";
        List<string> inputs = new() { "-10", "-5", firstValidUserInput, "20", "8" };

        InputReaderContainer.SetInputReader(new InputReaderForTest(inputs));

        int actualResult = AppUIHelpers.AskUntilValidIntInput("some prompt: ", minValue: 0, maxValue: 10);

        actualResult.Should().Be(int.Parse(firstValidUserInput));
    }
}
