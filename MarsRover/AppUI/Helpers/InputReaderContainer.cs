namespace MarsRover.AppUI.Helpers;
public static class InputReaderContainer
{
    private static InputReader _inputReader = new();

    public static void SetInputReader(InputReader inputReader) => _inputReader = inputReader;

    public static string GetUserInput(string prompt) => _inputReader.GetUserInput(prompt);
}
