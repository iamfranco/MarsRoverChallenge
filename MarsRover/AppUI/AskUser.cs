using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI
{
    public static class AskUser
    {
        public static RectangularPlateau AskUserToMakePlateau(IPositionStringConverter positionStringConverter, CommandHandler commandHandler)
        {
            return ExecuteUntilNoException(func);

            RectangularPlateau func()
            {
                RectangularPlateau plateau;

                string maximumCoordinatesString = AskUntilValidInput($"Enter Maximum Coordinates " +
                        $"(eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                        positionStringConverter.IsValidCoordinateString);

                Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
                plateau = new(maximumCoordinates);
                commandHandler.ConnectPlateau(plateau);

                ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
                return plateau;
            }
        }

        public static void AskUserToMakeObstacles(IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau)
        {
            while (true)
            {
                try
                {
                    string obstacleCoordinates = AskUntilValidInput($"Enter Obstacle Coordinate " +
                        $"(eg \"{positionStringConverter.ExampleCoordinateString}\", or empty if no more obstacle): ",
                        (s) => string.IsNullOrEmpty(s) || positionStringConverter.IsValidCoordinateString(s));

                    if (string.IsNullOrEmpty(obstacleCoordinates))
                        break;

                    plateau.ObstaclesContainer.AddObstacle(positionStringConverter.ToCoordinates(obstacleCoordinates));
                    ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void AskUserToCreateNewVehicleOrConnectToExistingVehicle(IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau)
        {
            commandHandler.ResetRecentPath();
            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);

            bool isConnectedToVehicle;
            string message;
            while (true)
            {
                string positionOrCoordinatesString = AskForPositionOrCoordinateasString(positionStringConverter);

                if (positionStringConverter.IsValidPositionString(positionOrCoordinatesString))
                {
                    // create new vehicle at position 
                    Position initialPosition = positionStringConverter.ToPosition(positionOrCoordinatesString);
                    VehicleBase vehicle = new Rover(initialPosition);
                    (isConnectedToVehicle, message) = commandHandler.AddVehicleToPlateau(vehicle);
                }
                else
                {
                    // connect to existing vehicle at coordinates
                    Coordinates initialCoordinates = positionStringConverter.ToCoordinates(positionOrCoordinatesString);
                    (isConnectedToVehicle, message) = commandHandler.ConnectToVehicleAtCoordinates(initialCoordinates);
                }

                if (!isConnectedToVehicle)
                {
                    Console.WriteLine(message);
                    continue;
                }

                break;
            }
            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
            Console.WriteLine(message);
        }

        public static void AskUserForMovementInstructionAndSendToVehicle(IInstructionReader instructionReader, CommandHandler commandHandler, PlateauBase plateau)
        {
            string instructionString = AskUntilValidInput($"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
                instructionReader.IsValidInstruction);

            (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
            Console.WriteLine(message);
        }

        private static ReturnType ExecuteUntilNoException<ReturnType>(Func<ReturnType> func)
        {
            while (true)
            {
                try
                {
                    func();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static string AskUntilValidInput(string prompt, Func<string, bool> validationFunc)
        {
            return ExecuteUntilNoException(func);

            string func()
            {
                Console.WriteLine();
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (input is null)
                    throw new Exception($"Input cannot be null");

                if (!validationFunc(input))
                    throw new Exception($"Input [{input}] is not in correct format");

                return input;
            }
        }

        private static string AskForPositionOrCoordinateasString(IPositionStringConverter positionStringConverter)
        {
            return AskUntilValidInput(
                        $"Enter Position (eg \"{positionStringConverter.ExamplePositionString}\") to add new Vehicle, or " +
                        $"\nEnter Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\") to connect with existing vehicle: ",
                        (input) => positionStringConverter.IsValidPositionString(input) || positionStringConverter.IsValidCoordinateString(input));
        }

        private static void ClearScreenAndPrintMap(PlateauBase plateau, List<Position> recentPath)
        {
            Console.Clear();
            Console.WriteLine("ctrl-C to exit");

            try
            {
                plateau.PrintMap(recentPath);
            }
            catch
            { }

            Console.WriteLine();
        }
    }
}
