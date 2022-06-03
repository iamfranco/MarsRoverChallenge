using MarsRover.AppUI.Components;
using MarsRover.Controllers;

namespace MarsRover.AppUI.Helpers;

public static class AppUIHelpers
{
    public static ReturnType ExecuteUntilNoException<ReturnType>(Func<ReturnType> func)
    {
        while (true)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  WARNING: {ex.Message}");
            }
        }
    }

    public static void ExecuteUntilNoException(Action act) => ExecuteUntilNoException(() => { act(); return true; });

    public static string AskUntilValidStringInput(string prompt, Func<string, bool> validationFunc)
    {
        return ExecuteUntilNoException(() =>
        {
            var input = GetUserInput(prompt);

            if (!validationFunc(input))
                throw new Exception($"Input [{input}] is not in correct format");

            return input;
        });
    }

    public static int AskUntilValidIntInput(string prompt, int minValue, int maxValue)
    {
        return ExecuteUntilNoException(() =>
        {
            var input = GetUserInput(prompt);

            if (!int.TryParse(input, out var num))
                throw new Exception($"Input [{input}] needs to be integer");

            if (num < minValue)
                throw new Exception($"Input [{input}] cannot be below {minValue}");

            if (num > maxValue)
                throw new Exception($"Input [{input}] cannot be above {maxValue}");

            return num;
        });
    }

    public static void ClearScreenAndPrintMap(AppController appController, MapPrinter mapPrinter)
    {
        Console.Clear();
        Console.WriteLine("ctrl-C to exit");

        mapPrinter.PrintMap(appController);

        Console.WriteLine();
    }

    public static string GetUserInput(string prompt)
    {
        Console.WriteLine();
        Console.Write(prompt);
        var input = Console.ReadLine();

        if (input is null)
            throw new Exception($"Input cannot be null");

        return input;
    }
}
