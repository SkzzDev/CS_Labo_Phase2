using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCartographyObjects
{
    public abstract class CartoObj : IIsPointClose
    {

        #region MemberVars

        private int _id;
        private static int _counter = 0;

        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }

        #endregion

        #region Constructors

        public CartoObj()
        {
            Id = ++Counter;
        }

        #endregion

        #region Functions

        public override string ToString()
        {
            return GetType() + " (#" + Id.ToString() + ")";
        }

        public virtual void Draw()
        {
            Console.WriteLine(this);
        }

        public abstract bool IsPointClose(Coordonnees clickCoord, double precision);

        public int GetCoordonneesNb()
        {
            if (this is Coordonnees) {
                return 1;
            } else if (this is IPointy) {
                IPointy pointy = this as IPointy;
                return pointy.NbPoints;
            }

            return -1;
        }

        #endregion

    }
}
