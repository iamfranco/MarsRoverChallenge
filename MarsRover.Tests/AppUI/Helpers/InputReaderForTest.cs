using MarsRover.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Helpers;
internal class InputReaderForTest : InputReader
{
    private readonly List<string> _inputs = new();
    private int _inputsIndex = 0;

    private readonly List<ConsoleKeyInfo> _keyInfos = new();
    private int _keyInfosIndex = 0;

    public InputReaderForTest(List<string> inputs) : this(inputs, new())
    {
    }

    public InputReaderForTest(List<string> inputs, List<ConsoleKeyInfo> keyInfos)
    {
        if (inputs is null)
            throw new ArgumentNullException(nameof(inputs));

        if (keyInfos is null)
            throw new ArgumentNullException(nameof(keyInfos));

        _inputs = inputs;
        _keyInfos = keyInfos;
    }

    public override string GetUserInput(string prompt) => _inputs[_inputsIndex++];
    public override ConsoleKeyInfo ReadKey() => _keyInfos[_keyInfosIndex++];
}
