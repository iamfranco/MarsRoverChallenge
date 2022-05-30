﻿using System.Text.RegularExpressions;

namespace MarsRover.Models.Positions
{
    public class PositionStringConverter : IPositionStringConverter
    {
        private readonly Regex _positionStringRegex = new(@"^-?\d+ -?\d+ (N|E|S|W)$");
        private readonly Regex _coordinateStringRegex = new(@"^-?\d+ -?\d+$");
        private static readonly List<(string directionSymbol, DirectionEnum direction)> _directionSymbolEnum = new()
        {
            ("N", DirectionEnum.North),
            ("E", DirectionEnum.East),
            ("S", DirectionEnum.South),
            ("W", DirectionEnum.West)
        };

        public bool IsValidPositionString(string? positionString) => IsNotNullAndMatchRegex(positionString, _positionStringRegex);

        public bool IsValidCoordinateString(string? coordinateString) => IsNotNullAndMatchRegex(coordinateString, _coordinateStringRegex);

        public (Coordinates, DirectionEnum) ToCoordinatesDirection(string? positionString)
        {
            if (!IsValidPositionString(positionString))
                throw new ArgumentException($"Input {positionString} is not in correct format for positionString (eg \"1 2 N\")", nameof(positionString));

            (string coordinateString, string directionString) = SplitPositionString(positionString!);

            DirectionEnum direction = _directionSymbolEnum
                .FirstOrDefault(item => item.directionSymbol == directionString)
                .direction;

            return (ToCoordinates(coordinateString), direction);
        }

        public Coordinates ToCoordinates(string? coordinateString)
        {
            if (!IsValidCoordinateString(coordinateString))
                throw new ArgumentException($"Input {coordinateString} is not in correct format for coordianteString (eg \"1 2\")", nameof(coordinateString));

            string[] coordinateStringArray = coordinateString!.Split(" ");
            int x = int.Parse(coordinateStringArray[0]);
            int y = int.Parse(coordinateStringArray[1]);

            return new Coordinates(x, y);
        }

        public string ToPositionString(Coordinates coordinates, DirectionEnum direction)
        {
            string directionSymbol = _directionSymbolEnum
                .FirstOrDefault(x => x.direction == direction)
                .directionSymbol;

            return $"{coordinates.X} {coordinates.Y} {directionSymbol}";
        }

        private static bool IsNotNullAndMatchRegex(string? str, Regex regex) => str is not null && regex.IsMatch(str);

        private static (string coordinateString, string directionString) SplitPositionString(string positionString)
        {
            int lastSpaceIndex = positionString.LastIndexOf(" ");

            string coordinateString = positionString[..lastSpaceIndex];
            string directionString = positionString[(lastSpaceIndex + 1)..];

            return (coordinateString, directionString);
        }
    }
}
