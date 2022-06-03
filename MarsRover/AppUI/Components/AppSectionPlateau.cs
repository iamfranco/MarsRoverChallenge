using MarsRover.AppUI.Helpers;
using MarsRover.Models.Plateaus;

namespace MarsRover.AppUI.Components;

internal static class AppSectionPlateau
{
    public static PlateauBase AskForPlateau(
        CommandHandler commandHandler, Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        Func<PlateauBase> selectedPlateauMaker = MakerMenu.AskUserToSelectMaker(
            groupName: "plateau",
            makers: plateauMakers);

        PlateauBase plateau = AskUserHelpers.ExecuteUntilNoException(selectedPlateauMaker);
        commandHandler.ConnectPlateau(plateau);
        AskUserHelpers.ClearScreenAndPrintMap(commandHandler);
        return plateau;
    }
}
