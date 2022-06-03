using MarsRover.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Helpers;
internal class InputReaderForTest : InputReader
{
    private readonly List<string> _inputs = new();
    private int index = 0;

    public InputReaderForTest(List<string> inputs)
    {
        if (inputs is null)
            throw new ArgumentNullException(nameof(inputs));

        if (inputs.Count == 0)
            throw new ArgumentException($"{nameof(inputs)} cannot be empty");

        _inputs = inputs;
    }

    public override string GetUserInput(string prompt) => _inputs[index++];
}
