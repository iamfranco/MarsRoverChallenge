using MarsRover.Models.Instructions;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI
{
    public class CommandHandler
    {
        private IInstructionReader _instructionReader;
        private IPositionStringConverter _positionStringConverter;

        public CommandHandler(IInstructionReader instructionReader, IPositionStringConverter positionStringConverter)
        {
            _instructionReader = instructionReader;
            _positionStringConverter = positionStringConverter;
        }

        public string RequestPosition()
        {
            return "No Vehicle Connected";
        }

        public void ConnectVehicle(VehicleBase vehicle)
        {
            throw new NotImplementedException();
        }
    }
}
