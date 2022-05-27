using MarsRover.Models.Plateaus;
using MarsRover.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarsRover.Helpers
{
    public static class Guard
    {
        public static void ThrowIfNull(object input, [CallerArgumentExpression("input")] string callerInputName = "")
        {
            if (input is null)
                throw new ArgumentException($"{callerInputName} cannot be null", callerInputName);
        }

        public static void ThrowIfBelowZero(int input, [CallerArgumentExpression("input")] string callerInputName = "")
        {
            if (input < 0)
                throw new ArgumentException($"{callerInputName} cannot be negative", callerInputName);
        }

        public static void ThrowIfCoordinateInvalidForPlateau(Coordinates coordinates, IPlateau plateau,
            [CallerArgumentExpression("coordinates")] string coordinatesName = "")
        {
            if (!plateau.IsCoordinateValid(coordinates))
                throw new ArgumentException($"Coordinate {coordinates} is invalid in plateau " +
                    $"(either outside of plateau or is on obstacle)", nameof(coordinatesName));
        }
    }
}
