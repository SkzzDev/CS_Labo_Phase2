using Microsoft.Maps.MapControl.WPF;
using MyCartographyObjects;
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
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>

    public delegate void UpdateMapAndListBox();

    public partial class UpdateWindow : Window
    {

        #region Properties

        public UpdateMapAndListBox send;

        public ICartoObj SaveReceivedObj { get; set; }
        public ICartoObj ReceivedObj { get; set; }

        #endregion

        #region Constructors

        public UpdateWindow(ICartoObj receivedObj)
        {
            InitializeComponent();

            TblkType.Text = receivedObj.GetType().Name + " #" + ((CartoObj)receivedObj).Id;
            TbDescription.Text = receivedObj.Description;

            switch (receivedObj.GetType().Name) {
                case "POI":
                    POI receivedPOI = (POI)receivedObj;
                    SaveReceivedObj = new POI(receivedPOI);
                    SaveReceivedObj.Tag = receivedPOI.Tag;
                    ReceivedObj = receivedPOI;

                    CpStroke.IsEnabled = false;

                    TbLatitude.Text = receivedPOI.Latitude.ToString();
                    TbLongitude.Text = receivedPOI.Longitude.ToString();
                    CpStroke.Background = Brushes.Gray;
                    CpFill.SelectedColor = receivedPOI.Fill;
                    break;
                case "Polyline":
                    MyCartographyObjects.Polyline receivedPolyline = (MyCartographyObjects.Polyline)receivedObj;
                    SaveReceivedObj = new MyCartographyObjects.Polyline(receivedPolyline);
                    SaveReceivedObj.Tag = receivedPolyline.Tag;
                    ReceivedObj = receivedPolyline;

                    CpFill.IsEnabled = false;
                    TbLatitude.IsEnabled = false;
                    CpFill.IsEnabled = false;

                    TbLatitude.Background = Brushes.Gray;
                    TbLongitude.Background = Brushes.Gray;
                    CpFill.Background = Brushes.Gray;
                    CpStroke.SelectedColor = receivedPolyline.Stroke;
                    break;
                case "Polygon":
                    MyCartographyObjects.Polygon receivedPolygon = (MyCartographyObjects.Polygon)receivedObj;
                    SaveReceivedObj = new MyCartographyObjects.Polygon(receivedPolygon);
                    SaveReceivedObj.Tag = receivedPolygon.Tag;
                    ReceivedObj = receivedPolygon;

                    TbLatitude.IsEnabled = false;
                    TbLongitude.IsEnabled = false;

                    TbLatitude.Background = Brushes.Gray;
                    TbLongitude.Background = Brushes.Gray;
                    CpFill.SelectedColor = receivedPolygon.Fill;
                    CpStroke.SelectedColor = receivedPolygon.Stroke;
                    break;
            }
        }

        #endregion

        public bool IsDouble(string text)
        {
            return string.IsNullOrEmpty(text) ? false : double.TryParse(text, out _);
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            if (TbDescription.Text == "" || (ReceivedObj is POI && (TbLatitude.Text == "" || TbLatitude.Text == ""))) {
                MessageBox.Show("Les champs non grisés doivent être remplis.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                ReceivedObj.Description = TbDescription.Text;
                if (ReceivedObj is POI receivedPOI) {
                    string latitude = TbLatitude.Text.Replace(".", ","), longitude = TbLongitude.Text.Replace(".", ",");
                    if (IsDouble(latitude) && IsDouble(longitude)) {
                        receivedPOI.Latitude = Convert.ToDouble(latitude);
                        receivedPOI.Longitude = Convert.ToDouble(longitude);
                        receivedPOI.Fill = (Color)CpFill.SelectedColor;

                        Pushpin pushpin = (Pushpin)receivedPOI.Tag;
                        pushpin.Location.Latitude = receivedPOI.Latitude;
                        pushpin.Location.Longitude = receivedPOI.Longitude;
                        pushpin.Background = new SolidColorBrush(receivedPOI.Fill);
                    } else {
                        MessageBox.Show("Les champs latitude et longitude ne sont pas des entiers.", "Attention!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                } else if (ReceivedObj is MyCartographyObjects.Polyline receivedPolyline) {
                    receivedPolyline.Stroke = (Color)CpStroke.SelectedColor;

                    MapPolyline mapPolyline = (MapPolyline)receivedPolyline.Tag;
                    mapPolyline.Stroke = new SolidColorBrush(receivedPolyline.Stroke);
                } else if (ReceivedObj is MyCartographyObjects.Polygon receivedPolygon) {
                    receivedPolygon.Fill = (Color)CpFill.SelectedColor;
                    receivedPolygon.Stroke = (Color)CpStroke.SelectedColor;

                    MapPolygon mapPolygon = (MapPolygon)receivedPolygon.Tag;
                    mapPolygon.Fill = new SolidColorBrush(receivedPolygon.Fill);
                    mapPolygon.Stroke = new SolidColorBrush(receivedPolygon.Stroke);
                }
            }
            send?.Invoke();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            BtnTest_Click(sender, e);
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReceivedObj.Description = SaveReceivedObj.Description;
            if (ReceivedObj is POI receivedPOI) {
                POI savedPOI = SaveReceivedObj as POI;
                receivedPOI.Latitude = savedPOI.Latitude;
                receivedPOI.Longitude = savedPOI.Longitude;
                receivedPOI.Fill = savedPOI.Fill;

                Pushpin receivedPushpin = (Pushpin)receivedPOI.Tag;
                receivedPushpin.Location.Latitude = savedPOI.Latitude;
                receivedPushpin.Location.Longitude = savedPOI.Longitude;
                receivedPushpin.Background = new SolidColorBrush(savedPOI.Fill);
            } else if (ReceivedObj is MyCartographyObjects.Polyline receivedPolyline) {
                MyCartographyObjects.Polyline savedPolyline = SaveReceivedObj as MyCartographyObjects.Polyline;
                receivedPolyline.Stroke = savedPolyline.Stroke;

                MapPolyline mapPolyline = (MapPolyline)receivedPolyline.Tag;
                mapPolyline.Stroke = new SolidColorBrush(savedPolyline.Stroke);
            } else if (ReceivedObj is MyCartographyObjects.Polygon receivedPolygon) {
                MyCartographyObjects.Polygon savedPolygon = SaveReceivedObj as MyCartographyObjects.Polygon;
                receivedPolygon.Fill = savedPolygon.Fill;
                receivedPolygon.Stroke = savedPolygon.Stroke;

                MapPolygon mapPolygon = (MapPolygon)receivedPolygon.Tag;
                mapPolygon.Fill = new SolidColorBrush(savedPolygon.Fill);
                mapPolygon.Stroke = new SolidColorBrush(savedPolygon.Stroke);
            }
            Close();
        }
    }
}
