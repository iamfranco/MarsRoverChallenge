﻿using MarsRover.AppUI.Components;
using MarsRover.AppUI.Helpers;
using MarsRover.Controllers;
using MarsRover.Models.Instructions;
using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using MarsRover.Models.Positions.Elementals;
using MarsRover.Models.Vehicles;

namespace MarsRover.AppUI;

public class AppUIHandler
{
    private readonly IPositionStringConverter _positionStringConverter;
    private readonly IInstructionReader _instructionReader;
    private readonly AppController _appController;
    private readonly MapPrinter _mapPrinter;

    public AppUIHandler(
        IPositionStringConverter positionStringConverter,
        IInstructionReader instructionReader,
        AppController appController,
        MapPrinter mapPrinter)
    {
        _positionStringConverter = positionStringConverter;
        _instructionReader = instructionReader;
        _appController = appController;
        _mapPrinter = mapPrinter;
    }

    public PlateauBase AskUserToMakePlateau(Dictionary<string, Func<PlateauBase>> plateauMakers)
    {
        PlateauBase plateau = AppUIHelpers.ExecuteUntilNoException(
            () => AppSectionPlateau.AskForPlateau(_appController, plateauMakers));

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
        return plateau;
    }

    public void AskUserToMakeObstacles(PlateauBase plateau)
    {
        AppSectionObstacles.AskForObstaclesUntilEmptyInput(_positionStringConverter, _appController, _mapPrinter, plateau);
    }

    public void AskUserToCreateNewVehicleOrConnectToExistingVehicle(
        PlateauBase plateau,
        Dictionary<string, Func<Position, VehicleBase>> vehicleMakers)
    {
        _appController.ResetRecentPath();
        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);

        AppUIHelpers.ExecuteUntilNoException(
            () => AppSectionVehicle.AskForPositionOrCoordinatesToCreateOrConnectVehicle(
                _positionStringConverter, _appController, plateau, vehicleMakers));

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
        Console.WriteLine($"Connected to [{_appController.GetVehicle()!.GetType().Name}] " +
            $"at [{_positionStringConverter.ToPositionString(_appController.GetVehicle()!.Position)}]");
    }

    public void AskUserForMovementInstructionAndSendToVehicle()
    {
        var message = AppSectionInstruction.AskForInstructionAndSendToVehicle(
            _instructionReader, _positionStringConverter, _appController);

        AppUIHelpers.ClearScreenAndPrintMap(_appController, _mapPrinter);
        Console.WriteLine(message);
    }
}
