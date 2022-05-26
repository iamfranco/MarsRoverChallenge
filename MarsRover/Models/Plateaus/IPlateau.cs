using MarsRover.Models.Positions;

namespace MarsRover.Models.Plateaus
{
    public interface IPlateau
    {
        bool IsCoordinateValid(Coordinates coordinates);
    }
}
