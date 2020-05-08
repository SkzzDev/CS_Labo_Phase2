using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mapping
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>

    public delegate void OptionsUpdateTool(object sender, OptionsEventArgs e);

    public partial class OptionsWindow : Window
    {

        #region Properties

        public OptionsUpdateTool send;

        public Brush LbInitBackground { get; set; }

        public Brush LbInitForeground { get; set; }

        public string InitPath { get; set; }

        #endregion

        #region Constructors

        public OptionsWindow(Brush lbBackground, Brush lbForeground, string path)
        {
            InitializeComponent();

            LbInitBackground = lbBackground;
            LbInitForeground = lbForeground;
            InitPath = path;

            TbPath.Text = path;
            CpBackground.SelectedColor = (Color)ColorConverter.ConvertFromString(lbBackground.ToString());
            CpForeground.SelectedColor = (Color)ColorConverter.ConvertFromString(lbForeground.ToString());
        }

        #endregion

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            SendUpdateEvent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            SendUpdateEvent();
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            send?.Invoke(this, new OptionsEventArgs((Color)ColorConverter.ConvertFromString(LbInitBackground.ToString()), (Color)ColorConverter.ConvertFromString(LbInitForeground.ToString()), InitPath));
            Close();
        }

        private void SendUpdateEvent()
        {
            send?.Invoke(this, new OptionsEventArgs((Color)CpBackground.SelectedColor, (Color)CpForeground.SelectedColor, TbPath.Text));
        }

    }
}
