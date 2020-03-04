using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #endregion

        #region Properties

        public int EditMode
        {
            get { return _editMode; }
            set { if (value >= 1 && value <= 4) _editMode = value; }
        }

        public List<Pushpin> Pushpins { get; set; } = new List<Pushpin>();

        public MyPersonnalMapData Mapdata { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Mapdata = new MyPersonnalMapData("Florent", "Banneux");

            Mapdata.Add(new POI(50.460554, 5.649703, "Maison"));
            Mapdata.Add(new POI(50.611265, 5.511353, "École"));
            Mapdata.Add(new POI(50.624466, 5.566776, "Liège Guillemin"));

            foreach (ICartoObj iCartoObj in Mapdata.CartoObjs) {
                if (iCartoObj is POI poi) {
                    Pushpin newPushpin = new Pushpin {
                        Location = new Location(poi.Latitude, poi.Longitude)
                    };
                    Pushpins.Add(newPushpin);
                    iCartoObj.Tag = newPushpin;
                }
            }

            LbCartographyObjects.ItemsSource = Mapdata.CartoObjs;

            foreach (Pushpin pushpin in Pushpins) {
                MyMap.Children.Add(pushpin);
            }
        }

        private void MyMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (EditMode) { // Selection mode
                case 1:
                    foreach (CartoObj cartoObj in Mapdata.CartoObjs) {
                        Point clickPoint = e.GetPosition(MyMap);
                        MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                        Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                        if (cartoObj.IsPointClose(clickCoordonnees, 0.001)) {
                            // LbCartographyObjects
                            // Change selected item in the ListBox
                        }
                    }
                    break;
                case 2: // Add mode
                    Location ClickLocation;
                    MyMap.TryViewportPointToLocation(e.GetPosition(MyMap), out ClickLocation);
                    switch (CbActionType.SelectedIndex) {
                        case 0: // Point of interest
                            POI newPOI = new POI(ClickLocation.Latitude, ClickLocation.Longitude, "Default");
                            Mapdata.Add(newPOI);
                            Pushpin newPushpin = new Pushpin {
                                Location = ClickLocation
                            };
                            Pushpins.Add(newPushpin);
                            MyMap.Children.Add(newPushpin);
                            LbCartographyObjects.ItemsSource = null;
                            LbCartographyObjects.ItemsSource = Mapdata.CartoObjs;
                            break;
                        case 1: // Travel
                            break;
                        case 2: // Surface
                            break;
                        default:
                            MessageBox.Show(CbActionType.SelectedIndex.ToString());
                            break;
                    }
                    break;
                case 3: // Delete mode
                    foreach (CartoObj cartoObj in Mapdata.CartoObjs) {
                        Point clickPoint = e.GetPosition(MyMap);
                        MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                        Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                        if (cartoObj.IsPointClose(clickCoordonnees, 0.001)) {
                            MessageBox.Show("Near object!");
                            ICartoObj iCartoObj = cartoObj as ICartoObj;
                            Mapdata.Remove(iCartoObj);
                            Pushpins.Remove((Pushpin)iCartoObj.Tag);
                            LbCartographyObjects.ItemsSource = null;
                            LbCartographyObjects.ItemsSource = Mapdata.CartoObjs;
                        }
                    }
                    break;
                case 4: // Update mode
                    break;
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

        private void LbCartographyObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CartoObj cartoObjSelected = (CartoObj)LbCartographyObjects.SelectedItem;
            if (cartoObjSelected is Coordonnees coordonnee) {
                Location location = new Location(coordonnee.Latitude, coordonnee.Longitude);
                MyMap.SetView(location, MyMap.ZoomLevel);
            }
        }
    }
}
