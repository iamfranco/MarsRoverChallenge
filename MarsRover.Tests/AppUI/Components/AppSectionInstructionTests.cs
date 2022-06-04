using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.AppUI.PositionStringFormat;
using MarsRover.Controllers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Components;
internal class AppSectionInstructionTests
{
    private IPositionStringConverter positionStringConverter;
    private IInstructionReader instructionReader;
    private AppController appController;

    [SetUp]
    public void Setup()
    {
        positionStringConverter = new StandardPositionStringConverter();
        instructionReader = new StandardInstructionReader();
        appController = new AppController(instructionReader);

        PlateauBase plateau = new RectangularPlateau(new(10, 5));
        appController.ConnectPlateau(plateau);

        Rover vehicle = new(new(new(1, 2), Direction.North));
        appController.AddVehicleToPlateau(vehicle);
    }

    [Test]
    public void AskForInstructionAndSendToVehicle_With_Null_PositionStringConverter_Should_Throw_Exception()
    {
        Action act = () => AppSectionInstruction.AskForInstructionAndSendToVehicle(null, appController);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AskForInstructionAndSendToVehicle_With_Null_AppController_Should_Throw_Exception()
    {
        Action act = () => AppSectionInstruction.AskForInstructionAndSendToVehicle(positionStringConverter, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AskForInstructionAndSendToVehicle_Should_Move_Vehicle_With_First_Valid_UserInput_Instruction()
    {
        string firstValidUserInput = "MMRMM";
        List<string> userInputs = new() { "ajdklfjsdlk", "!jDSF*(", firstValidUserInput, "MM" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));
        Position expectedPosition = new Position(new(3, 4), Direction.East);

        string _ = AppSectionInstruction.AskForInstructionAndSendToVehicle(positionStringConverter, appController);

        appController.Vehicle!.Position.Should().Be(expectedPosition);
    }

    [Test]
    public void AskForInstructionAndSendToVehicle_With_UserInput_Instruction_That_Hits_No_Invalid_Coordinates_Should_Return_Success_String_Message()
    {
        string firstValidUserInput = "MMRMM";
        List<string> userInputs = new() { "ajdklfjsdlk", "!jDSF*(", firstValidUserInput, "MM" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));
        string expectedMessage = "[Rover] reached Position: [3 4 E] after fully applying instruction [MMRMM]";

        string actualMessage = AppSectionInstruction.AskForInstructionAndSendToVehicle(positionStringConverter, appController);

        actualMessage.Should().Be(expectedMessage);
    }

    [Test]
    public void AskForInstructionAndSendToVehicle_With_UserInput_Instruction_That_Hits_Invalid_Coordinates_Should_Return_Danger_Ahead_Message()
    {
        string firstValidUserInput = "MMMMMMMMMMMMMM";
        List<string> userInputs = new() { "ajdklfjsdlk", "!jDSF*(", firstValidUserInput, "MM" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));
        string expectedMessage = "[Rover] sensed danger ahead, so stopped at [1 5 N] instead of applying full instruction [MMMMMMMMMMMMMM]";

        string actualMessage = AppSectionInstruction.AskForInstructionAndSendToVehicle(positionStringConverter, appController);

        actualMessage.Should().Be(expectedMessage);
    }

    [Test]
    public void AskForInstructionAndSendToVehicle_With_UserInput_Instruction_Empty_Should_Return_InstructionEmpty_Message()
    {
        string firstValidUserInput = "";
        List<string> userInputs = new() { "ajdklfjsdlk", "!jDSF*(", firstValidUserInput, "MM" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));
        string expectedMessage = "Instruction is empty, [Rover] is in the same Position: [1 2 N]";

        string actualMessage = AppSectionInstruction.AskForInstructionAndSendToVehicle(positionStringConverter, appController);

        actualMessage.Should().Be(expectedMessage);
    }
}
