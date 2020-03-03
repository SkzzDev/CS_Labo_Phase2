using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{
    public interface IPointy : IComparable<IPointy>
    {

        int NbPoints {
            get;
        }

        double GetPerimeter();

    }
}
