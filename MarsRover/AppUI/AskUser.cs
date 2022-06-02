using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI
{
    public static class AskUser
    {
        public static PlateauBase AskUserToMakePlateau(CommandHandler commandHandler, Dictionary<string, Func<PlateauBase>> plateauMaker)
        {
            List<string> plateauNames = plateauMaker.Keys.ToList();
            
            return ExecuteUntilNoException(func);

            PlateauBase func()
            {
                Func<PlateauBase> selectedPlateauMaker = plateauMaker[plateauNames[0]];
                if (plateauNames.Count > 1)
                {
                    PrintAllAvailablePlateauTypes(plateauNames);
                    selectedPlateauMaker = AskUserToSelectPlateau(plateauMaker, plateauNames);
                }

                PlateauBase plateau = ExecuteUntilNoException(selectedPlateauMaker);
                commandHandler.ConnectPlateau(plateau);
                ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
                return plateau;
            }

            static void PrintAllAvailablePlateauTypes(List<string> plateauNames)
            {
                Console.WriteLine("Available Plateau Shapes: ");
                for (int i = 0; i < plateauNames.Count; i++)
                    Console.WriteLine($"  {i + 1} - {plateauNames[i]}");
            }

            static Func<PlateauBase> AskUserToSelectPlateau(Dictionary<string, Func<PlateauBase>> plateauMaker, List<string> plateauNames)
            {
                int selectedNum = AskUntilValidIntInput(
                                        $"\nEnter a number to select the plateau shape (number between 1 and {plateauNames.Count}): ",
                                        minValue: 1, maxValue: plateauNames.Count);

                string selectedPlateauName = plateauNames[selectedNum - 1];
                return plateauMaker[selectedPlateauName];
            }
        }

        public static void AskUserToMakeObstacles(IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau)
        {
            while (true)
            {
                try
                {
                    string obstacleCoordinates = AskUntilValidStringInput($"Enter Obstacle Coordinate " +
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
            string instructionString = AskUntilValidStringInput($"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
                instructionReader.IsValidInstruction);

            (bool status, string message) = commandHandler.SendMoveInstruction(instructionString);

            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
            Console.WriteLine(message);
        }

        public static string AskUntilValidStringInput(string prompt, Func<string, bool> validationFunc)
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

        private static int AskUntilValidIntInput(string prompt, int minValue, int maxValue)
        {
            return ExecuteUntilNoException(func);

            int func()
            {
                Console.WriteLine();
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (input is null)
                    throw new Exception($"Input cannot be null");

                if (!int.TryParse(input, out int num))
                    throw new Exception($"Input [{input}] needs to be integer");

                if (num < minValue)
                    throw new Exception($"Input [{input}] cannot be below {minValue}");

                if (num > maxValue)
                    throw new Exception($"Input [{input}] cannot be above {maxValue}");

                return num;
            }
        }

        private static ReturnType ExecuteUntilNoException<ReturnType>(Func<ReturnType> func)
        {
            while (true)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static string AskForPositionOrCoordinateasString(IPositionStringConverter positionStringConverter)
        {
            return AskUntilValidStringInput(
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
