using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZUtils
{
    public class ZZCoordinate
    {

        #region MemberVars

        private double _x;
        private double _y;
        private double _z;

        #endregion

        #region Properties

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        #endregion

        #region Constructors

        public ZZCoordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public ZZCoordinate(double x, double y) : this(x, y, 0) { }

        public ZZCoordinate() : this(0, 0, 0) { }

        #endregion

        #region Functions

        public override string ToString()
        {
            return "[#] " + GetType() + ": (" + X.ToString("0.00") + " ; " + Y.ToString("0.00") + ")";
        }

        #endregion

    }
}
