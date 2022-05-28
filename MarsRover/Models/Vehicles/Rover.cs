using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public class Rover : VehicleBase
    {
        public Rover(string initialPosition, PlateauBase plateau, IPositionStringConverter positionStringConverter)
            : base(initialPosition, plateau, positionStringConverter)
        {
        }

        public Rover(string initialPosition, PlateauBase plateau)
            : base(initialPosition, plateau)
        {
        }

        public void TakePhotoAndSendToStation()
        {
            Console.WriteLine("Rover takes photo and send to station");
        }

        public void CollectSample()
        {
            Console.WriteLine("Rover collects Sample");
        }
    }
}
