using MarsRover.Models.Elementals;

namespace MarsRover.Models.Plateaus;
public class CircularPlateau : PlateauBase
{
    private readonly int _radius;

    public override Coordinates MaximumCoordinates => new(_radius, _radius);

    public override Coordinates MinimumCoordinates => new(-_radius, -_radius);

    public CircularPlateau(int radius)
    {
        if (radius < 1)
            throw new ArgumentException("Radius must be at least 1");

        _radius = radius;
    }

    public override bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates)
    {
        return Squared(coordinates.X) + Squared(coordinates.Y) <= Squared(_radius);
    }

    private static int Squared(int num) => num * num;
}
