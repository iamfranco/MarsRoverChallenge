using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Models.Plateaus;
internal class CircularPlateau : PlateauBase
{
    private readonly int _radius;

    public CircularPlateau(int radius)
    {
        if (radius < 1)
            throw new ArgumentException("Radius must be at least 1");

        _radius = radius;
    }

    public override void PrintMap(List<Position> recentPath) => PrintMap(recentPath, new(_radius * 2, _radius * 2));

    protected override bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates)
    {
        return Squared(coordinates.X - _radius) + Squared(coordinates.Y - _radius) <= Squared(_radius);
    }

    private static int Squared(int num) => num * num;
}
