using MarsRover.Models.Elementals;

namespace MarsRover.Models.Vehicles;

public class Rover : VehicleBase
{
    public Rover(Position initialPosition) : base(initialPosition)
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
