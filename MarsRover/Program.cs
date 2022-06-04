using MarsRover.AppUI;
using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

IPositionStringConverter positionStringConverter = new StandardPositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
MapPrinter mapPrinter = new MapPrinter();

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

AppController appController = new(instructionReader);
AppUIHandler appUIHandler = new(positionStringConverter, appController, mapPrinter);

ConsoleApp.Run(appUIHandler, plateauMakers, vehicleMakers);
