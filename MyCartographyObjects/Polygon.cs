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
    public class Polygon : CartoObj, IPointy, ICartoObj, ISerializable
    {

        #region MemberVars

        private List<Coordonnees> _coordonnees = new List<Coordonnees>();
        private Color _fill;
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

        public Color Fill
        {
            get { return _fill; }
            set { _fill = value; }
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

        public Polygon(Color fill, Color stroke, double oppacity = 0.8, int thickness = 3, string description = "Surface") : base() // Main initialisation constructor
        {
            Fill = fill;
            Stroke = stroke;
            Opacity = oppacity;
            Thickness = thickness;
            Description = description;
        }

        public Polygon(List<Coordonnees> coords) : base() // Random constructor
        {
            foreach (Coordonnees coord in coords)
                Add(new Coordonnees(coord));
        }

        public Polygon(Polygon polygon) : this(polygon.Fill, polygon.Stroke, polygon.Opacity, polygon.Thickness, polygon.Description) // Copy constructor - From a Polyline
        {
            foreach (Coordonnees coordonnee in polygon.Coordonnees) {
                Coordonnees.Add(new Coordonnees(coordonnee));
            }
        }

        public Polygon(Polyline polyline) : this(Colors.Red, polyline.Stroke, polyline.Opacity, polyline.Thickness, polyline.Description) // Copy constructor - From a Polyline
        {
            foreach (Coordonnees coordonnee in polyline.Coordonnees) {
                Coordonnees.Add(new Coordonnees(coordonnee));
            }
        }

        public Polygon() : this(Colors.Red, Colors.DarkRed) { } // Default constructor
        public Polygon(SerializationInfo info, StreamingContext context) // Serialization constructor
        {
            byte R, G, B;
            Coordonnees = (List<MyCartographyObjects.Coordonnees>)info.GetValue("Coordonnees", typeof(List<Coordonnees>));

            R = (byte)info.GetValue("BG_R", typeof(byte));
            G = (byte)info.GetValue("BG_G", typeof(byte));
            B = (byte)info.GetValue("BG_B", typeof(byte));
            Fill = Color.FromRgb(R, G, B);

            R = (byte)info.GetValue("BD_R", typeof(byte));
            G = (byte)info.GetValue("BD_G", typeof(byte));
            B = (byte)info.GetValue("BD_B", typeof(byte));
            Stroke = Color.FromRgb(R, G, B);

            Thickness = (int)info.GetValue("Thickness", typeof(int));
            Opacity = (double)info.GetValue("Opacity", typeof(double));
            Description = (string)info.GetValue("Description", typeof(string));
        }

        #endregion

        #region Functions

        public override string ToString()
        {
            String toReturn = base.ToString() + "\n[#] Couleur de fond: " + Fill + "\n[#] Couleur des bordures: " + Stroke + "\n[#] Oppacite: " + Opacity + "\n[#] Coordonnees:\n";
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
                        double slopeAngle = ZZMath.SlopeAngle(anglePoint, previousPoint);
                        double fx = m * nextPoint.X + p;
                        if (slopeAngle < 90 || slopeAngle > 270) { // The point needs to be below the line
                            if (nextPoint.Y > fx) sign = -1;
                        } else { // The point needs to be above the line
                            if (nextPoint.Y < fx) sign = -1;
                        }
                    }
                    sumAngle += sign * alpha;
                }
                return ((sumAngle > 359.9 && sumAngle < 360.1) || (sumAngle < -359.9 && sumAngle > -360.1)); // To be inside, the total of the angles needs to be above 359.9 or below 359.9, depending on the rotation sense made when creating the polygon (basicly == 360 but double precision doesn't always makes it work)
            }
            return false;
        }

        public Polygon Extend(double extentionSize)
        {
            if (IsValid()) {
                Polygon toReturn = new Polygon(Fill, Stroke, Opacity);
                for (int i = 0; i < NbPoints; i++) {
                    ZZCoordinate previousPoint = (ZZCoordinate)Coordonnees[ZZFunctions.nmod(i - 1, NbPoints)], anglePoint = (ZZCoordinate)Coordonnees[i], nextPoint = (ZZCoordinate)Coordonnees[(i + 1) % NbPoints];
                    double alpha = ZZMath.AngleFrom3Points(previousPoint, anglePoint, nextPoint), beta = (180 - alpha) / 2.0;
                    double slopeAngle = ZZMath.SlopeAngle(previousPoint, anglePoint) + beta;
                    double distanceToAdd = extentionSize / Math.Cos(ZZMath.ToRadians(beta));

                    double newX = anglePoint.X + distanceToAdd * Math.Cos(ZZMath.ToRadians(slopeAngle));
                    double newY = anglePoint.Y + distanceToAdd * Math.Sin(ZZMath.ToRadians(slopeAngle));
                    toReturn.Add(new Coordonnees(newX, newY));
                }
                return toReturn;
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

        public Coordonnees GetCenter()
        {
            Coordonnees topLeft = GetTopLeft(), bottomRight = GetBottomRight();
            ZZCoordinate centerOfSegment = ZZMath.GetCenterOfSegment((ZZCoordinate)topLeft, (ZZCoordinate)bottomRight);
            return new Coordonnees(centerOfSegment.Y, centerOfSegment.X);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Coordonnees", this.Coordonnees, typeof(List<Coordonnees>));

            info.AddValue("BG_R", Fill.R, typeof(byte));
            info.AddValue("BG_G", Fill.G, typeof(byte));
            info.AddValue("BG_B", Fill.B, typeof(byte));

            info.AddValue("BD_R", Stroke.R, typeof(byte));
            info.AddValue("BD_G", Stroke.G, typeof(byte));
            info.AddValue("BD_B", Stroke.B, typeof(byte));

            info.AddValue("Thickness", Thickness, typeof(int));
            info.AddValue("Opacity", Opacity, typeof(double));
            info.AddValue("Description", Description, typeof(string));
        }

        #endregion

    }
}
