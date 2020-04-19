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
using Microsoft.Win32;
using MyCartographyObjects;

namespace Mapping
{
    /// <summary>
    /// Interaction logic for MappingWindow.xaml
    /// </summary>

    public enum EditModes { Select, Add, Remove, Update }

    public partial class MappingWindow : Window
    {

        #region MemberVars

        public static double _PRECISION = 0.0005;

        #endregion

        #region Properties

        public EditModes EditMode { get; set; } = EditModes.Select;

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

        public MappingWindow(MyPersonnalMapData mapData)
        {
            InitializeComponent();

            MapData = mapData;

            CurrentTravel = new MyCartographyObjects.Polyline();
            CurrentTravel.Tag = new MapPolyline() {
                Stroke = new SolidColorBrush(CurrentTravel.Stroke),
                StrokeThickness = CurrentTravel.Thickness,
                Opacity = CurrentTravel.Opacity,
                Locations = new LocationCollection()
            };
            CurrentSurface = new MyCartographyObjects.Polygon();
            CurrentSurface.Tag = new MapPolygon() {
                Fill = new SolidColorBrush(CurrentSurface.Fill),
                Stroke = new SolidColorBrush(CurrentSurface.Stroke),
                StrokeThickness = CurrentSurface.Thickness,
                Opacity = CurrentSurface.Opacity,
                Locations = new LocationCollection()
            };

            string sessionFileFullPath = MyPersonnalMapData.BINARIES_DIR + "\\" + MapData.GetSessionFilename();
            if (File.Exists(sessionFileFullPath)) { // If the user has a session file
                LoadBinaryFile(MyPersonnalMapData.BINARIES_DIR + "\\" + MapData.GetSessionFilename());
            } else {
                File.WriteAllText(sessionFileFullPath, ""); // If not, we create a new file without any data
            }

            UpdateLbCartographyObjectsItemsSource();
        }

        #region Functions

        private void DrawMapDataElements() // Draw defaults elements, maybe useless later
        {
            foreach (ICartoObj iCartoObj in MapData.ICartoObjs) {
                if (iCartoObj is POI poi) {
                    Pushpin newPushpin = new Pushpin {
                        Location = new Location(poi.Latitude, poi.Longitude),
                        Background = Brushes.Red
                    };
                    iCartoObj.Tag = newPushpin;
                    MyMap.Children.Add(newPushpin);
                }
            }
        }

        private void UpdateLbCartographyObjectsItemsSource()
        {
            LbCartographyObjects.ItemsSource = null;
            LbCartographyObjects.ItemsSource = MapData.ICartoObjs;
        }

        private void ClickOnToolbarButton(EditModes newEditMode)
        {
            if (newEditMode != EditModes.Select)
                LbCartographyObjects.SelectedItem = null;
            if (newEditMode != EditMode) {
                // Remove old selected button
                Button clickedButton = new Button();
                switch (EditMode) {
                    case EditModes.Select:
                        clickedButton = BtnSelect;
                        break;
                    case EditModes.Add:
                        clickedButton = BtnAdd;
                        break;
                    case EditModes.Remove:
                        clickedButton = BtnRemove;
                        break;
                    case EditModes.Update:
                        clickedButton = BtnUpdate;
                        break;
                }
                clickedButton.Background = Brushes.LightGray;
                clickedButton.Foreground = Brushes.Black;

                // Set new selected button
                EditMode = newEditMode;
                if (newEditMode != EditModes.Add) {
                    CbType.IsHitTestVisible = false;
                    CbType.Focusable = false;
                } else {
                    CbType.IsHitTestVisible = true;
                    CbType.Focusable = true;
                }
                switch (newEditMode) {
                    case EditModes.Select:
                        BtnSelect.Background = Brushes.DodgerBlue;
                        BtnSelect.Foreground = Brushes.White;
                        break;
                    case EditModes.Add:
                        BtnAdd.Background = Brushes.Green;
                        BtnAdd.Foreground = Brushes.White;
                        break;
                    case EditModes.Remove:
                        BtnRemove.Background = Brushes.DarkRed;
                        BtnRemove.Foreground = Brushes.White;
                        break;
                    case EditModes.Update:
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

        private void AddPushpin(Location locationToAdd)
        {
            Pushpin newPushpin = new Pushpin {
                Location = locationToAdd,
                Background = Brushes.Red
            };
            POI newPOI = new POI(locationToAdd.Latitude, locationToAdd.Longitude);
            newPOI.Tag = newPushpin;
            MapData.Add(newPOI);
            MyMap.Children.Add(newPushpin);
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
            for (int i = MapData.ICartoObjs.Count() - 1; i >= 0 && !stop; i--) { // Reverse loop because the last element is the first showed on screen
                ICartoObj iCartoObj = MapData.ICartoObjs[i];
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

        private void LoadBinaryFile(string filename)
        {
            if (filename != "") {
                if (filename != MapData.GetSessionFilename()) { // Load objects into the instance
                    MapData.LoadFromBinaryFormat(filename);
                    MyMap.Children.Clear(); // Clear the map
                    foreach (ICartoObj iCartoObj in MapData.ICartoObjs) { // Add the new objects on the map
                        UIElement uiElement = new UIElement();
                        Console.WriteLine(iCartoObj);
                        if (iCartoObj is MyCartographyObjects.POI poi) {
                            Location location = new Location(poi.Latitude, poi.Longitude);
                            uiElement = new Pushpin {
                                Location = location,
                                Background = new SolidColorBrush(poi.Fill)
                            };
                        } else if (iCartoObj is MyCartographyObjects.Polyline polyline) {
                            LocationCollection locations = new LocationCollection();
                            foreach (Coordonnees coordonnees in polyline.Coordonnees) {
                                locations.Add(new Location(coordonnees.Latitude, coordonnees.Longitude));
                            }
                            uiElement = new MapPolyline() {
                                Stroke = new SolidColorBrush(polyline.Stroke),
                                StrokeThickness = polyline.Thickness,
                                Opacity = polyline.Opacity,
                                Locations = locations
                            };
                        } else if (iCartoObj is MyCartographyObjects.Polygon polygon) {
                            LocationCollection locations = new LocationCollection();
                            foreach (Coordonnees coordonnees in polygon.Coordonnees) {
                                locations.Add(new Location(coordonnees.Latitude, coordonnees.Longitude));
                            }
                            uiElement = new MapPolygon() {
                                Fill = new SolidColorBrush(polygon.Fill),
                                Stroke = new SolidColorBrush(polygon.Stroke),
                                StrokeThickness = polygon.Thickness,
                                Opacity = polygon.Opacity,
                                Locations = locations
                            };
                        }
                        iCartoObj.Tag = uiElement;
                        MyMap.Children.Add(uiElement);
                    }
                    UpdateLbCartographyObjectsItemsSource();
                } else {
                    MessageBox.Show("Ce fichier de données ne vous appartient pas.", "Erreur!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } else {
                // Cancelled or failled at opening
            }
        }

        public bool IsDouble(string text)
        {
            return string.IsNullOrEmpty(text) ? false : double.TryParse(text, out _);
        }

        #endregion

        #region Events

        private void MyMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (EditMode) {
                case EditModes.Select: // Selection mode

                    break;
                case EditModes.Add: // Add mode
                    switch (CbType.SelectedIndex) {
                        case 1: // Travel
                        case 2: // Surface
                            ResetCurrentItems();
                            break;
                    }
                    break;
                case EditModes.Remove: // Remove mode

                    break;
                case EditModes.Update: // Update mode

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
                switch (EditMode) {
                    case EditModes.Select:
                        bool elementSelected = false;
                        for (int i = MapData.ICartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = MapData.ICartoObjs[i] as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, _PRECISION)) {
                                elementSelected = true;
                                LbCartographyObjects.SelectedItem = MapData.ICartoObjs[i];
                                LbCartographyObjects.ScrollIntoView(MapData.ICartoObjs[i]);
                                break;
                            }
                        }
                        if (!elementSelected) {
                            LbCartographyObjects.SelectedItem = null;
                        }
                        break;
                    case EditModes.Add:
                        Location ClickLocation;
                        MyMap.TryViewportPointToLocation(e.GetPosition(MyMap), out ClickLocation);
                        switch (CbType.SelectedIndex) {
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
                    case EditModes.Remove:
                        bool somethingToRemove = false;
                        ICartoObj toRemove = new POI(); // Default value 
                        for (int i = MapData.ICartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            ICartoObj iCartoObj = MapData.ICartoObjs[i];
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = iCartoObj as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, _PRECISION)) {
                                somethingToRemove = true;
                                toRemove = iCartoObj;
                                UIElement uiElement = (UIElement)(iCartoObj.Tag);
                                MyMap.Children.Remove(uiElement);
                                UpdateLbCartographyObjectsItemsSource();
                                break;
                            }
                        }
                        if (somethingToRemove)
                            MapData.Remove(toRemove);
                        break;
                    case EditModes.Update:
                        bool somethingToUpdate = false;
                        ICartoObj toUpdate = new POI(); // Default value 
                        for (int i = MapData.ICartoObjs.Count() - 1; i >= 0; i--) { // Reverse loop because the last element is the first showed on screen
                            ICartoObj iCartoObj = MapData.ICartoObjs[i];
                            Point clickPoint = e.GetPosition(MyMap);
                            MyMap.TryViewportPointToLocation(clickPoint, out Location clickLocation);
                            Coordonnees clickCoordonnees = new Coordonnees(clickLocation.Latitude, clickLocation.Longitude);
                            CartoObj cartoObj = iCartoObj as CartoObj;
                            if (cartoObj.IsPointClose(clickCoordonnees, _PRECISION)) {
                                somethingToUpdate = true;
                                toUpdate = iCartoObj;
                                break;
                            }
                        }
                        if (somethingToUpdate) {
                            UpdateWindow updateWindow = new UpdateWindow(toUpdate);
                            updateWindow.send = UpdateMapAndListBox;
                            updateWindow.Show();
                        }

                        // TO DO: CREATE AN EVENT THAT UPDATE BOTH SIDE ON TEST, OK AND CANCEL BUTTON!!
                        MyMap.UpdateLayout();
                        UpdateLbCartographyObjectsItemsSource();
                        break;
                }
            } else { // OnDrag
                // Nothing
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(EditModes.Select);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(EditModes.Add);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(EditModes.Remove);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ClickOnToolbarButton(EditModes.Update);
        }

        private void BtnAddPOI_Click(object sender, RoutedEventArgs e)
        {
            if (TbLatitude.Text != "" && TbLongitude.Text != "") {
                string latitude = TbLatitude.Text.Replace(".", ","), longitude = TbLongitude.Text.Replace(".", ",");
                if (IsDouble(latitude) && IsDouble(longitude)) {
                    Location location = new Location(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                    AddPushpin(location);
                } else {
                    MessageBox.Show("Les deux champs ne sont pas des entiers.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } else {
                MessageBox.Show("Les deux champs doivent être remplis.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LbCartographyObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbCartographyObjects.SelectedItem is POI poi) {
                Location location = new Location(poi.Latitude, poi.Longitude);
                MyMap.SetView(location, MyMap.ZoomLevel);
            } else if (LbCartographyObjects.SelectedItem is MyCartographyObjects.Polyline polyline) {
                Coordonnees centerPolyline = polyline.GetCenter();
                Location location = new Location(centerPolyline.Latitude, centerPolyline.Longitude);
                MyMap.SetView(location, MyMap.ZoomLevel);
            } else if (LbCartographyObjects.SelectedItem is MyCartographyObjects.Polygon polygon) {
                Coordonnees centerPolygon = polygon.GetCenter();
                Location location = new Location(centerPolygon.Latitude, centerPolygon.Longitude);
                MyMap.SetView(location, MyMap.ZoomLevel);
            }
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

        private void MenuItem_File_Open_Click(object sender, RoutedEventArgs e)
        {
            string filename = MapData.GetFilenameToOpen();
            if (File.Exists(filename)) {
                LoadBinaryFile(filename);
            }
        }

        private void MenuItem_File_Save_Click(object sender, RoutedEventArgs e)
        {
            string sessionFileFullPath = MyPersonnalMapData.BINARIES_DIR + "\\" + MapData.GetSessionFilename();
            File.WriteAllText(sessionFileFullPath, "");
            MapData.SaveAsBinaryFormat(sessionFileFullPath);
        }

        private void MenuItem_File_POI_Import_Click(object sender, RoutedEventArgs e)
        {
            string filename = MapData.GetFilenameToOpen("csv");
            if (File.Exists(filename)) {
                List<Coordonnees> coordList = new List<Coordonnees>();
                if (MapData.LoadFromCsvFormat(filename, "POI", coordList)) {
                    POI poiToAdd = new POI((POI)coordList[0]);
                    Location locationNewPOI = new Location(poiToAdd.Latitude, poiToAdd.Longitude);
                    Pushpin newPushpin = new Pushpin {
                        Location = locationNewPOI,
                        Background = Brushes.Red
                    };
                    poiToAdd.Tag = newPushpin;
                    MapData.Add(poiToAdd);
                    MyMap.Children.Add(newPushpin);
                } else {
                    MessageBox.Show("Ce CSV n'est pas un POI !", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void MenuItem_File_POI_Export_Click(object sender, RoutedEventArgs e)
        {
            if (LbCartographyObjects.SelectedIndex != -1) {
                if (LbCartographyObjects.SelectedItem is POI poi) {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                    saveFileDialog.Title = "Sauvegarder un nouveau POI";
                    saveFileDialog.ShowDialog();
                    if (saveFileDialog.FileName != "") {
                        File.WriteAllText(saveFileDialog.FileName, poi.Latitude.ToString() + ";" + poi.Longitude.ToString() + ";" + poi.Description);
                    }
                } else {
                    MessageBox.Show("L'élément sélectionné n'est pas un POI.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void MenuItem_File_Travel_Import_Click(object sender, RoutedEventArgs e)
        {
            string filename = MapData.GetFilenameToOpen("csv");
            if (File.Exists(filename)) {
                List<Coordonnees> coordList = new List<Coordonnees>();
                if (MapData.LoadFromCsvFormat(filename, "Travel", coordList)) {
                    MyCartographyObjects.Polyline newPolyline = new MyCartographyObjects.Polyline();
                    LocationCollection locations = new LocationCollection();
                    foreach (Coordonnees coord in coordList) {
                        newPolyline.Add(coord);
                        Location location = new Location(coord.Latitude, coord.Longitude);
                        locations.Add(location);
                    }
                    MapData.Add(newPolyline);
                    MapPolyline newMapPolyline = new MapPolyline() {
                        Stroke = new SolidColorBrush(newPolyline.Stroke),
                        StrokeThickness = newPolyline.Thickness,
                        Opacity = newPolyline.Opacity,
                        Locations = locations
                    };
                    newPolyline.Tag = newMapPolyline;
                    MyMap.Children.Add(newMapPolyline);
                } else {
                    MessageBox.Show("L'élément sélectionné n'est pas un trajet.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void MenuItem_File_Travel_Export_Click(object sender, RoutedEventArgs e)
        {
            if (LbCartographyObjects.SelectedIndex != -1) {
                if (LbCartographyObjects.SelectedItem is MyCartographyObjects.Polyline polyline) {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                    saveFileDialog.Title = "Sauvegarder un nouveau trajet";
                    saveFileDialog.ShowDialog();
                    if (saveFileDialog.FileName != "") {
                        string toWrite = "";
                        foreach (Coordonnees coord in polyline.Coordonnees) {
                            toWrite += coord.Latitude.ToString() + ";" + coord.Longitude.ToString();
                            if (coord is POI poi) {
                                toWrite += ";" + poi.Description;
                            }
                            toWrite += Environment.NewLine;
                        }
                        File.WriteAllText(saveFileDialog.FileName, toWrite);
                    }
                } else {
                    MessageBox.Show("L'élément sélectionné n'est pas un trajet.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void MenuItem_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItem_Tools_About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void MenuItem_Tools_Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow(LbCartographyObjects.Background, LbCartographyObjects.Foreground, MyPersonnalMapData.BINARIES_DIR);
            optionsWindow.send = EventUpdateOptions;
            optionsWindow.Show();
        }

        private void EventUpdateOptions(object sender, OptionsEventArgs e)
        {
            LbCartographyObjects.Background = new SolidColorBrush(e.LbBackground);
            LbCartographyObjects.Foreground = new SolidColorBrush(e.LbForeground);
            MyPersonnalMapData.BINARIES_DIR = e.Path;
        }

        private void UpdateMapAndListBox()
        {
            MyMap.UpdateLayout();
            UpdateLbCartographyObjectsItemsSource();
        }

        #endregion

    }
}
