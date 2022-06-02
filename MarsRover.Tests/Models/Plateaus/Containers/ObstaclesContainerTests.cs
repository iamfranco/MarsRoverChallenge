using MarsRover.Models.Plateaus.Containers;
using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Plateaus.Containers;

internal class ObstaclesContainerTests
{
    private Func<Coordinates, bool> coordinateValidateFunc;
    private ObstaclesContainer obstaclesContainer;
    [SetUp]
    public void Setup()
    {
        // simulate PlateauBase's coordinate validation method
        coordinateValidateFunc = (coordinates) => coordinates.X >= 0 && coordinates.Y >= 0;

        obstaclesContainer = new ObstaclesContainer(coordinateValidateFunc);
    }

    [Test]
    public void Constructor_With_Null_Predicate_Should_Throw_Exception()
    {
        Action act = () => obstaclesContainer = new ObstaclesContainer(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ObstacleCoordinates_Should_Return_Empty_Collection_Of_Coordinates_By_Default()
    {
        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(0);
    }

    [Test]
    public void AddObstacle_Then_ObstacleCoordinates_Should_Return_AddedObstacleCoordinates()
    {
        List<Coordinates> obstacleCoordinates = new()
        {
            new(1, 3),
            new(2, 2),
            new(3, 4)
        };

        foreach (var obstacle in obstacleCoordinates)
        {
            obstaclesContainer.AddObstacle(obstacle);
        }

        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(3);
        for (var i = 0; i < obstaclesContainer.ObstacleCoordinates.Count; i++)
        {
            obstaclesContainer.ObstacleCoordinates[i].Should().Be(obstacleCoordinates[i]);
        }
    }

    [Test]
    public void AddObstacle_On_Invalid_Coordinate_Should_Not_Change_Obstacles()
    {
        Action act = () => obstaclesContainer.AddObstacle(new(-100, -100));

        act.Should().Throw<ArgumentException>();
        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(0);
    }

    [Test]
    public void RemoveObstacle_Then_ObstacleCoordinates_Should_Not_Include_Removed_ObstacleCoordinate()
    {
        obstaclesContainer.AddObstacle(new(1, 2));
        obstaclesContainer.AddObstacle(new(2, 4));

        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(2);

        obstaclesContainer.RemoveObstacle(new(1, 2));
        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(1);
        obstaclesContainer.ObstacleCoordinates.Contains(new Coordinates(1, 2)).Should().Be(false);
    }

    [Test]
    public void RemoveObstacle_With_NonExistent_Obstacle_Should_Not_Change_ObstacleCoordinates()
    {
        obstaclesContainer.AddObstacle(new(1, 2));
        obstaclesContainer.AddObstacle(new(2, 4));

        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(2);
        obstaclesContainer.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 2));
        obstaclesContainer.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 4));

        obstaclesContainer.RemoveObstacle(new(2, 2));
        obstaclesContainer.ObstacleCoordinates.Count.Should().Be(2);
        obstaclesContainer.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 2));
        obstaclesContainer.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 4));
    }
}
