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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;

using MyCartographyObjects;

namespace Mapping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region MemberVars

        private int _editMode = 1; // 1: Select, 2: Add, 3: Delete, 4: Update
        private List<CartoObj> _cartoObjs = new List<CartoObj>();

        #endregion

        #region Properties

        public int EditMode
        {
            get { return _editMode; }
            set { if (value >= 1 && value <= 4) _editMode = value; }
        }

        public List<CartoObj> CartoObjs
        {
            get { return _cartoObjs; }
            set { _cartoObjs = value; }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            CartoObjs = new List<CartoObj>() {
                new POI(50.460554, 5.649703, "Maison"),
                new POI(50.611265, 5.511353, "École"),
                new POI(50.624466, 5.566776, "Liège Guillemin")
            };
            List<Pushpin> pushpins = new List<Pushpin>() { };

            LbCartographyObjects.ItemsSource = CartoObjs;
            LbCartographyObjects.SelectionChanged += LbCartographyObjects_SelectedIndexChanged;

            foreach (CartoObj cartoObj in CartoObjs) {
                if (cartoObj is POI poi) {
                    Pushpin newPushpin = new Pushpin();
                    newPushpin.Location = new Location(poi.Latitude, poi.Longitude);
                    pushpins.Add(newPushpin);
                }
            }

            foreach (Pushpin pushpin in pushpins) {
                MyMap.Children.Add(pushpin);
            }

            MyMap.Mode = new AerialMode(true);

            MyMap.MouseLeftButtonUp +=
                new MouseButtonEventHandler(MyMap_MouseLeftButtonUp);

            BtnSelect.Click += BtnSelect_Click;
            BtnAdd.Click += BtnAdd_Click;
            BtnRemove.Click += BtnRemove_Click;
            BtnUpdate.Click += BtnUpdate_Click;
        }

        void MyMap_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (EditMode == 1) { // Selection mode
                foreach (CartoObj cartoObj in CartoObjs) {
                    // e.GetPosition(MyMap);
                    Coordonnees clickCoord = new Coordonnees();
                    if (cartoObj.IsPointClose(clickCoord, 20) || 1 == 1) {
                        // To do
                    }
                }

                // Temporary
                if (CartoObjs[0] is Coordonnees coordonnee) {
                    Location location = new Location(coordonnee.Latitude, coordonnee.Longitude);
                    MyMap.SetView(location, MyMap.ZoomLevel);
                }
            }
        }

        private void ClickOnToolbarButton(int editMode)
        {
            if (editMode != EditMode) {
                // Remove old selected button
                Button clickedButton = BtnSelect;
                switch (EditMode) {
                    // No case 1 because its the Select one by default (2 line upper)
                    case 2:
                        clickedButton = BtnAdd;
                        break;
                    case 3:
                        clickedButton = BtnRemove;
                        break;
                    case 4:
                        clickedButton = BtnUpdate;
                        break;
                }
                clickedButton.Background = Brushes.LightGray;
                clickedButton.Foreground = Brushes.Black;

                // Set new selected button
                EditMode = editMode;
                switch (editMode) {
                    case 1:
                        BtnSelect.Background = Brushes.DodgerBlue;
                        BtnSelect.Foreground = Brushes.White;
                        break;
                    case 2:
                        BtnAdd.Background = Brushes.Green;
                        BtnAdd.Foreground = Brushes.White;
                        break;
                    case 3:
                        BtnRemove.Background = Brushes.DarkRed;
                        BtnRemove.Foreground = Brushes.White;
                        break;
                    case 4:
                        BtnUpdate.Background = Brushes.Orange;
                        BtnUpdate.Foreground = Brushes.White;
                        break;
                }
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(1);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(2);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(3);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(4);
        }

        private void LbCartographyObjects_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            CartoObj cartoObjSelected = (CartoObj)LbCartographyObjects.SelectedItem;
            if (cartoObjSelected is Coordonnees coordonnee) {
                Location location = new Location(coordonnee.Latitude, coordonnee.Longitude);
                MyMap.SetView(location, MyMap.ZoomLevel);
            }
        }
    }
}
