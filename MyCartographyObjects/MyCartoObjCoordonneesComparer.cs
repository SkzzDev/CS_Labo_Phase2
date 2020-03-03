using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{
    public class MyCartoObjCoordonneesComparer : IComparer<CartoObj>
    {
        public int Compare(CartoObj x, CartoObj y)
        {
            return x.GetCoordonneesNb().CompareTo(y.GetCoordonneesNb());
        }

    }
}
