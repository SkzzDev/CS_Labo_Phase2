using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{
    public class MyPolylineBoundingBoxComparer : IComparer<Polyline>
    {

        public int Compare(Polyline x, Polyline y)
        {
            return x.GetBoundingBoxArea().CompareTo(y.GetBoundingBoxArea());
        }
    }
}
