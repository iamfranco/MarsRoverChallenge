
using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

IPositionStringConverter positionStringConverter = new PositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
CommandHandler commandHandler = new(instructionReader, positionStringConverter);

Dictionary<string, Func<PlateauBase>> plateauMaker = new()
{
    {
        "Rectangular Plateau", 
        () =>
        {
            string maximumCoordinatesString = AskUser.AskUntilValidStringInput(
                $"Enter Maximum Coordinates (eg \"{positionStringConverter.ExampleCoordinateString}\"): ",
                positionStringConverter.IsValidCoordinateString);

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates(maximumCoordinatesString);
            return new RectangularPlateau(maximumCoordinates);
        }
    },
    {
        "Square Plateau",
        () =>
        {
            string squareLength = AskUser.AskUntilValidStringInput(
                $"Enter Maximum X Coordinate of square (eg \"5\"): ",
                s => int.TryParse(s, out _));

            Coordinates maximumCoordinates = positionStringConverter.ToCoordinates($"{squareLength} {squareLength}");
            return new RectangularPlateau(maximumCoordinates);
        }
    },
};



PlateauBase plateau = AskUser.AskUserToMakePlateau(commandHandler, plateauMaker);
AskUser.AskUserToMakeObstacles(positionStringConverter, commandHandler, plateau);

while (true)
{
    AskUser.AskUserToCreateNewVehicleOrConnectToExistingVehicle(positionStringConverter, commandHandler, plateau);
    AskUser.AskUserForMovementInstructionAndSendToVehicle(instructionReader, commandHandler, plateau);

    Console.WriteLine();
    Console.Write("Press any key to continue.. ");
    Console.ReadKey();
}