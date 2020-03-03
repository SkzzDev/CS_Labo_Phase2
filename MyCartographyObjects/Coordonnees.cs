using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZZUtils;

namespace MyCartographyObjects
{
    public class Coordonnees : CartoObj
    {

        #region MemberVars

        private double _latitude;
        private double _longitude;

        #endregion

        #region Properties

        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        #endregion

        #region Constructors

        public Coordonnees(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Coordonnees() : this(0, 0) { }

        public Coordonnees(Coordonnees coordonnee) : this(coordonnee.Latitude, coordonnee.Longitude) { }

        #endregion

        #region Functions

        public override string ToString()
        {
            return base.ToString() + " (" + Latitude.ToString("0.000") + " ; " + Longitude.ToString("0.000") + ")";
        }

        public override bool IsPointClose(Coordonnees toCheck, double precision)
        {
            return (ZZMath.GetDistance((ZZCoordinate)toCheck, (ZZCoordinate)this) < precision);
        }

        public static explicit operator ZZCoordinate(Coordonnees coord)
        {
            ZZCoordinate zzCoord = new ZZCoordinate();
            zzCoord.X = (double) coord.Latitude;
            zzCoord.Y = (double) coord.Longitude;
            zzCoord.Z = 0;
            return zzCoord;
        }

        #endregion

    }
}
