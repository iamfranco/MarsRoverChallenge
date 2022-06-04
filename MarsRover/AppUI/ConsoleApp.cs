using MarsRover.Models.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;
public static class ConsoleApp
{
    public static void Run(AppUIHandler appUIHandler,
        Dictionary<string, Func<PlateauBase>> plateauMakers,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        try
        {
            appUIHandler.AskUserToMakePlateau(plateauMakers);
            appUIHandler.AskUserToMakeObstacles();

            while (true)
            {
                appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(vehicleMakers);
                appUIHandler.AskUserForMovementInstructionAndSendToVehicle();

                Console.WriteLine();
                Console.Write("Press any key to continue.. ");
                Console.ReadKey();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }
}
