using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{

    public interface ICartoObj
    {

        #region Properties

        object Tag { get; set; }

        string Description { get; set; }

        #endregion

        #region Functions

        string ToCSV();

        Coordonnees GetCenter();

        #endregion

    }
}
