using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Models.Plateaus;

public class RectangularPlateau : PlateauBase
{
    private readonly Coordinates _maximumCoordinates;

    public RectangularPlateau(Coordinates maximumCoordinates)
    {
        if (!IsValidMaximumCoordinates(maximumCoordinates))
            throw new ArgumentException($"Maximum Coordinates {maximumCoordinates} cannot have negative components");

        _maximumCoordinates = maximumCoordinates;
    }

    public override void PrintMap(List<Position> recentPath) => PrintMap(recentPath, _maximumCoordinates);
    
    protected override bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates)
    {
        if (coordinates.X < 0 || coordinates.X > _maximumCoordinates.X)
            return false;

        if (coordinates.Y < 0 || coordinates.Y > _maximumCoordinates.Y)
            return false;

        return true;
    }

    private static bool IsValidMaximumCoordinates(Coordinates maximumCoordinates) => maximumCoordinates.X >= 0 && maximumCoordinates.Y >= 0;
}
