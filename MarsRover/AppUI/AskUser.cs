using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI
{
    public static class AskUser
    {
        public static PlateauBase AskUserToMakePlateau(CommandHandler commandHandler, Dictionary<string, Func<PlateauBase>> plateauMakers)
        {
            return ExecuteUntilNoException(func);

            PlateauBase func()
            {
                Func<PlateauBase> selectedPlateauMaker = AskUserToSelectMaker(
                    groupName: "plateau",
                    makers: plateauMakers);

                PlateauBase plateau = ExecuteUntilNoException(selectedPlateauMaker);
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

        public static void AskUserToCreateNewVehicleOrConnectToExistingVehicle(
            IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau,
            Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
        {
            commandHandler.ResetRecentPath();
            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);

            ExecuteUntilNoException(() =>
            {
                string positionOrCoordinatesString = AskForPositionOrCoordinateasString(positionStringConverter);

                if (positionStringConverter.IsValidPositionString(positionOrCoordinatesString))
                {
                    CreateNewVehicleAtPosition(positionStringConverter, commandHandler, plateau, vehicleMakers, positionOrCoordinatesString);
                }
                else
                {
                    Coordinates initialCoordinates = positionStringConverter.ToCoordinates(positionOrCoordinatesString);
                    commandHandler.ConnectToVehicleAtCoordinates(initialCoordinates);
                }
                return true;
            });

            ClearScreenAndPrintMap(plateau, commandHandler.RecentPath);
            Console.WriteLine(commandHandler.GetVehicle()!.GetType());

            static Dictionary<string, Func<VehicleBase>> GetVehicleWithKnownPositionMakers(
                Dictionary<string, Func<Position, VehicleBase>> vehicleMakers,
                Position initialPosition)
            {
                Dictionary<string, Func<VehicleBase>> vehicleMakersWithKnownPosition = new();

                var keyValueCollection = vehicleMakers
                    .Select(item => new KeyValuePair<string, Func<VehicleBase>>(item.Key, () => item.Value(initialPosition)));

                foreach (var keyValue in keyValueCollection)
                {
                    vehicleMakersWithKnownPosition[keyValue.Key] = keyValue.Value;
                }

                return vehicleMakersWithKnownPosition;
            }

            static void CreateNewVehicleAtPosition(IPositionStringConverter positionStringConverter, CommandHandler commandHandler, PlateauBase plateau, Dictionary<string, Func<Position, VehicleBase>> vehicleMakers, string positionOrCoordinatesString)
            {
                Position initialPosition = positionStringConverter.ToPosition(positionOrCoordinatesString);

                if (!plateau.IsCoordinateValidInPlateau(initialPosition.Coordinates))
                    throw new ArgumentException($"{positionOrCoordinatesString} is on Coordinates {initialPosition.Coordinates}, which is not valid on Plateau");

                Dictionary<string, Func<VehicleBase>> vehicleWithKnownPositionMakers = GetVehicleWithKnownPositionMakers(vehicleMakers, initialPosition);

                Func<VehicleBase> selectedVehicleMaker = AskUserToSelectMaker(
                    groupName: "vehicle",
                    makers: vehicleWithKnownPositionMakers);

                VehicleBase vehicle = ExecuteUntilNoException(selectedVehicleMaker);
                commandHandler.AddVehicleToPlateau(vehicle);
            }
        }

        public static void AskUserForMovementInstructionAndSendToVehicle(IInstructionReader instructionReader, CommandHandler commandHandler, PlateauBase plateau)
        {
            string instructionString = AskUntilValidStringInput($"Enter Movement Instruction (eg \"{instructionReader.ExampleInstructionString}\"): ",
                instructionReader.IsValidInstruction);

            string message = commandHandler.SendMoveInstruction(instructionString);

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
                    Console.WriteLine($"\n  WARNING: {ex.Message}");
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

        private static Func<MakerReturnerType> AskUserToSelectMaker<MakerReturnerType>(string groupName, Dictionary<string, Func<MakerReturnerType>> makers)
        {
            List<string> names = makers.Keys.ToList();
            Func<MakerReturnerType> selectedMaker = makers[names[0]];
            if (names.Count > 1)
            {
                PrintAllAvailableNames(names, groupName);
                selectedMaker = AskUserToSelectTypeToMake(makers, names, groupName);
            }

            return selectedMaker;

            static void PrintAllAvailableNames(List<string> names, string groupName)
            {
                Console.WriteLine($"All available {groupName} types: ");
                for (int i = 0; i < names.Count; i++)
                    Console.WriteLine($"  {i + 1} - {names[i]}");
            }

            static Func<MakerReturnerType> AskUserToSelectTypeToMake(Dictionary<string, Func<MakerReturnerType>> makerFunc, List<string> names, string groupName)
            {
                int selectedNum = AskUntilValidIntInput(
                    $"Enter a number to select a {groupName} (number between 1 and {names.Count}): ",
                    minValue: 1, maxValue: names.Count);

                string selectedName = names[selectedNum - 1];
                return makerFunc[selectedName];
            }
        }
    }
}
