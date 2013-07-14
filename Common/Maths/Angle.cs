using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Maths
{
    public static class Angle
    {
        public const double RadianToDegreeD = (180.0 / Math.PI);
        public const float RadianToDegreeF = (float)(180.0 / Math.PI);
        public const double DegreeToRadianD = (Math.PI / 180.0);
        public const float DegreeToRadianF = (float)(Math.PI / 180.0);

        public static double RadianToDegree(double angle)
        {
            return angle * RadianToDegreeD;
        }

        public static double DegreeToRadian(double angle)
        {
            return angle * DegreeToRadianD;
        }
    }
}
