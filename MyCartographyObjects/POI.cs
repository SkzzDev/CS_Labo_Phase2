using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyCartographyObjects
{

    [Serializable]
    public class POI : Coordonnees, ICartoObj, ISerializable
    {

        #region MemberVars

        private string _description;
        private Color _fill;
        private object _tag;

        #endregion

        #region Properties

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Color Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        #endregion

        #region Constructors

        public POI(double latitude, double longitude, string description, Color fill) : base(latitude, longitude) // 1 of 2 main initialisation constructor
        {
            Description = description;
            Fill = fill;
        }

        public POI(double latitude = 0, double longitude = 0, string description = "Default") : this(latitude, longitude, description, Colors.Red) { } // 2 of 2 main initialisation constructor

        public POI(string csv)
        {
            ImportCSV(csv);
        }

        public POI(POI poi) : this(poi.Latitude, poi.Longitude, poi.Description, poi.Fill) { } // Copy constructor

        public POI() : this(0, 0) { } // Default constructor

        public POI(SerializationInfo info, StreamingContext context) // Serialization constructor
        {
            byte R, G, B;
            Latitude = (double)info.GetValue("Latitude", typeof(double));
            Longitude = (double)info.GetValue("Longitude", typeof(double));

            R = (byte)info.GetValue("R", typeof(byte));
            G = (byte)info.GetValue("G", typeof(byte));
            B = (byte)info.GetValue("B", typeof(byte));
            Fill = Color.FromRgb(R, G, B);

            Description = (string)info.GetValue("Description", typeof(string));
        }

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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("R", Fill.R, typeof(byte));
            info.AddValue("G", Fill.G, typeof(byte));
            info.AddValue("B", Fill.B, typeof(byte));
            info.AddValue("Description", Description, typeof(string));
        }

        #endregion

    }
}
