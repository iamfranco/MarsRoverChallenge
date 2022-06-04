using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Plateaus;

namespace MarsRover.AppUI.Components;

public static class AppSectionPlateau
{
    public static void AskForPlateau(
        AppController appController, Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        if (appController is null)
            throw new ArgumentNullException(nameof(appController));

        if (plateauMakers is null)
            throw new ArgumentNullException(nameof(plateauMakers));

        Func<PlateauBase> selectedPlateauMaker = MakerMenu.AskUserToSelectMaker(
            groupName: "plateau",
            makers: plateauMakers);

        PlateauBase plateau = AppUIHelpers.ExecuteUntilNoException(selectedPlateauMaker);
        appController.ConnectPlateau(plateau);
    }
}
