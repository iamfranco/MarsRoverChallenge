using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

IPositionStringConverter positionStringConverter = new PositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
CommandHandler commandHandler = new(instructionReader, positionStringConverter);

Dictionary<string, Func<PlateauBase>> plateauMakers = new()
{
    {
        "Rectangular Plateau", () =>
        {
            string maximumCoordinatesString = AskUser.AskUntilValidStringInput(
                $"Enter Maximum Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                positionStringConverter.IsValidCoordinateString);

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
            return new RectangularPlateau(maximumCoordinates);
        }
    },
    {
        "Square Plateau", () =>
        {
            string squareLength = AskUser.AskUntilValidStringInput(
                $"Enter Maximum X Coordinate of square (eg \"5\"): ",
                s => int.TryParse(s, out _));

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates($"{squareLength} {squareLength}");
            return new RectangularPlateau(maximumCoordinates);
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