using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using ZZUtils;

namespace MyCartographyObjects
{
    public class Polyline : CartoObj, IPointy, IEquatable<Polyline>
    {

        #region MemberVars

        private List<Coordonnees> _coordonnees;
        private Color _lineColor;
        private int _thickness;

        #endregion

        #region Properties

        public List<Coordonnees> Coordonnees
        {
            get { return _coordonnees; }
            set { _coordonnees = value; }
        }

        public Color LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        public int Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        public int NbPoints
        {
            get {
                int total = 0;
                for (int i = 0; i < Coordonnees.Count(); i++) { // Read each coord
                    bool increment = true;
                    for (int j = 0; j < i; j++) { // Check if the currentCoord is already present in the past coords
                        if (Coordonnees[i].Id == Coordonnees[j].Id) {
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

        public Polyline(List<Coordonnees> coordCollection, Color lineColor, int thickness = 1) : base() // Main initialisation constructor
        {
            Coordonnees = coordCollection;
            LineColor = lineColor;
            Thickness = thickness;
        }

        public Polyline(List<Coordonnees> coordCollection) : this(coordCollection, Colors.WhiteSmoke) { }  // Other initialisation constructor

        public Polyline(Polyline polyline) : this(polyline.Coordonnees, polyline.LineColor, polyline.Thickness) { }  // Copy constructor - From a polyline

        public Polyline(Polygon polygon) : this(polygon.Coordonnees, Colors.WhiteSmoke) { }  // Copy constructor - From a polygon

        public Polyline() : this(new List<Coordonnees>(), Colors.WhiteSmoke) { } // Default constructor

        #endregion

        #region Functions

        public override string ToString()
        {
            String toReturn = base.ToString() + "\n[#] Couleur des traits: " + LineColor + "\n[#] Epaisseur des traits: " + Thickness + "\n[#] Coordonnees:\n";
            foreach (Coordonnees coordonnees in Coordonnees) {
                toReturn += "\t" + coordonnees.ToString() + "\n";
            }
            return toReturn;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override bool IsPointClose(Coordonnees toCheck, double precision)
        {
            for (int i = 0; i < Coordonnees.Count() - 1; i++) {
                if (ZZMath.GetDistancePointToLine((ZZCoordinate)Coordonnees[i], (ZZCoordinate)Coordonnees[i + 1], (ZZCoordinate)toCheck) < precision) return true;
            }

            return false;
        }

        public double GetBoundingBoxArea()
        {
            List<Coordonnees> coords = Coordonnees;

            Coordonnees topLeft = new Coordonnees(coords[0]), bottomRight = new Coordonnees(coords[0]);

            for (int i = 1; i < Coordonnees.Count(); i++) {
                if (coords[i].Latitude > bottomRight.Latitude) bottomRight.Latitude = coords[i].Latitude;
                else if (coords[i].Latitude < topLeft.Latitude) topLeft.Latitude = coords[i].Latitude;
                if (coords[i].Longitude > topLeft.Longitude) topLeft.Longitude = coords[i].Longitude;
                else if (coords[i].Longitude < bottomRight.Longitude) bottomRight.Longitude = coords[i].Longitude;
            }

            return (bottomRight.Latitude - topLeft.Latitude) * (topLeft.Longitude - bottomRight.Longitude);
        }

        public double GetPerimeter()
        {
            double perimeter = 0;

            for (int i = 0; i < Coordonnees.Count() - 1; i++) {
                perimeter += ZZMath.GetDistance((ZZCoordinate) Coordonnees[i], (ZZCoordinate) Coordonnees[i + 1]);
            }

            return perimeter;
        }

        public int CompareTo(IPointy pointy) // Compare polylines with their perimeter
        {
            return GetPerimeter().CompareTo(pointy.GetPerimeter());
        }

        public bool Equals(Polyline other)
        {
            return CompareTo(other) == 0;
        }

        #endregion

    }
}
