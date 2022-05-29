using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.AppUI
{
    internal class CommandHandlerTests
    {
        CommandHandler commandHandler;
        [SetUp]
        public void Setup()
        {
            IInstructionReader instructionReader = new StandardInstructionReader();
            IPositionStringConverter positionStringConverter = new PositionStringConverter();
            commandHandler = new CommandHandler(instructionReader, positionStringConverter);
        }

        [Test]
        public void RequestPosition_Without_ConnectVehicle_Should_Return_No_Vehicle_Connected()
        {
            commandHandler.RequestPosition().Should().Be("No Vehicle Connected");
        }

        [Test]
        public void RequestPosition_After_ConnectVehicle_Should_Return_Vehicle_Position()
        {
            PlateauBase plateau = new RectangularPlateau(new Coordinates(5, 5));
            VehicleBase vehicle = new Rover("1 2 N", plateau);

            commandHandler.ConnectVehicle(vehicle);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }
    }
}
