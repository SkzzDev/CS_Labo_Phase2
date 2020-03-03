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
        public MainWindow()
        {
            InitializeComponent();

            List<CartoObj> cartoObjs = new List<CartoObj>() {
                new POI(50.460554, 5.649703, "Maison"),
                new POI(50.611265, 5.511353, "École"),
                new POI(50.624466, 5.566776, "Liège Guillemin")
            };

            lbCartographyObjects.ItemsSource = cartoObjs;

            myMap.Mode = new AerialMode(true);

            myMap.MouseLeftButtonUp +=
                new MouseButtonEventHandler(MapWithEvents_MouseLeftButtonUp);
        }
        void MapWithEvents_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            // Updates the count of single mouse clicks.
            Location location;
            myMap.TryViewportPointToLocation(e.GetPosition(myMap), out location);
            myMap.SetView(location, myMap.ZoomLevel);
            
        }
    }
}
