using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private int _editMode = 1; // 1: Select, 2: Add, 3: Remove, 4: Update
        public static double _PRECISION = 0.0005;

        #endregion

        #region Properties

        public int EditMode
        {
            get { return _editMode; }
            set { if (value >= 1 && value <= 4) _editMode = value; }
        }

        #region Surface

        public MyCartographyObjects.Polygon CurrentSurface { get; set; }
        public bool IsDrawingSurface { get; set; } = false;

        #endregion

        #region Travel

        public MyCartographyObjects.Polyline CurrentTravel { get; set; }
        public bool IsDrawingTravel { get; set; } = false;

        #endregion

        public Point MouseLeftButtonDownPoisition { get; set; } = new Point();

        public MyPersonnalMapData MapData { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            MapData = new MyPersonnalMapData(MyMap, "Florent", "Banneux");

            CurrentTravel = new MyCartographyObjects.Polyline();
            CurrentTravel.Tag = new MapPolyline() {
                Stroke = new SolidColorBrush(CurrentTravel.LineColor),
                StrokeThickness = CurrentTravel.Thickness,
                Opacity = CurrentTravel.Opacity,
                Locations = new LocationCollection()
            };
            CurrentSurface = new MyCartographyObjects.Polygon();
            CurrentSurface.Tag = new MapPolygon() {
                Fill = new SolidColorBrush(CurrentSurface.BackgroundColor),
                Stroke = new SolidColorBrush(CurrentSurface.BorderColor),
                StrokeThickness = CurrentSurface.Thickness,
                Opacity = CurrentSurface.Opacity,
                Locations = new LocationCollection()
            };

            DrawMapdataElements();

            UpdateLbCartographyObjectsItemsSource();
        }

        #region Functions

        private void DrawMapdataElements()
        {
            foreach (ICartoObj iCartoObj in MapData.CartoObjs) {
                if (iCartoObj is POI poi) {
                    Pushpin newPushpin = new Pushpin {
                        Location = new Location(poi.Latitude, poi.Longitude)
                    };
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

        private void ResetCurrentSurface()
        {
            IsDrawingSurface = false;
            CurrentSurface.Coordonnees.Clear();
            MapPolygon mapPolygon = (MapPolygon)CurrentSurface.Tag;
            mapPolygon.Locations.Clear();
            MyMap.Children.Remove(mapPolygon);
        }

        private void ResetCurrentTravel()
        {
            IsDrawingTravel = false;
            CurrentTravel.Coordonnees.Clear();
            MapPolyline mapPolyline = (MapPolyline)CurrentTravel.Tag;
            mapPolyline.Locations.Clear();
            MyMap.Children.Remove(mapPolyline);
        }

        private void ResetCurrentItems()
        {
            ResetCurrentSurface();
            ResetCurrentTravel();
        }

        private void AddPointToSurface(Location clickLocation)
        {
            MapPolygon mapPolygon = (MapPolygon)(CurrentSurface.Tag);
            if (!IsDrawingSurface) {
                ResetCurrentTravel();
                IsDrawingSurface = true;
                MyMap.Children.Add(mapPolygon);
            }
            bool searchCoordsNearClick = true;
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
                searchCoordsNearClick = false;
            }
            Coordonnees newCoordonnee = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
            Location newLocation = new Location(clickLocation);
            if (searchCoordsNearClick) {
                GetCoordonneesAndLocationNearClick(clickLocation, ref newCoordonnee, ref newLocation);
            }
            CurrentSurface.Coordonnees.Add(newCoordonnee);
            mapPolygon.Locations.Add(newLocation);
        }

        private void AddPointToTravel(Location clickLocation)
        {
            MapPolyline mapPolyline = (MapPolyline)(CurrentTravel.Tag);
            if (!IsDrawingTravel) {
                ResetCurrentSurface();
                IsDrawingTravel = true;
                MyMap.Children.Add(mapPolyline);
            }
            bool searchCoordsNearClick = true;
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) {
                searchCoordsNearClick = false;
            }
            Coordonnees newCoordonnee = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
            Location newLocation = new Location(clickLocation);
            if (searchCoordsNearClick) {
                GetCoordonneesAndLocationNearClick(clickLocation, ref newCoordonnee, ref newLocation);
            }
            CurrentTravel.Coordonnees.Add(newCoordonnee);
            mapPolyline.Locations.Add(newLocation);
        }

        private void GetCoordonneesAndLocationNearClick(Location clickLocation, ref Coordonnees newCoordonnee, ref Location newLocation)
        {
            bool stop = false;
            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
            for (int i = MapData.CartoObjs.Count() - 1; i >= 0 && !stop; i--) { // Reverse loop because the last element is the first showed on screen
                ICartoObj iCartoObj = MapData.CartoObjs[i];
                if (iCartoObj is POI poi) { // POI (Pushpin)
                    if (poi.IsPointClose(clickCoordonnees, _PRECISION)) {
                        newCoordonnee.Latitude = poi.Latitude;
                        newCoordonnee.Longitude = poi.Longitude;
                        newLocation.Latitude = poi.Latitude;
                        newLocation.Longitude = poi.Longitude;
                        stop = true;
                    }
                } else { // Polyline or Polygon (Coords of the element)
                    IPointy iPointy = iCartoObj as IPointy;
                    for (int j = iPointy.Coordonnees.Count() - 1; j >= 0 && !stop; j--) { // Reverse loop because the last element is the first showed on screen
                        Coordonnees currentCoord = iPointy.Coordonnees[j];
                        if (currentCoord.IsPointClose(clickCoordonnees, _PRECISION)) {
                            newCoordonnee.Latitude = currentCoord.Latitude;
                            newCoordonnee.Longitude = currentCoord.Longitude;
                            newLocation.Latitude = currentCoord.Latitude;
                            newLocation.Longitude = currentCoord.Longitude;
                            stop = true;
                        }
                    }
                }
            }
        }

        private void AddPushpin(Location locationToAdd)
        {
            Pushpin newPushpin = new Pushpin {
                Location = locationToAdd
            };
            POI newPOI = new POI(locationToAdd.Latitude, locationToAdd.Longitude);
            newPOI.Tag = newPushpin;
            MapData.Add(newPOI);
            MyMap.Children.Add(newPushpin);
        }

        #endregion

        #region Events

        private void MyMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (EditMode) {
                case 1: // Selection mode

                    break;
                case 2: // Add mode
                    switch (CbActionType.SelectedIndex) {
                        case 1: // Travel
                        case 2: // Surface
                            ResetCurrentItems();
                            break;
                    }
                    break;
                case 3: // Remove mode

                    break;
                case 4: // Update mode

                    break;
            }
        }
        private void MyMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDownPoisition = new Point(e.GetPosition(MyMap).X, e.GetPosition(MyMap).Y);
        }

        private void MyMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouseLeftButtonUpPosition = new Point(e.GetPosition(MyMap).X, e.GetPosition(MyMap).Y);
            if (mouseLeftButtonUpPosition == MouseLeftButtonDownPoisition) { // OnClick
                switch (EditMode) { // Selection mode
                    case 1:
                        bool elementSelected = false;
                        for (int i = MapData.CartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            ICartoObj iCartoObj = MapData.CartoObjs[i];
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = iCartoObj as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, _PRECISION)) {
                                elementSelected = true;
                                LbCartographyObjects.SelectedItem = iCartoObj;
                                LbCartographyObjects.ScrollIntoView(iCartoObj);
                                break;
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
                                AddPushpin(ClickLocation);
                                break;
                            case 1: // Travel
                                AddPointToTravel(ClickLocation);
                                break;
                            case 2: // Surface
                                AddPointToSurface(ClickLocation);
                                break;
                        }
                        UpdateLbCartographyObjectsItemsSource();
                        break;
                    case 3: // Remove mode
                        bool somethingToRemove = false;
                        ICartoObj toRemove = new POI(); // Default value 
                        for (int i = MapData.CartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            ICartoObj iCartoObj = MapData.CartoObjs[i];
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = iCartoObj as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, _PRECISION)) {
                                somethingToRemove = true;
                                toRemove = iCartoObj;
                                if (iCartoObj is POI poi) {
                                    Pushpin pushpinToRemove = (Pushpin)poi.Tag;
                                    MyMap.Children.Remove(pushpinToRemove);
                                } else if (iCartoObj is MyCartographyObjects.Polyline polyline) {
                                    MapPolyline mapPolylineToRemove = (MapPolyline)(polyline.Tag);
                                    MyMap.Children.Remove(mapPolylineToRemove);
                                } else if (iCartoObj is MyCartographyObjects.Polygon polygon) {
                                    MapPolygon mapPolygonToRemove = (MapPolygon)(polygon.Tag);
                                    MyMap.Children.Remove(mapPolygonToRemove);
                                }
                                UpdateLbCartographyObjectsItemsSource();
                                break;
                            }
                        }
                        if (somethingToRemove)
                            MapData.Remove(toRemove);
                        break;
                    case 4: // Update mode
                        break;
                }
            } else { // OnDrag
                // Nothing
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

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.IsKeyDown(Key.LeftCtrl)) { // CTRL + Enter
                if (IsDrawingTravel) {
                    MyCartographyObjects.Polyline newPolyline = new MyCartographyObjects.Polyline(CurrentTravel);
                    MapData.Add(newPolyline);
                    MapPolyline currentTravelMapPolyline = (MapPolyline)(CurrentTravel.Tag);
                    MapPolyline newMapPolyline = new MapPolyline() {
                        Stroke = currentTravelMapPolyline.Stroke,
                        StrokeThickness = currentTravelMapPolyline.StrokeThickness,
                        Opacity = currentTravelMapPolyline.Opacity,
                        Locations = new LocationCollection()
                    };
                    foreach (Location location in currentTravelMapPolyline.Locations) {
                        newMapPolyline.Locations.Add(location);
                    }
                    MyMap.Children.Add(newMapPolyline);
                    newPolyline.Tag = newMapPolyline;
                    ResetCurrentTravel();
                } else if (IsDrawingSurface) {
                    MyCartographyObjects.Polygon newPolygon = new MyCartographyObjects.Polygon(CurrentSurface);
                    MapData.Add(newPolygon);
                    MapPolygon currentSurfaceMapPolygon = (MapPolygon)(CurrentSurface.Tag);
                    MapPolygon newMapPolygon = new MapPolygon() {
                        Fill = currentSurfaceMapPolygon.Fill,
                        Stroke = currentSurfaceMapPolygon.Stroke,
                        StrokeThickness = currentSurfaceMapPolygon.StrokeThickness,
                        Opacity = currentSurfaceMapPolygon.Opacity,
                        Locations = new LocationCollection()
                    };
                    foreach (Location location in currentSurfaceMapPolygon.Locations) {
                        newMapPolygon.Locations.Add(location);
                    }
                    MyMap.Children.Add(newMapPolygon);
                    newPolygon.Tag = newMapPolygon;
                    ResetCurrentSurface();
                }
            }
        }
        private void MenuItem_File_POI_Import_Click(object sender, RoutedEventArgs e)
        {
            MapData.Import();
        }

        #endregion

    }
}
