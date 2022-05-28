using MarsRover.Models.Positions;
using System.Collections.ObjectModel;

namespace MarsRover.Models.Plateaus
{
    public class RectangularPlateau : PlateauBase
    {
        public Coordinates PlateauSize { get; }

        public RectangularPlateau(Coordinates plateauSize)
        {
            if (!IsValidPlateauSize(plateauSize))
                throw new ArgumentException($"{nameof(plateauSize)} {plateauSize} cannot have negative components", nameof(plateauSize));

            PlateauSize = plateauSize;
        }

        public override bool IsCoordinateValidInPlateau(Coordinates coordinates)
        {
            if (coordinates.X < 0 || coordinates.X > PlateauSize.X)
                return false;

            if (coordinates.Y < 0 || coordinates.Y > PlateauSize.Y)
                return false;

            if (ObstacleCoordinates.Contains(coordinates))
                return false;

            return true;
        }
        private static bool IsValidPlateauSize(Coordinates plateauSize) => plateauSize.X >= 0 && plateauSize.Y >= 0;
    }
}
