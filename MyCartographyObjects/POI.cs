using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{
    public class POI : Coordonnees, ICartoObj
    {

        #region MemberVars

        private string _description;
        private object _tag;

        #endregion

        #region Properties

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

        #endregion

        #region Constructors

        public POI(double latitude = 0, double longitude = 0, string description = "Default") : base(latitude, longitude)
        {
            Description = description;
        }

        public POI(POI poi) : this(poi.Latitude, poi.Longitude, poi.Description) { }

        #endregion

        #region Functions

        public override string ToString()
        {
            return base.ToString() + " « " + Description + " »";
        }

        #endregion

    }
}
