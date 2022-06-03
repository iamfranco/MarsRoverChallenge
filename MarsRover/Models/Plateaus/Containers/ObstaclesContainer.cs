using System.Collections.ObjectModel;
using MarsRover.Models.Positions.Elementals;

namespace MarsRover.Models.Plateaus.Containers;

public class ObstaclesContainer
{
    private readonly List<Coordinates> _obstacleCoordinates;
    private readonly Func<Coordinates, bool> _coordinateValidateFunc;

    public ObstaclesContainer(Func<Coordinates, bool> coordinateValidateFunc)
    {
        if (coordinateValidateFunc is null)
            throw new ArgumentNullException(nameof(coordinateValidateFunc));

        _obstacleCoordinates = new();
        _coordinateValidateFunc = coordinateValidateFunc;
    }

    public ReadOnlyCollection<Coordinates> ObstacleCoordinates => _obstacleCoordinates.AsReadOnly();

    public void AddObstacle(Coordinates obstacle)
    {
        if (!_coordinateValidateFunc(obstacle))
            throw new ArgumentException($"Coordinates {obstacle} is not available on plateau");

        _obstacleCoordinates.Add(obstacle);
    }

    public void RemoveObstacle(Coordinates obstacle) => _obstacleCoordinates.Remove(obstacle);
}
