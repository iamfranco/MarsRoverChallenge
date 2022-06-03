using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Models.Plateaus;
internal class CircularPlateau : PlateauBase
{
    private readonly int _radius;

    public override Coordinates MaximumCoordinates => new(_radius * 2, _radius * 2);

    public override Coordinates MinimumCoordinates => new(0, 0);

    public CircularPlateau(int radius)
    {
        if (radius < 1)
            throw new ArgumentException("Radius must be at least 1");

        _radius = radius;
    }

    public override bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates)
    {
        return Squared(coordinates.X - _radius) + Squared(coordinates.Y - _radius) <= Squared(_radius);
    }

    private static int Squared(int num) => num * num;
}
