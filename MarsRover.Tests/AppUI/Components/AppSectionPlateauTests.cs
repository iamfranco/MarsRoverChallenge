using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Components;
internal class AppSectionPlateauTests
{
    AppController appController;
    Dictionary<string, Func<PlateauBase>> plateauMakers;

    [SetUp]
    public void Setup()
    {
        IInstructionReader instructionReader = new StandardInstructionReader();
        appController = new AppController(instructionReader);

        plateauMakers = new()
        {
            {"Rectangular Plateau", () => new RectangularPlateau(new(10, 5)) },
            {"Circular Plateau", () => new CircularPlateau(8) }
        };
    }

    [Test]
    public void AskForPlateau_With_Null_AppController_Should_Throw_Exception()
    {
        Action act = () => AppSectionPlateau.AskForPlateau(null, plateauMakers);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AskForPlateau_With_Null_PlateauMakers_Should_Throw_Exception()
    {
        Action act = () => AppSectionPlateau.AskForPlateau(appController, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AskForPlateau_With_PlateaMakers_Of_One_Item_Should_Connect_AppController_To_The_One_Plateau_Type()
    {
        plateauMakers = new()
        {
            {"Rectangular Plateau", () => new RectangularPlateau(new(10, 5)) }
        };

        AppSectionPlateau.AskForPlateau(appController, plateauMakers);

        appController.Plateau.Should().NotBeNull();
        appController.Plateau!.GetType().Name.Should().Be(nameof(RectangularPlateau));
    }

    [Test]
    public void AskForPlateau_With_PlateaMakers_Having_Multiple_Items_Should_Prompt_For_UserInput_On_Number_And_Then_Connect_AppController_To_User_Selected_Plateau_Type()
    {
        List<string> userInputs = new() { "2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        AppSectionPlateau.AskForPlateau(appController, plateauMakers);

        appController.Plateau.Should().NotBeNull();
        appController.Plateau!.GetType().Name.Should().Be(nameof(CircularPlateau));
    }

    [Test]
    public void AskForPlateau_With_Multiple_UserInputs_Should_Only_Connect_To_Plateau_That_Matches_The_First_UserInput_That_Is_Between_1_And_Number_Of_Items_In_PlateauMakers()
    {
        string firstValidInput = "1";
        List<string> userInputs = new() { "-1", "0", firstValidInput, "2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        AppSectionPlateau.AskForPlateau(appController, plateauMakers);

        appController.Plateau.Should().NotBeNull();
        appController.Plateau!.GetType().Name.Should().Be(nameof(RectangularPlateau));
    }
}
