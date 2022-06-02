using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.Models.Vehicles;

internal class RoverTests
{
    private readonly List<List<SingularInstruction>> validInstructions = new()
    {
        new() { },
        new()
        {
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.TurnLeft
        },
        new()
        {
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward
        }
    };
    private readonly List<Coordinates> obstacles = new() { new(2, 3), new(5, 5) };
    private PlateauBase plateau;
    private PlateauBase plateauWithObstacles;
    private Rover rover;

    [SetUp]
    public void Setup()
    {
        plateau = new RectangularPlateau(new(5, 5));
        plateauWithObstacles = new RectangularPlateau(new(5, 5));
        foreach (Coordinates obstacle in obstacles)
        {
            plateauWithObstacles.ObstaclesContainer.AddObstacle(obstacle);
        }

        rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
    }

    [Test]
    public void Position_After_Successful_Construction_Should_Return_InitialPosition()
    {
        Position initialPosition = new(new(1, 2), Direction.North);
        Rover rover = new Rover(initialPosition);

        rover.Position.Should().Be(initialPosition);
    }

    [Test]
    public void ApplyMoveInstruction_With_Null_Instruction_Should_Throw_Exception()
    {
        Action act = () => rover.ApplyMoveInstruction(null, plateau);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ApplyMoveInstruction_With_Null_Plateau_Should_Throw_Exception()
    {
        Action act = () => rover.ApplyMoveInstruction(validInstructions[0], null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ApplyMoveInstruction_With_Vehicle_That_Does_Not_Exist_On_Plateau_Should_Throw_Exception()
    {
        Action act = () => rover.ApplyMoveInstruction(validInstructions[0], plateau);
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ApplyMoveInstruction_With_Valid_Instruction_String_Should_Succeed()
    {
        Action act;
        plateau.VehiclesContainer.AddVehicle(rover);

        foreach (List<SingularInstruction> validInstruction in validInstructions)
        {
            act = () => rover.ApplyMoveInstruction(validInstruction, plateau);
            act.Should().NotThrow();
        }
    }

    [Test]
    public void Position_After_ApplyMoveInstruction_Should_Return_Correct_New_Position()
    {
        Rover rover;
        List<Position> recentPath;
        bool isEmergencyStopUsed;

        rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        plateau.VehiclesContainer.AddVehicle(rover);
        (recentPath, isEmergencyStopUsed) = rover.ApplyMoveInstruction(new()
        {
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward
        }, plateau);

        rover.Position.Should().Be(new Position(new(1, 3), Direction.North));
        recentPath.Count.Should().Be(10);
        recentPath.Should().BeEquivalentTo(new List<Position>()
        {
            new(new(1,2), Direction.North),
            new(new(1,2), Direction.West),
            new(new(0,2), Direction.West),
            new(new(0,2), Direction.South),
            new(new(0,1), Direction.South),
            new(new(0,1), Direction.East),
            new(new(1,1), Direction.East),
            new(new(1,1), Direction.North),
            new(new(1,2), Direction.North),
            new(new(1,3), Direction.North),
        });
        isEmergencyStopUsed.Should().Be(false);

        rover = new Rover(new Position(new Coordinates(3, 3), Direction.East));
        plateau.VehiclesContainer.AddVehicle(rover);
        (recentPath, isEmergencyStopUsed) = rover.ApplyMoveInstruction(new()
        {
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward
        }, plateau);

        rover.Position.Should().Be(new Position(new(5, 1), Direction.East));
        recentPath.Count.Should().Be(11);
        recentPath.Should().BeEquivalentTo(new List<Position>()
        {
            new(new(3,3), Direction.East),
            new(new(4,3), Direction.East),
            new(new(5,3), Direction.East),
            new(new(5,3), Direction.South),
            new(new(5,2), Direction.South),
            new(new(5,1), Direction.South),
            new(new(5,1), Direction.West),
            new(new(4,1), Direction.West),
            new(new(4,1), Direction.North),
            new(new(4,1), Direction.East),
            new(new(5,1), Direction.East)
        });
        isEmergencyStopUsed.Should().Be(false);
    }

    [Test]
    public void ApplyMoveInstruction_Into_Invalid_Coordinate_For_Plateau_Should_Have_Position_Stop_Before_Invalid_Coordinate()
    {
        PlateauBase plateauWithOneObstacle = new RectangularPlateau(new(5, 5));
        plateauWithOneObstacle.ObstaclesContainer.AddObstacle(new(2, 3));

        Rover rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        plateauWithOneObstacle.VehiclesContainer.AddVehicle(rover);

        List<SingularInstruction> instruction = new()
        {
            SingularInstruction.TurnRight,
            SingularInstruction.TurnLeft,
            SingularInstruction.MoveForward,
            SingularInstruction.TurnRight,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
            SingularInstruction.MoveForward,
        };

        (List<Position> recentPath, bool isEmergencyStopUsed) = rover.ApplyMoveInstruction(instruction, plateauWithOneObstacle);

        rover.Position.Should().Be(new Position(new(1, 3), Direction.East));
        recentPath.Count.Should().Be(5);
        recentPath.Should().BeEquivalentTo(new List<Position>()
        {
            new(new(1, 2), Direction.North),
            new(new(1, 2), Direction.East),
            new(new(1, 2), Direction.North),
            new(new(1, 3), Direction.North),
            new(new(1, 3), Direction.East)
        });
        isEmergencyStopUsed.Should().Be(true);
    }

    [Test]
    public void TakePhotoAndSendToStation_Should_Not_Throw_Exception()
    {
        Rover rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        Action act;

        act = () => rover.TakePhotoAndSendToStation();
        act.Should().NotThrow();
    }

    [Test]
    public void CollectSample_Should_Not_Throw_Exception()
    {
        Rover rover = new Rover(new Position(new Coordinates(1, 2), Direction.North));
        Action act;

        act = () => rover.CollectSample();
        act.Should().NotThrow();
    }
}
