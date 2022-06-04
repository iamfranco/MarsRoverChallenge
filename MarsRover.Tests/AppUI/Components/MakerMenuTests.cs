using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Models.Elementals;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Vehicles;
using MarsRover.Tests.AppUI.Helpers;

namespace MarsRover.Tests.AppUI.Components;
internal class MakerMenuTests
{
    private Dictionary<string, Func<PlateauBase>> plateauMakers;
    private Dictionary<string, Func<Position, VehicleBase>> vehicleMakers;

    [SetUp]
    public void Setup()
    {
        plateauMakers = new()
        {
            { "Rectangular Plateau", () => new RectangularPlateau(new(10, 5)) },
            { "Circular Plateau", () => new CircularPlateau(8) }
        };

        vehicleMakers = new()
        {
            { "Rover", position => new Rover(position) },
            { "Wall-E", position => new WallE(position) }
        };
    }

    [Test]
    public void AskUserToSelectMaker_With_Empty_Makers_Should_Throw_Exception()
    {
        plateauMakers.Clear();

        Action act = () => MakerMenu.AskUserToSelectMaker("plateau", plateauMakers);

        act.Should().Throw<ArgumentException>().WithMessage("plateauMakers cannot be empty");
    }

    [Test]
    public void AskUserToSelectMaker_With_Makers_With_One_Item_Should_Return_The_One_Item()
    {
        Func<PlateauBase> expectedMaker = () => new RectangularPlateau(new(10, 5));
        plateauMakers = new()
        {
            { "Rectangular Plateau", expectedMaker }
        };

        Func<PlateauBase> actualMaker = MakerMenu.AskUserToSelectMaker("plateau", plateauMakers);

        actualMaker.Should().BeSameAs(expectedMaker);
    }

    [Test]
    public void AskUserToSelectMaker_With_Makers_With_Multiple_Items_And_UserInputs_With_Multiple_Invalid_Inputs_Then_Input_2_Should_Return_Second_Item_In_Makers()
    {
        List<string> userInputs = new() { "ajsdlkfjs", "-1", "100000", "2", "1" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));

        Func<PlateauBase> expectedMaker = () => new CircularPlateau(8);
        plateauMakers = new()
        {
            { "Rectangular Plateau", () => new RectangularPlateau(new(10, 5)) },
            { "Circular Plateau", expectedMaker}
        };
        Func<PlateauBase> actualMaker = MakerMenu.AskUserToSelectMaker("plateau", plateauMakers);

        actualMaker.Should().BeSameAs(expectedMaker);
    }

    [Test]
    public void GetMakersWithKnownPosition_With_VehicleMakers_And_Position_Then_AskUserToSelectMaker_Then_UserInput_2_Should_Return_One_WallE_Maker_At_Position()
    {
        List<string> userInputs = new() { "2" };
        InputReaderContainer.SetInputReader(new InputReaderForTest(userInputs));
        Position initialPosition = new(new(4, 1), Direction.West);

        Dictionary<string, Func<VehicleBase>> vehicleMakersWithKnowPosition = MakerMenu.GetMakersWithKnownPosition(vehicleMakers, initialPosition);

        Func<VehicleBase> actualMaker = MakerMenu.AskUserToSelectMaker("vehicle", vehicleMakersWithKnowPosition);
        VehicleBase actualVehicle = actualMaker();

        actualVehicle.GetType().Name.Should().Be("WallE");
        actualVehicle.Position.Should().Be(initialPosition);
    }
}
