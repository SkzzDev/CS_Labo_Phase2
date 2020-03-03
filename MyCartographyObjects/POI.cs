using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{
    public class POI : Coordonnees
    {

        #region MemberVars

        private string _description;

        #endregion

        #region Properties

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        #endregion

        #region Constructors

        public POI(double latitude, double longitude, string description) : base(latitude, longitude)
        {
            Description = description;
        }

        public POI(double latitude, double longitude) : base(latitude, longitude) { }

        public POI(string description) : this(50.6088641, 5.5110179, description) { }

        public POI() : this("HEPL Seraing") {}

        #endregion

        #region Functions

        public override string ToString()
        {
            return base.ToString() + " « " + Description + " »";
        }

        #endregion

    }
}
