﻿using MarsRover.Models.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;
public static class ConsoleApp
{
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

        try
        {
            appUIHandler.AskUserToMakePlateau(plateauMakers);
            appUIHandler.AskUserToMakeObstacles();

            while (true)
            {
                appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(vehicleMakers);
                appUIHandler.AskUserForMovementInstructionAndSendToVehicle();

                Console.WriteLine();

                Console.Write("Press [Q] key to quit, any other key to continue.. ");
                var a = Console.ReadKey();
                if (a.KeyChar.ToString().ToLower() == "q")
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }
}
