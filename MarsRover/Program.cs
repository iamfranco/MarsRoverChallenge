using MarsRover.AppUI;
using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

IPositionStringConverter positionStringConverter = new StandardPositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
MapPrinter mapPrinter = new MapPrinter();
AppController appController = new(instructionReader);
AppUIHandler appUIHandler = new(positionStringConverter, appController, mapPrinter);

Dictionary<string, Func<PlateauBase>> plateauMakers = new()
{
    {
        "Rectangular Plateau", () =>
        {
            string maximumCoordinatesString = AppUIHelpers.AskUntilValidStringInput(
                $"Enter Maximum Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                positionStringConverter.IsValidCoordinateString);

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
            return new RectangularPlateau(maximumCoordinates);
        }
    },
    {
        "Circular Plateau", () =>
        {
            string radiusString = AppUIHelpers.AskUntilValidStringInput(
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

PlateauBase plateau = appUIHandler.AskUserToMakePlateau(plateauMakers);
appUIHandler.AskUserToMakeObstacles(plateau);

while (true)
{
    appUIHandler.AskUserToCreateNewVehicleOrConnectToExistingVehicle(plateau, vehicleMakers);
    appUIHandler.AskUserForMovementInstructionAndSendToVehicle();

    Console.WriteLine();
    Console.Write("Press any key to continue.. ");
    Console.ReadKey();
}
