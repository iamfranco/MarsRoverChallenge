
using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

IPositionStringConverter positionStringConverter = new PositionStringConverter();
IInstructionReader instructionReader = new StandardInstructionReader();
CommandHandler commandHandler = new(instructionReader, positionStringConverter);
RectangularPlateau plateau;

plateau = AskUser.AskUserToMakePlateau(positionStringConverter, commandHandler);
AskUser.AskUserToMakeObstacles(positionStringConverter, commandHandler, plateau);

while (true)
{
    AskUser.AskUserToCreateNewVehicleOrConnectToExistingVehicle(positionStringConverter, commandHandler, plateau);
    AskUser.AskUserForMovementInstructionAndSendToVehicle(instructionReader, commandHandler, plateau);

    Console.WriteLine();
    Console.Write("Press any key to continue.. ");
    Console.ReadKey();
}