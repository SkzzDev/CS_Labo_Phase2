using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mapping
{
    public class OptionsEventArgs : EventArgs
    {

        #region Properties

        public Color LbForeground { get; set; }
        public Color LbBackground { get; set; }
        public string Path { get; set; }

        #endregion

        #region Constructors

        public OptionsEventArgs(Color lbBackground, Color lbForeground, string path)
        {
            LbBackground = lbBackground;
            LbForeground = lbForeground;
            Path = path;
        }

        #endregion

    }
}
