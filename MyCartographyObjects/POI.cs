using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyCartographyObjects
{
    public class POI : Coordonnees, ICartoObj
    {

        #region MemberVars

        private string _description;
        private Color _backgroundColor;
        private object _tag;

        #endregion

        #region Properties

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        #endregion

        #region Constructors

        public POI(double latitude, double longitude, string description, Color backgroundColor) : base(latitude, longitude)
        {
            Description = description;
            BackgroundColor = backgroundColor;
        }

        public POI(double latitude = 0, double longitude = 0, string description = "Default") : this(latitude, longitude, description, Colors.Red) { }

        public POI(string csv)
        {
            ImportCSV(csv);
        }

        public POI(POI poi) : this(poi.Latitude, poi.Longitude, poi.Description, poi.BackgroundColor) { }

        public POI() : this(0, 0) { }

        #endregion

        #region Functions

        public override string ToString()
        {
            return base.ToString() + " « " + Description + " »";
        }

        public override string ToCSV()
        {
            return base.ToCSV() + ";" + Description;
        }

        public bool ImportCSV(string csv)
        {
            int commaCount = csv.Count(f => f == ';');
            if (commaCount >= 2) {
                int commaPos = csv.IndexOf(';');
                Latitude = Convert.ToDouble(csv.Substring(0, commaPos));
                csv = csv.Substring(commaPos + 1);
                commaPos = csv.IndexOf(';');
                if (commaPos == -1) {
                    Longitude = Convert.ToDouble(csv);
                } else {
                    Longitude = Convert.ToDouble(csv.Substring(0, commaPos));
                    Description = csv.Substring(commaPos + 1).TrimEnd('\r', '\n'); ;
                }
                return true;
            }
            return false;
        }

        public Coordonnees GetCenter()
        {
            return new Coordonnees(Latitude, Longitude);
        }

        #endregion

    }
}
