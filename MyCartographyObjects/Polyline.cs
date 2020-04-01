using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using ZZUtils;

namespace MyCartographyObjects
{

    [Serializable]
    public class Polyline : CartoObj, IPointy, IEquatable<Polyline>, ICartoObj, ISerializable
    {

        #region MemberVars

        private List<Coordonnees> _coordonnees = new List<Coordonnees>();
        private Color _stroke;
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

        public Color Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
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

        public Polyline(Color stroke, double opacity = 0.8, int thickness = 3, string description = "Travel") : base() // Main initialisation constructor
        {
            Stroke = stroke;
            Opacity = opacity;
            Thickness = thickness;
            Description = description;
        }

        public Polyline(Polyline polyline) : this(polyline.Stroke, polyline.Opacity, polyline.Thickness, polyline.Description) // Copy constructor - From a polyline
        {
            foreach (Coordonnees coordonnee in polyline.Coordonnees) {
                Add(new Coordonnees(coordonnee));
            }
        }

        public Polyline(Polygon polygon) : this(polygon.Stroke, polygon.Opacity, polygon.Thickness, polygon.Description) // Copy constructor - From a polygon
        {
            foreach (Coordonnees coordonnee in polygon.Coordonnees) {
                Add(new Coordonnees(coordonnee));
            }
        }

        public Polyline() : this(Colors.Red) { } // Default constructor

        public Polyline(SerializationInfo info, StreamingContext context) // Serialization constructor
        {
            byte R, G, B;
            Coordonnees = (List<MyCartographyObjects.Coordonnees>)info.GetValue("Coordonnees", typeof(List<Coordonnees>));

            R = (byte)info.GetValue("R", typeof(byte));
            G = (byte)info.GetValue("G", typeof(byte));
            B = (byte)info.GetValue("B", typeof(byte));
            Stroke = Color.FromRgb(R, G, B);

            Thickness = (int)info.GetValue("Thickness", typeof(int));
            Opacity = (double)info.GetValue("Opacity", typeof(double));
            Description = (string)info.GetValue("Description", typeof(string));
        }

        #endregion

        #region Functions

        public override string ToString()
        {
            String toReturn = base.ToString() + "\n[#] Couleur des traits: " + Stroke + "\n[#] Epaisseur des traits: " + Thickness + "\n[#] Coordonnees:\n";
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

        private Coordonnees GetTopLeft()
        {
            Coordonnees topLeft = new Coordonnees();
            if (NbPoints > 0) {
                topLeft = new Coordonnees(Coordonnees[0]);
                for (int i = 1; i < Coordonnees.Count(); i++) {
                    if (Coordonnees[i].Latitude < topLeft.Latitude) topLeft.Latitude = Coordonnees[i].Latitude;
                    if (Coordonnees[i].Longitude > topLeft.Longitude) topLeft.Longitude = Coordonnees[i].Longitude;
                }
            }
            return topLeft;
        }

        private Coordonnees GetBottomRight()
        {
            Coordonnees bottomRight = new Coordonnees();
            if (NbPoints > 0) {
                bottomRight = new Coordonnees(Coordonnees[0]);
                for (int i = 1; i < Coordonnees.Count(); i++) {
                    if (Coordonnees[i].Latitude > bottomRight.Latitude) bottomRight.Latitude = Coordonnees[i].Latitude;
                    if (Coordonnees[i].Longitude < bottomRight.Longitude) bottomRight.Longitude = Coordonnees[i].Longitude;
                }
            }
            return bottomRight;
        }

        public double GetBoundingBoxArea()
        {
            if (NbPoints > 0) {
                Coordonnees topLeft = GetTopLeft(), bottomRight = GetBottomRight();

                return (bottomRight.Latitude - topLeft.Latitude) * (topLeft.Longitude - bottomRight.Longitude);
            }
            return 0;
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

        public Coordonnees GetCenter()
        {
            Coordonnees topLeft = GetTopLeft(), bottomRight = GetBottomRight();
            ZZCoordinate centerOfSegment = ZZMath.GetCenterOfSegment((ZZCoordinate)topLeft, (ZZCoordinate)bottomRight);
            return new Coordonnees(centerOfSegment.Y, centerOfSegment.X);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Coordonnees", this.Coordonnees, typeof(List<Coordonnees>));

            info.AddValue("R", Stroke.R, typeof(byte));
            info.AddValue("G", Stroke.G, typeof(byte));
            info.AddValue("B", Stroke.B, typeof(byte));

            info.AddValue("Thickness", Thickness, typeof(int));
            info.AddValue("Opacity", Opacity, typeof(double));
            info.AddValue("Description", Description, typeof(string));
        }

        #endregion

    }
}
