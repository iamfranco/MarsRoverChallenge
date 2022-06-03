using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Plateaus;

namespace MarsRover.AppUI.Components;

internal static class AppSectionPlateau
{
    public static PlateauBase AskForPlateau(
        AppController appController, Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        Func<PlateauBase> selectedPlateauMaker = MakerMenu.AskUserToSelectMaker(
            groupName: "plateau",
            makers: plateauMakers);

        PlateauBase plateau = AskUserHelpers.ExecuteUntilNoException(selectedPlateauMaker);
        appController.ConnectPlateau(plateau);
        AskUserHelpers.ClearScreenAndPrintMap(appController);
        return plateau;
    }
}
