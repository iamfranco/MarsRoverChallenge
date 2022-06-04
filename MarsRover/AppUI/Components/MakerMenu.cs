using System.Runtime.CompilerServices;
using MarsRover.AppUI.Helpers;
using MarsRover.Models.Elementals;

namespace MarsRover.AppUI.Components;
public static class MakerMenu
{
    public static Func<MakerReturnType> AskUserToSelectMaker<MakerReturnType>(string groupName,
        Dictionary<string, Func<MakerReturnType>> makers, [CallerArgumentExpression("makers")] string makersCallerName = "")
    {
        if (!makers.Any())
            throw new ArgumentException($"{makersCallerName} cannot be empty");

        var names = makers.Keys.ToList();
        var selectedMaker = makers[names[0]];
        if (names.Count > 1)
        {
            PrintAllAvailableNames(names, groupName);
            selectedMaker = AskUserToSelectMakerByNumber(makers, names, groupName);
        }
        return selectedMaker;
    }

    public static Dictionary<string, Func<T>> GetMakersWithKnownPosition<T>(
        Dictionary<string, Func<Position, T>> makers, Position position)
    {
        Dictionary<string, Func<T>> makersWithKnownPosition = new();

        var keyValueCollection = makers
            .Select(item => new KeyValuePair<string, Func<T>>(item.Key, () => item.Value(position)));

        foreach (var keyValue in keyValueCollection)
        {
            makersWithKnownPosition[keyValue.Key] = keyValue.Value;
        }

        return makersWithKnownPosition;
    }

    private static void PrintAllAvailableNames(List<string> names, string groupName)
    {
        Console.WriteLine($"All available {groupName} types: ");
        for (var i = 0; i < names.Count; i++)
            Console.WriteLine($"  {i + 1} - {names[i]}");
    }

    private static Func<MakerReturnType> AskUserToSelectMakerByNumber<MakerReturnType>(Dictionary<string,
        Func<MakerReturnType>> makerFunc, List<string> names, string groupName)
    {
        int selectedNum = AppUIHelpers.AskUntilValidIntInput(
            $"Enter a number to select a {groupName} (number between 1 and {names.Count}): ",
            minValue: 1, maxValue: names.Count);

        var selectedName = names[selectedNum - 1];
        return makerFunc[selectedName];
    }
}
