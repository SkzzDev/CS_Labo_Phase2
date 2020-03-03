using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZUtils
{
    public static class ZZFunctions
    {

        #region Functions

        public static int nmod(int toMod, int mod)
        {
            if (toMod >= 0) return toMod % mod;
            return mod - (-toMod % mod);
        }

        public static double nmod(double toMod, double mod)
        {
            while (toMod - mod >= 0) toMod -= mod;
            while (toMod < 0) toMod += mod;
            return toMod;
        }

        #endregion

    }
}
