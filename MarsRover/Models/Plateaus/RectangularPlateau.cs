using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Models.Plateaus;

public class RectangularPlateau : PlateauBase
{
    public override Coordinates MaximumCoordinates { get; }

    public override Coordinates MinimumCoordinates => new(0, 0);

    public RectangularPlateau(Coordinates maximumCoordinates)
    {
        if (!IsValidMaximumCoordinates(maximumCoordinates))
            throw new ArgumentException($"Maximum Coordinates {maximumCoordinates} cannot have negative components");

        MaximumCoordinates = maximumCoordinates;
    }

    public override bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates)
    {
        if (coordinates.X < MinimumCoordinates.X || coordinates.X > MaximumCoordinates.X)
            return false;

        if (coordinates.Y < MinimumCoordinates.Y || coordinates.Y > MaximumCoordinates.Y)
            return false;

        return true;
    }

    private static bool IsValidMaximumCoordinates(Coordinates maximumCoordinates) => maximumCoordinates.X >= 0 && maximumCoordinates.Y >= 0;
}
