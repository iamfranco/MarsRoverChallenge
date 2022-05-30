using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Models.Vehicles
{
    public class Rover : VehicleBase
    {
        public Rover(Coordinates initialCoordinates, DirectionEnum initialDirection, PlateauBase plateau)
            : base(initialCoordinates, initialDirection, plateau)
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
