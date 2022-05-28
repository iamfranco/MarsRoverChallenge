using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;

namespace MarsRover.Tests.Models.Plateaus
{
    internal class RectangularPlateauTests
    {
        [Test]
        public void Constructor_Should_Throw_Exception_For_Input_PlateauSize_With_Negative_Components()
        {
            RectangularPlateau plateau;
            Coordinates plateauSize;
            Action act;

            plateauSize = new(-5, 8);
            act = () => plateau = new(plateauSize);
            act.Should().Throw<ArgumentException>();

            plateauSize = new(3, -4);
            act = () => plateau = new(plateauSize);
            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void PlateauSize_Should_Return_Constructor_PlateauSize_Input()
        {
            RectangularPlateau plateau;
            Coordinates plateauSize;

            plateauSize = new(5, 5);
            plateau = new(plateauSize);
            plateau.PlateauSize.Should().Be(plateauSize);

            plateauSize = new(2, 10);
            plateau = new(plateauSize);
            plateau.PlateauSize.Should().Be(plateauSize);

            plateauSize = new(0, 0);
            plateau = new(plateauSize);
            plateau.PlateauSize.Should().Be(plateauSize);
        }

        [Test]
        public void ObstacleCoordinates_Should_Return_Empty_List_By_Default()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.ObstacleCoordinates.Count.Should().Be(0);
        }

        [Test]
        public void AddObstacle_Then_ObstacleCoordinates_Should_Return_AddedObstacleCoordinate()
        {
            RectangularPlateau plateau = new(new(5, 5));
            List<Coordinates> obstacleCoordinates = new()
            {
                new(1, 3),
                new(2, 2),
                new(3, 4)
            };

            foreach (Coordinates obstacle in obstacleCoordinates)
            {
                plateau.AddObstacle(obstacle);
            }

            plateau.ObstacleCoordinates.Count.Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                plateau.ObstacleCoordinates[i].Should().Be(obstacleCoordinates[i]);
            }
        }

        [Test]
        public void AddObstacle_Then_ObstacleCoordinates_Should_Not_Have_Duplicate_Obstacles()
        {
            RectangularPlateau plateau = new(new(5, 5));

            plateau.AddObstacle(new(1, 3));
            plateau.ObstacleCoordinates.Count.Should().Be(1);

            plateau.AddObstacle(new(1, 3));
            plateau.ObstacleCoordinates.Count.Should().Be(1);

            plateau.AddObstacle(new(2, 2));
            plateau.ObstacleCoordinates.Count.Should().Be(2);

            plateau.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 3));
            plateau.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 2));
        }

        [Test]
        public void RemoveObstacle_Then_ObstacleCoordinates_Should_Not_Include_Removed_ObstacleCoordinate()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(1, 2));
            plateau.AddObstacle(new(2, 4));

            plateau.ObstacleCoordinates.Count.Should().Be(2);

            plateau.RemoveObstacle(new(1, 2));
            plateau.ObstacleCoordinates.Count.Should().Be(1);
            plateau.ObstacleCoordinates.Contains(new Coordinates(1, 2)).Should().Be(false);
        }

        [Test]
        public void RemoveObstacle_With_NonExistent_Obstacle_Should_Not_Change_ObstacleCoordinates()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(1, 2));
            plateau.AddObstacle(new(2, 4));

            plateau.ObstacleCoordinates.Count.Should().Be(2);
            plateau.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 2));
            plateau.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 4));

            plateau.RemoveObstacle(new(2, 2));
            plateau.ObstacleCoordinates.Count.Should().Be(2);
            plateau.ObstacleCoordinates[0].Should().Be(new Coordinates(1, 2));
            plateau.ObstacleCoordinates[1].Should().Be(new Coordinates(2, 4));
        }

        [Test]
        public void IsCoordinateValidInPlateau_With_Coordinate_Outside_Of_Plateau_Should_Return_False()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.IsCoordinateValidInPlateau(new(-1, -2)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(-1, 3)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(3, 10)).Should().Be(false);
        }

        [Test]
        public void IsCoordinateValidInPlateau_With_Coordinate_Inside_Plateau_And_Not_On_Obstacle_Should_Return_True()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.IsCoordinateValidInPlateau(new(4, 3)).Should().Be(true);
            plateau.IsCoordinateValidInPlateau(new(1, 5)).Should().Be(true);
            plateau.IsCoordinateValidInPlateau(new(0, 0)).Should().Be(true);
        }

        [Test]
        public void IsCoordinateValidInPLateau_With_Coordinate_On_Obstacle_Should_Return_False()
        {
            RectangularPlateau plateau = new(new(5, 5));
            plateau.AddObstacle(new(1, 2));
            plateau.AddObstacle(new(4, 3));

            plateau.IsCoordinateValidInPlateau(new(1, 2)).Should().Be(false);
            plateau.IsCoordinateValidInPlateau(new(4, 3)).Should().Be(false);
        }
    }
}
