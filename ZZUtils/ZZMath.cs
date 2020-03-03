using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZUtils
{
    public static class ZZMath
    {

        #region Functions

        public static double GetDistance(ZZCoordinate coord1, ZZCoordinate coord2) => System.Math.Sqrt(System.Math.Pow(coord1.X - coord2.X, 2) + System.Math.Pow(coord1.Y - coord2.Y, 2));

        public static double GetDistance(ZZCoordinate coord)
        {
            ZZCoordinate zero = new ZZCoordinate(0, 0);
            return System.Math.Sqrt(System.Math.Pow(zero.X - coord.X, 2) + System.Math.Pow(zero.Y - coord.Y, 2));
        }

        public static double GetDistance(double x, double y) => System.Math.Sqrt(System.Math.Pow(0 - x, 2) + System.Math.Pow(0 - y, 2));

        public static double GetDistance(double x1, double y1, double x2, double y2) => System.Math.Sqrt(System.Math.Pow(x1 - x2, 2) + System.Math.Pow(y1 - y2, 2));

        // http://villemin.gerard.free.fr/GeomLAV/Triangle/Calcul/RelQuel_fichiers/image089.jpg
        // C = point to check, A = first point of line, B = second point of line
        public static double GetDistancePointToLine(ZZCoordinate firstPoint, ZZCoordinate secondPoint, ZZCoordinate toCheck)
        {
            // Calculation of the triangle' sides' length
            double b = ZZMath.GetDistance(firstPoint, toCheck);
            double c = ZZMath.GetDistance(firstPoint, secondPoint);

            // Calculation of one of the angle attached to the straight line
            double angleFirstPoint = ZZMath.AngleFrom3Points(secondPoint, firstPoint, toCheck, true);
            // double angleA = angleARadian / (Math.PI / 180.0); // Falcultative: to know the angle in degrees

            double h = b * Math.Sin(angleFirstPoint); // Perpendicular distance

            double c1 = b * Math.Cos(angleFirstPoint); // Distance between firstPoint base of h (can be negative)
            double c2 = c - c1; // Distance between secondPoint base of h (can be negative)

            double clickCoordToFirstPoint = GetDistance(c1, h); // Distance between toCheck and firstPoint
            double clickCoordToSecondPoint = GetDistance(c2, h); // Distance between toCheck and secondPoint

            if (c1 >= 0 && c2 >= 0) { // toCheck is between firstPoint and secondPoint perpendicularly
                return h;
            } else { // toCheck isn't betwwen firstPoint and secondPoint
                return (clickCoordToFirstPoint < clickCoordToSecondPoint) ? clickCoordToFirstPoint : clickCoordToSecondPoint;
            }
        }

        // https://stackoverflow.com/questions/1211212/how-to-calculate-an-angle-from-three-points
        // http://villemin.gerard.free.fr/GeomLAV/Triangle/Calcul/RelQuel_fichiers/image089.jpg
        public static double AngleFrom3Points(ZZCoordinate pointBefore, ZZCoordinate anglePoint, ZZCoordinate pointAfter, bool degres = true)
        {
            // Calculation of the triangle' sides' length
            double a = GetDistance(pointBefore, pointAfter);
            double b = GetDistance(anglePoint, pointAfter);
            double c = GetDistance(anglePoint, pointBefore);

            // Calculation of one of the angle attached to the straight line
            double alpha = Math.Acos((-Math.Pow(a, 2) + Math.Pow(b, 2) + Math.Pow(c, 2)) / (2 * b * c));
            return (degres) ? alpha * (180 / Math.PI) : alpha;
        }

        public static double SlopeAngle(ZZCoordinate firstPoint, ZZCoordinate nextPoint, bool degres = true)
        {
            double dx = nextPoint.X - firstPoint.X, dy = nextPoint.Y - firstPoint.Y;
            double slopeAngle;

            if (dx == 0) { // Vertical slope
                slopeAngle = (dy > 0) ? 90 : -90; // Upward / Downward
            } else if (dy == 0) { // Horizontal slope
                slopeAngle = (dx > 0) ? 0 : 180; // Right / Left
            } else {
                slopeAngle = ZZMath.ToDegrees(Math.Atan(dy / dx));
                if (dx < 0)
                    slopeAngle += 180;
                else if (dy < 0)
                    slopeAngle += 360; // Because ATan return a negative angle in the 2th and 4th quarter, but the 2th is made positive thanks to the +180
            }

            return (degres) ? slopeAngle : ZZMath.ToRadians(slopeAngle);
        }

        public static double ToRadians(double degreeAngle)
        {
            return degreeAngle * (Math.PI / 180);
        }

        public static double ToDegrees(double radianAngle)
        {
            return radianAngle * (180 / Math.PI);
        }

        #endregion

    }
}
