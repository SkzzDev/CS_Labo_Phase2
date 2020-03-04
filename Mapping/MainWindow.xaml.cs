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

        public Point MouseDownPoisition { get; set; } = new Point();

        public List<Pushpin> Pushpins { get; set; } = new List<Pushpin>();

        public MyPersonnalMapData MapData { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            MapData = new MyPersonnalMapData("Florent", "Banneux");

            DrawMapdataElements();

            UpdateLbCartographyObjectsItemsSource();
        }

        private void DrawMapdataElements()
        {
            foreach (ICartoObj iCartoObj in MapData.CartoObjs) {
                if (iCartoObj is POI poi) {
                    Pushpin newPushpin = new Pushpin {
                        Location = new Location(poi.Latitude, poi.Longitude)
                    };
                    Pushpins.Add(newPushpin);
                    iCartoObj.Tag = newPushpin;
                    MyMap.Children.Add(newPushpin);
                }
            }
        }

        private void UpdateLbCartographyObjectsItemsSource()
        {
            LbCartographyObjects.ItemsSource = null;
            LbCartographyObjects.ItemsSource = MapData.CartoObjs;
        }

        private void MyMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownPoisition = new Point(e.GetPosition(MyMap).X, e.GetPosition(MyMap).Y);
        }

        private void MyMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouseUpPosition = new Point(e.GetPosition(MyMap).X, e.GetPosition(MyMap).Y);
            if (mouseUpPosition == MouseDownPoisition) {
                switch (EditMode) { // Selection mode
                    case 1:
                        bool elementSelected = false;
                        for (int i = MapData.CartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            ICartoObj iCartoObj = MapData.CartoObjs[i];
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = iCartoObj as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, 0.0008)) {
                                elementSelected = true;
                                LbCartographyObjects.SelectedItem = iCartoObj;
                                LbCartographyObjects.ScrollIntoView(iCartoObj);
                            }
                        }
                        if (!elementSelected) {
                            LbCartographyObjects.SelectedItem = null;
                        }
                        break;
                    case 2: // Add mode
                        Location ClickLocation;
                        MyMap.TryViewportPointToLocation(e.GetPosition(MyMap), out ClickLocation);
                        switch (CbActionType.SelectedIndex) {
                            case 0: // Point of interest
                                POI newPOI = new POI(ClickLocation.Latitude, ClickLocation.Longitude, "Default");
                                MapData.Add(newPOI);
                                Pushpin newPushpin = new Pushpin {
                                    Location = ClickLocation
                                };
                                Pushpins.Add(newPushpin);
                                MyMap.Children.Add(newPushpin);
                                newPOI.Tag = newPushpin;
                                UpdateLbCartographyObjectsItemsSource();
                                break;
                            case 1: // Travel
                                break;
                            case 2: // Surface
                                break;
                        }
                        break;
                    case 3: // Delete mode
                        ICartoObj toRemove = new POI();
                        for (int i = MapData.CartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            ICartoObj iCartoObj = MapData.CartoObjs[i];
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = iCartoObj as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, 0.0008)) {
                                toRemove = iCartoObj;
                                Pushpin PushpinToRemove = (Pushpin)iCartoObj.Tag;
                                Pushpins.Remove(PushpinToRemove);
                                MyMap.Children.Remove(PushpinToRemove);
                                UpdateLbCartographyObjectsItemsSource();
                                break;
                            }
                        }
                        MapData.Remove(toRemove);
                        break;
                    case 4: // Update mode
                        break;
                }
            }
        }

        private void ClickOnToolbarButton(int newEditMode)
        {
            if (newEditMode != 1)
                LbCartographyObjects.SelectedItem = null;
            if (newEditMode != EditMode) {
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
                EditMode = newEditMode;
                switch (newEditMode) {
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
            /* 
             * Made to center the POI selected on the map, but bug due to mouse movements etc
             * 
            if (LbCartographyObjects.SelectedItem is POI poi) {
                Location location = new Location(poi.Latitude, poi.Longitude);
                MyMap.SetView(location, MyMap.ZoomLevel);
            }
            */
        }

    }
}
