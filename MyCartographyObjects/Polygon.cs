using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using ZZUtils;

namespace MyCartographyObjects
{
    public class Polygon : CartoObj, IPointy
    {

        #region MemberVars

        private List<Coordonnees> _coordonnees;
        private Color _backgroundColor;
        private Color _borderColor;
        private double _oppacity = 1;

        #endregion

        #region Properties

        public List<Coordonnees> Coordonnees
        {
            get { return _coordonnees; }
            set { _coordonnees = value; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        public double Oppacity
        {
            get { return _oppacity; }
            set { _oppacity = value; }
        }

        public int NbPoints {
            get {
                int total = 0;
                for (int i = 0; i < Coordonnees.Count(); i++) { // Read each coord
                    bool increment = true;
                    for (int j = 0; j < i; j++) { // Check if the currentCoord is already present in the past coords
                        if (Coordonnees[i].Id == Coordonnees[j].Id) { // If the id is the same, the coords are the same
                            increment = false;
                            break; // Stop the research and go for the next currentCoord
                        }
                    }
                    if (increment) total++;
                }
                return total;
            }
        }

        #endregion

        #region Constructors

        public Polygon(List<Coordonnees> coordCollection, Color backgroundColor, Color borderColor, double oppacity = 1) : base() // Main initialisation constructor
        {
            Coordonnees = coordCollection;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
            Oppacity = oppacity;
        }

        public Polygon(List<Coordonnees> coordCollection) : this(coordCollection, Colors.LightBlue, Colors.Blue) { } // Other initialisation constructor

        public Polygon(Polygon polygon) : this(polygon.Coordonnees, polygon.BackgroundColor, polygon.BorderColor, polygon.Oppacity) { } // Copy constructor - From a Polyline

        public Polygon(Polyline polyline) : this(polyline.Coordonnees) { } // Copy constructor - From a Polyline

        public Polygon() : this(new List<Coordonnees>(), Colors.LightBlue, Colors.Blue) { } // Default constructor

        #endregion

        #region Functions

        public override string ToString()
        {
            String toReturn = base.ToString() + "\n[#] Couleur de fond: " + BackgroundColor + "\n[#] Couleur des bordures: " + BorderColor + "\n[#] Oppacite: " + Oppacity + "\n[#] Coordonnees:\n";
            foreach (Coordonnees coordonnees in Coordonnees) {
                toReturn += "\t" + coordonnees.ToString() + "\n";
            }
            return toReturn;
        }

        public override void Draw()
        {
            base.Draw();
        }

        private bool IsValid()
        {
            if (NbPoints >= 3) { // Is at least a triangle, 3 coordinates
                for (int i = 0; i < NbPoints; i++) {
                    ZZCoordinate previousPoint = (ZZCoordinate)Coordonnees[ZZFunctions.nmod(i - 1, NbPoints)], anglePoint = (ZZCoordinate)Coordonnees[i], nextPoint = (ZZCoordinate)Coordonnees[(i + 1) % NbPoints];
                    double alpha = ZZMath.AngleFrom3Points(previousPoint, anglePoint, nextPoint);
                    if (alpha == 0) return false; // The angle formed by the 3 points isn't 0 degrees
                }
                return true;
            }
            return false;
        }

        /*
        public bool IsPointClose_BoundingBox(Coordonnees toCheck, double precision)
        {
            if (IsValid()) {
                double max_X = Coordonnees[0].Latitude, max_Y = Coordonnees[0].Longitude, min_X = Coordonnees[0].Latitude, min_Y = Coordonnees[0].Longitude;

                for (int i = 1; i < NbPoints; i++) {
                    if (Coordonnees[i].Latitude > max_X) max_X = Coordonnees[i].Latitude;
                    else if (Coordonnees[i].Latitude < min_X) min_X = Coordonnees[i].Latitude;
                    if (Coordonnees[i].Longitude > max_Y) max_Y = Coordonnees[i].Longitude;
                    else if (Coordonnees[i].Longitude < min_Y) min_Y = Coordonnees[i].Longitude;
                }

                if (toCheck.Latitude > min_X - precision && toCheck.Latitude < max_X + precision) {
                    if (toCheck.Longitude > min_Y - precision && toCheck.Longitude < max_Y + precision) {
                        return true;
                    }
                }
            }
            return false;
        }
        */

        public override bool IsPointClose(Coordonnees toCheck, double precision)
        {
            if (IsValid()) {
                // return Extend(precision).IsPointInside(toCheck);
                // Or, more precise:
                if (!IsPointInside(toCheck)) {
                    for (int i = 0; i < NbPoints; i++) {
                        if (ZZMath.GetDistancePointToLine((ZZCoordinate)Coordonnees[i], (ZZCoordinate)Coordonnees[(i + 1) % NbPoints], (ZZCoordinate)toCheck) < precision) return true;
                    }
                } else {
                    return true;
                }
            }
            return false;
        }

        public bool IsPointInside(Coordonnees toCheck)
        {
            if (IsValid()) {
                double sumAngle = 0;
                for (int i = 1; i <= NbPoints; i++) {
                    ZZCoordinate previousPoint = (ZZCoordinate)Coordonnees[i - 1], anglePoint = (ZZCoordinate)toCheck, nextPoint = (ZZCoordinate)Coordonnees[i % NbPoints];
                    double alpha = ZZMath.AngleFrom3Points(previousPoint, anglePoint, nextPoint);
                    double dx = anglePoint.X - previousPoint.X, dy = anglePoint.Y - previousPoint.Y;
                    int sign = 1;
                    if (dx == 0) { // Vertical line
                        if (dy > 0) { // The line goes upward
                            if (nextPoint.X > previousPoint.X) sign = -1;
                        } else { // The line goes downward
                            if (nextPoint.X < previousPoint.X) sign = -1;
                        }
                    } else {
                        double m = dy / dx, p = previousPoint.Y - m * previousPoint.X;
                        double slopeAngle = ZZMath.SlopeAngle(previousPoint, anglePoint);
                        if (slopeAngle < 90 || slopeAngle > 270) { // The point needs to be below the line
                            if (nextPoint.Y < m * nextPoint.Y + p) sign = -1;
                        } else { // The point needs to be above the line
                            if (nextPoint.Y > m * nextPoint.Y + p) sign = -1;
                        }
                    }
                    sumAngle += sign * alpha;
                }
                Console.WriteLine(sumAngle);
                return (sumAngle == 360); // To be inside, the total of the angles needs to equal 360
            }
            return false;
        }

        public Polygon Extend(double extentionSize)
        {
            if (IsValid()) {
                List<Coordonnees> newCoordCollection = new List<Coordonnees>();
                for (int i = 0; i < NbPoints; i++) {
                    ZZCoordinate previousPoint = (ZZCoordinate)Coordonnees[ZZFunctions.nmod(i - 1, NbPoints)], anglePoint = (ZZCoordinate)Coordonnees[i], nextPoint = (ZZCoordinate)Coordonnees[(i + 1) % NbPoints];
                    double alpha = ZZMath.AngleFrom3Points(previousPoint, anglePoint, nextPoint), beta = (180 - alpha) / 2.0;
                    double slopeAngle = ZZMath.SlopeAngle(previousPoint, anglePoint) + beta;
                    double distanceToAdd = extentionSize / Math.Cos(ZZMath.ToRadians(beta));

                    double newX = anglePoint.X + distanceToAdd * Math.Cos(ZZMath.ToRadians(slopeAngle));
                    double newY = anglePoint.Y + distanceToAdd * Math.Sin(ZZMath.ToRadians(slopeAngle));
                    newCoordCollection.Add(new Coordonnees(newX, newY));
                }
                return new Polygon(newCoordCollection, BackgroundColor, BorderColor, Oppacity);
            }
            return this;
        }

        public double GetPerimeter()
        {
            double perimeter = 0;

            for (int i = 0; i < NbPoints - 1; i++) {
                perimeter += ZZMath.GetDistance((ZZCoordinate) Coordonnees[i], (ZZCoordinate) Coordonnees[i + 1]);
            }
            return perimeter;
        }

        public int CompareTo(IPointy pointy)
        {
            return GetPerimeter().CompareTo(pointy.GetPerimeter());
        }

        #endregion

    }
}
