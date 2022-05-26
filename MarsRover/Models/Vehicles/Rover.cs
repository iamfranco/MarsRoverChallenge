using MarsRover.Models.MovementInstructions;
using MarsRover.Models.Plateaus;

namespace MarsRover.Models.Vehicles
{
    public class Rover : VehicleBase
    {
        private int _remainingSampleStorageSpace = 10;

        public Rover(string initialPosition, IPlateau plateau, IInstructionReader instructionReader)
            : base(initialPosition, plateau, instructionReader)
        {
        }

        public Rover(string initialPosition, IPlateau plateau)
            : this(initialPosition, plateau, new StandardInstructionReader())
        {
        }

        public void TakePhotoAndSendToStation()
        {
            Console.WriteLine("Rover took a photo and sent it back to station.");
        }

        public void CollectSample()
        {
            if (_remainingSampleStorageSpace > 0)
            {
                _remainingSampleStorageSpace--;
                Console.WriteLine("Rover collected a sample");
                Console.WriteLine($"Remaining sample storage space: {_remainingSampleStorageSpace}");
            }
            else
            {
                Console.WriteLine("Rover sample storage space is full, cannot collect more samples.");
            }
        }
    }
}
