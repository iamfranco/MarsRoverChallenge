using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;

public static class AskUser
{
    public static PlateauBase AskUserToMakePlateau(CommandHandler commandHandler,
        Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        return AskUserHelpers.ExecuteUntilNoException(() =>
        {
            Func<PlateauBase> selectedPlateauMaker = MakerMenu.AskUserToSelectMaker(
                groupName: "plateau", 
                makers: plateauMakers);

            PlateauBase plateau = AskUserHelpers.ExecuteUntilNoException(selectedPlateauMaker);
            commandHandler.ConnectPlateau(plateau);
            ClearScreenAndPrintMap(commandHandler);
            return plateau;
        });
    }

    public static void AskUserToMakeObstacles(IPositionStringConverter positionStringConverter,
        CommandHandler commandHandler, PlateauBase plateau)
    {
        while (true)
        {
            try
            {
                string obstacleCoordinates = AskUserHelpers.AskUntilValidStringInput($"Enter Obstacle Coordinate " +
                    $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
                    (s) => string.IsNullOrEmpty(s) || positionStringConverter.IsValidCoordinateString(s));

                if (string.IsNullOrEmpty(obstacleCoordinates))
                    break;

                plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
                ClearScreenAndPrintMap(commandHandler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public static void AskUserToCreateNewVehicleOrConnectToExistingVehicle(
        IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        commandHandler.ResetRecentPath();
        ClearScreenAndPrintMap(commandHandler);

        AskUserHelpers.ExecuteUntilNoException(() =>
        {
            string positionOrCoordinatesString = AskForPositionOrCoordinatesString(positionStringConverter);

            if (positionStringConverter.IsValidPositionString(positionOrCoordinatesString))
            {
                CreateNewVehicleAtPosition(positionOrCoordinatesString);
            }
            else
            {
                Coordinates initialCoordinates = positionStringConverter.ToCoordinates(positionOrCoordinatesString);
                commandHandler.ConnectToVehicleAtCoordinates(initialCoordinates);
            }
            return true;
        });

        ClearScreenAndPrintMap(commandHandler);
        Console.WriteLine($"Connected to [{commandHandler.GetVehicle()!.GetType().Name}] " +
            $"at [{commandHandler.GetPositionString()}]");

        void CreateNewVehicleAtPosition(string positionOrCoordinatesString)
        {
            Position initialPosition = positionStringConverter.ToPosition(positionOrCoordinatesString);

            if (!plateau.IsCoordinateValidInPlateau(initialPosition.Coordinates))
            {
                throw new ArgumentException($"{positionOrCoordinatesString} is on " +
                    $"Coordinates {initialPosition.Coordinates}, which is not valid on Plateau");
            }

            var vehicleWithKnownPositionMakers = MakerMenu.GetMakersWithKnownPosition(vehicleMakers, initialPosition);

            Func<VehicleBase> selectedVehicleMaker = MakerMenu.AskUserToSelectMaker(
                groupName: "vehicle",
                makers: vehicleWithKnownPositionMakers);

            VehicleBase vehicle = AskUserHelpers.ExecuteUntilNoException(selectedVehicleMaker);
            commandHandler.AddVehicleToPlateau(vehicle);
        }
    }

    public static void AskUserForMovementInstructionAndSendToVehicle(
        IInstructionReader instructionReader, CommandHandler commandHandler, PlateauBase plateau)
    {
        string instructionString = AskUserHelpers.AskUntilValidStringInput(
            $"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
            instructionReader.IsValidInstruction);

        string message = commandHandler.SendMoveInstruction(instructionString);

        ClearScreenAndPrintMap(commandHandler);
        Console.WriteLine(message);
    }

    private static string AskForPositionOrCoordinatesString(IPositionStringConverter positionStringConverter)
    {
        return AskUserHelpers.AskUntilValidStringInput(
            $"Enter Position (eg \"{positionStringConverter.ExamplePositionString}\") to add new Vehicle, or " +
            $"\nEnter Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\") to connect with existing vehicle: ",
            (input) => positionStringConverter.IsValidPositionString(input) || positionStringConverter.IsValidCoordinateString(input));
    }

    private static void ClearScreenAndPrintMap(CommandHandler commandHandler)
    {
        Console.Clear();
        Console.WriteLine("ctrl-C to exit");

        commandHandler.MapPrinter.PrintMap(commandHandler);

        Console.WriteLine();
    }
}
