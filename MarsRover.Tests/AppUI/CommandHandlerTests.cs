using MarsRover.AppUI;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Vehicles;

namespace MarsRover.Tests.AppUI
{
    internal class CommandHandlerTests
    {
        CommandHandler commandHandler;
        PlateauBase plateau;

        [SetUp]
        public void Setup()
        {
            IInstructionReader instructionReader = new StandardInstructionReader();
            IPositionStringConverter positionStringConverter = new PositionStringConverter();
            commandHandler = new CommandHandler(instructionReader, positionStringConverter);

            plateau = new RectangularPlateau(new(5, 5));
        }

        [Test]
        public void RequestPosition_Without_ConnectVehicle_Should_Return_No_Vehicle_Connected()
        {
            commandHandler.RequestPosition().Should().Be("No Vehicle Connected");
        }

        [Test]
        public void ConnectVehicle_With_Null_Then_RequestPosition_Should_Return_No_Vehicle_Connected()
        {
            VehicleBase vehicle = null;
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.RequestPosition().Should().Be("No Vehicle Connected");
        }

        [Test]
        public void ConnectVehicle_Then_RequestPosition_Should_Return_Vehicle_Position()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);

            commandHandler.ConnectVehicle(vehicle);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }

        [Test]
        public void ConnectVehicle_Then_ConnectVehicle_With_Null_Then_RequestPosition_Should_Return_Initial_Vehicle_Position()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);

            commandHandler.ConnectVehicle(vehicle);
            commandHandler.ConnectVehicle(null);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }

        [Test]
        public void DisconnectVehicle_Then_RequestPosition_Should_Return_No_Vehicle_Connected()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);

            commandHandler.ConnectVehicle(vehicle);
            commandHandler.DisconnectVehicle();

            commandHandler.RequestPosition().Should().Be("No Vehicle Connected");
        }

        [Test]
        public void SendMoveInstruction_Without_ConnectVehicle_Should_Return_False()
        {
            string instruction = "RMMLM";
            (bool status, string _) = commandHandler.SendMoveInstruction(instruction);
            
            status.Should().Be(false);
        }

        [Test]
        public void ConnectVehicle_Then_SendMoveInstruction_With_Null_Instruction_Should_Return_False_And_Not_Modify_Vehicle_Position()
        {
            string instruction = null;
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);
            
            commandHandler.ConnectVehicle(vehicle);
            (bool status, string _) = commandHandler.SendMoveInstruction(instruction);

            status.Should().Be(false);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }

        [Test]
        public void ConnectVehicle_Then_SendMoveInstruction_With_Empty_Instruction_String_Should_Return_False_And_Not_Modify_Vehicle_Position()
        {
            string instruction = "";
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);

            commandHandler.ConnectVehicle(vehicle);
            (bool status, string _) = commandHandler.SendMoveInstruction(instruction);

            status.Should().Be(false);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }

        [Test]
        public void ConnectVehicle_Then_SendMoveInstruction_With_Invalidly_Formatted_Instruction_String_Should_Return_False_And_Not_Modify_Vehicle_Position()
        {
            string instruction = "LM!LM";
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);

            commandHandler.ConnectVehicle(vehicle);
            (bool status, string _) = commandHandler.SendMoveInstruction(instruction);

            status.Should().Be(false);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }

        [Test]
        public void ConnectVehicle_Then_SendMoveInstruction_With_Instruction_String_Which_Move_On_Invalid_Coordinates_Of_Plateau_Should_Return_False_And_Not_Modify_Vehicle_Position()
        {
            PlateauBase plateauWithOneObstacle = new RectangularPlateau(new(5, 5));
            plateauWithOneObstacle.AddObstacle(new(2, 4));

            string instruction = "RMLMMM";
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateauWithOneObstacle);

            commandHandler.ConnectVehicle(vehicle);
            (bool status, string _) = commandHandler.SendMoveInstruction(instruction);

            status.Should().Be(false);
            commandHandler.RequestPosition().Should().Be("1 2 N");
        }

        [Test]
        public void ConnectVehicle_Then_SendMoveInstruction_With_Valid_Instruction_String_Should_Return_True_And_Modify_Vehicle_Position()
        {
            string instruction = "MRM";
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);

            commandHandler.ConnectVehicle(vehicle);
            (bool status, string _) = commandHandler.SendMoveInstruction(instruction);

            status.Should().Be(true);
            commandHandler.RequestPosition().Should().Be("2 3 E");
        }

        [Test]
        public void RecentPath_Should_Return_Empty_List_By_Default()
        {
            commandHandler.RecentPath.Count.Should().Be(0);
        }

        [Test]
        public void RecentPath_After_Successful_SendMoveInstruction_Should_Return_Instructions_Travel_Path()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.SendMoveInstruction("MRM");

            List<(Coordinates, Direction)> expectedResult = new()
            {
                (new(1, 2), new("north")),
                (new(1, 3), new("north")),
                (new(1, 3), new("east")),
                (new(2, 3), new("east"))
            };

            List<(Coordinates, Direction)> actualResult = commandHandler.RecentPath;

            actualResult.Count.Should().Be(expectedResult.Count);
            for (int i=0; i<actualResult.Count; i++)
            {
                actualResult[i].Should().BeEquivalentTo(expectedResult[i]);
            }
        }

        [Test]
        public void RecentPath_After_Multiple_Successful_SendMoveInstruction_Should_Return_Last_Instructions_Travel_Path()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.SendMoveInstruction("MMLL");
            commandHandler.SendMoveInstruction("MMLL");
            commandHandler.SendMoveInstruction("MRMR");

            List<(Coordinates, Direction)> expectedResult = new()
            {
                (new(1, 2), new("north")),
                (new(1, 3), new("north")),
                (new(1, 3), new("east")),
                (new(2, 3), new("east")),
                (new(2, 3), new("south"))
            };

            List<(Coordinates, Direction)> actualResult = commandHandler.RecentPath;

            actualResult.Count.Should().Be(expectedResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                actualResult[i].Should().BeEquivalentTo(expectedResult[i]);
            }
        }

        [Test]
        public void RecentPath_After_Invalidly_Formatted_Instruction_To_SendMoveInstruction_Should_Return_Last_Successful_Instruction_Travel_Path()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.SendMoveInstruction("MMLL");
            commandHandler.SendMoveInstruction("MMLL");
            commandHandler.SendMoveInstruction("MRMR");
            commandHandler.SendMoveInstruction("ML!!?");

            List<(Coordinates, Direction)> expectedResult = new()
            {
                (new(1, 2), new("north")),
                (new(1, 3), new("north")),
                (new(1, 3), new("east")),
                (new(2, 3), new("east")),
                (new(2, 3), new("south"))
            };

            List<(Coordinates, Direction)> actualResult = commandHandler.RecentPath;

            actualResult.Count.Should().Be(expectedResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                actualResult[i].Should().BeEquivalentTo(expectedResult[i]);
            }
        }

        [Test]
        public void RecentPath_After_SendMoveInstruction_With_Instruction_That_Leads_To_Obstacle_Should_Return_Path_Up_Until_Obstacle()
        {
            PlateauBase plateauWithObstacle = new RectangularPlateau(new(5, 5));
            plateauWithObstacle.AddObstacle(new(2, 4));

            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateauWithObstacle);
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.SendMoveInstruction("MML");
            commandHandler.SendMoveInstruction("RRMM");

            List<(Coordinates, Direction)> expectedResult = new()
            {
                (new(1, 4), new("west")),
                (new(1, 4), new("north")),
                (new(1, 4), new("east"))
            };

            List<(Coordinates, Direction)> actualResult = commandHandler.RecentPath;

            actualResult.Count.Should().Be(expectedResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                actualResult[i].Should().BeEquivalentTo(expectedResult[i]);
            }
        }

        [Test]
        public void RecentPath_After_Re_ConnectVehicle_Should_Return_Empty_List()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.SendMoveInstruction("MML");
            commandHandler.SendMoveInstruction("RRMM");

            commandHandler.ConnectVehicle(vehicle);

            commandHandler.RecentPath.Count.Should().Be(0);
        }

        [Test]
        public void RecentPath_After_DisconnectVehicle_Should_Return_Empty_List()
        {
            VehicleBase vehicle = new Rover(new Coordinates(1, 2), DirectionEnum.North, plateau);
            commandHandler.ConnectVehicle(vehicle);
            commandHandler.SendMoveInstruction("MML");
            commandHandler.SendMoveInstruction("RRMM");

            commandHandler.DisconnectVehicle();

            commandHandler.RecentPath.Count.Should().Be(0);
        }
    }
}
