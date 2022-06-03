using MarsRover.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Helpers;
internal class UserInputReaderForTest : InputReader
{
    private readonly List<string> _inputs = new();
    private int index = 0;

    public UserInputReaderForTest(List<string> inputs)
    {
        _inputs = inputs;
    }

    public override string GetUserInput(string prompt) => _inputs[index++];
}
