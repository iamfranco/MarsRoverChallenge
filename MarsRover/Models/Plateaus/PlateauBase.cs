using MarsRover.Models.Positions;

namespace MarsRover.Models.Plateaus;

public abstract class PlateauBase
{
    public ObstaclesContainer ObstaclesContainer { get; }
    public VehiclesContainer VehiclesContainer { get; }

    public PlateauBase()
    {
        ObstaclesContainer = new ObstaclesContainer(IsCoordinateValidInPlateau);
        VehiclesContainer = new VehiclesContainer(IsCoordinateValidInPlateau);
    }

    public bool IsCoordinateValidInPlateau(Coordinates coordinates)
    {
        if (!IsCoordinateWithinPlateauBoundary(coordinates))
            return false;

        if (ObstaclesContainer.ObstacleCoordinates.Contains(coordinates))
            return false;

        if (VehiclesContainer.Vehicles.Select(vehicle => vehicle.Position.Coordinates).Contains(coordinates))
            return false;

        return true;
    }
    public abstract void PrintMap(List<Position> recentPath);
    protected abstract bool IsCoordinateWithinPlateauBoundary(Coordinates coordinates);
}
