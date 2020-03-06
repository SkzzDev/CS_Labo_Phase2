using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using ZZUtils;

namespace MyCartographyObjects
{
    public class Polyline : CartoObj, IPointy, IEquatable<Polyline>, ICartoObj
    {

        #region MemberVars

        private List<Coordonnees> _coordonnees = new List<Coordonnees>();
        private Color _lineColor;
        private int _thickness;
        private double _oppacity;
        private string _description;
        private object _tag;

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

        public double Opacity
        {
            get { return _oppacity; }
            set { _oppacity = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
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

        public Polyline(Color lineColor, double opacity = 0.8, int thickness = 3, string description = "Travel") : base() // Main initialisation constructor
        {
            LineColor = lineColor;
            Opacity = opacity;
            Thickness = thickness;
            Description = description;
        }

        public Polyline(Polyline polyline) : this(polyline.LineColor, polyline.Opacity, polyline.Thickness, polyline.Description) // Copy constructor - From a polyline
        {
            foreach (Coordonnees coordonnee in polyline.Coordonnees) {
                Add(new Coordonnees(coordonnee));
            }
        }

        public Polyline(Polygon polygon) : this(polygon.BorderColor, polygon.Opacity, polygon.Thickness, polygon.Description) // Copy constructor - From a polygon
        {
            foreach (Coordonnees coordonnee in polygon.Coordonnees) {
                Add(new Coordonnees(coordonnee));
            }
        }

        public Polyline() : this(Colors.Red) { } // Default constructor

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

        public void Add(Coordonnees coordonneeToAdd)
        {
            Coordonnees.Add(coordonneeToAdd);
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

        public override string ToCSV()
        {
            string csv = "";
            foreach (Coordonnees currentCoord in Coordonnees) {
                if (currentCoord is POI poi) {
                    csv += poi.ToCSV();
                } else {
                    csv += currentCoord.ToCSV();
                }
                csv += "\r\n";
            }
            csv = csv.Substring(0, -1);
            return csv;
        }

        #endregion

    }
}
