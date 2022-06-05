using MarsRover.AppUI;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.MapPrinters;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;

IPositionStringConverter positionStringConverter = new StandardPositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
IMapPrinter mapPrinter = new MapPrinter();

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
};

Dictionary<string, Func<Position, VehicleBase>> vehicleMakers = new()
{
    { "Rover", position => new Rover(position) },
};

AppController appController = new(instructionReader);
AppUIHandler appUIHandler = new(positionStringConverter, appController, mapPrinter);

ConsoleApp.Run(appUIHandler, plateauMakers, vehicleMakers);
