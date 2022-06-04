using MarsRover.Models.Elementals;

namespace MarsRover.Models.Vehicles;

public class WallE : VehicleBase
{
    public WallE(Position initialPosition) : base(initialPosition)
    {
    }

    public void GrabAndCompressGarbage()
    {
        Console.WriteLine("Wall E is grabbing and compressing garbage");
    }
}
