using MarsRover.AppUI.Helpers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;
public static class ConsoleApp
{
    private static readonly ConsoleKeyInfo _qKeyInfo = new('q', ConsoleKey.Q, false, false, false);

    public static void Run(AppUIHandler appUIHandler,
        Dictionary<string, Func<PlateauBase>> plateauMakers,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        if (appUIHandler is null)
            throw new ArgumentNullException(nameof(appUIHandler));

        if (plateauMakers is null)
            throw new ArgumentNullException(nameof(plateauMakers));

        if (vehicleMakers is null)
            throw new ArgumentNullException(nameof(vehicleMakers));

        if (!plateauMakers.Any())
            throw new ArgumentException($"{nameof(plateauMakers)} cannot be empty");

        if (!vehicleMakers.Any())
            throw new ArgumentException($"{nameof(vehicleMakers)} cannot be empty");

        appUIHandler.AskUserToMakePlateau(plateauMakers);
        appUIHandler.AskUserToMakeObstacles();

        while (true)
        {
            appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(vehicleMakers);
            appUIHandler.AskUserForMovementInstructionAndSendToVehicle();

            Console.WriteLine();

            Console.Write("Press [q] key to quit, press any other key to continue.. ");
            ConsoleKeyInfo keyInfo = InputReaderContainer.ReadKey();
            if (keyInfo == _qKeyInfo)
                break;
        }
    }
}
