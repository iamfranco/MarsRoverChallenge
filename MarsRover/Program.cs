using MarsRover.AppUI;
using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

IPositionStringConverter positionStringConverter = new StandardPositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
MapPrinter mapPrinter = new MapPrinter();
CommandHandler commandHandler = new(instructionReader, positionStringConverter, mapPrinter);

Dictionary<string, Func<PlateauBase>> plateauMakers = new()
{
    {
        "Rectangular Plateau", () =>
        {
            string maximumCoordinatesString = AskUserHelpers.AskUntilValidStringInput(
                $"Enter Maximum Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                positionStringConverter.IsValidCoordinateString);

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
            return new RectangularPlateau(maximumCoordinates);
        }
    },
    {
        "Circular Plateau", () =>
        {
            string radiusString = AskUserHelpers.AskUntilValidStringInput(
                $"Enter Radius (eg \"5\"): ",
                s => int.TryParse(s, out _));

            return new CircularPlateau(int.Parse(radiusString));
        }
    },
};

Dictionary<string, Func<Position, VehicleBase>> vehicleMakers = new()
{
    { "Rover", position => new Rover(position) },
    { "Wall E", position => new WallE(position) }
};

PlateauBase plateau = AskUser.AskUserToMakePlateau(commandHandler, plateauMakers);
AskUser.AskUserToMakeObstacles(positionStringConverter, commandHandler, plateau);

while (true)
{
    AskUser.AskUserToCreateNewVehicleOrConnectToExistingVehicle(positionStringConverter, commandHandler, plateau, vehicleMakers);
    AskUser.AskUserForMovementInstructionAndSendToVehicle(instructionReader, commandHandler, plateau);

    Console.WriteLine();
    Console.Write("Press any key to continue.. ");
    Console.ReadKey();
}
