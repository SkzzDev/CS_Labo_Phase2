using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using MyCartographyObjects;

namespace Mapping
{
    public class MyPersonnalMapData
    {

        #region MemberVars

        private string _firstname;
        private string _lastname;
        private string _email;
        private ObservableCollection<ICartoObj> _cartoObjs = new ObservableCollection<ICartoObj>();
        private static readonly string CSVDir = @"C:\Compilations\C#\Labos\Phase2\csvs";
        private string _lastCSVSaved = "";

        #endregion

        #region Properties

        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value.ToLower(); }
        }

        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value.ToLower(); }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value.ToLower(); }
        }

        public ObservableCollection<ICartoObj> CartoObjs
        {
            get { return _cartoObjs;  }
        }

        public string LastCSVSaved
        {
            get { return _lastCSVSaved; }
            set { _lastCSVSaved = value; }
        }

        public Map MyMap
        {
            get; set;
        }

        #endregion

        #region Constructors

        public MyPersonnalMapData(Map map, string firstname = "", string lastname = "")
        {
            MyMap = map;
            Firstname = firstname;
            Lastname = lastname;

            // Default POIs present on the map
            Add(new POI(50.460554, 5.649703, "Maison"));
            Add(new POI(50.611265, 5.511353, "École"));
            Add(new POI(50.624466, 5.566776, "Liège Guillemin"));

            LastCSVSaved = ToCSV();
        }

        #endregion

        #region Functions

        public void Add(ICartoObj iCartoObj)
        {
            CartoObjs.Add(iCartoObj);
        }

        public void Remove(ICartoObj iCartoObj)
        {
            CartoObjs.Remove(iCartoObj);
        }

        public string ToCSV()
        {
            string csv = "";

            foreach (CartoObj cartoObj in CartoObjs) {
                csv += cartoObj.ToCSV() + "\r\n";
            }

            return csv;
        }

        public void Export()
        {

        }

        public void Import()
        {
            if (ToCSV() == LastCSVSaved) {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true) {
                    try {
                        // LoadFromBinaryFormat(openFileDialog.FileName);
                        if (Directory.Exists(CSVDir)) {
                            Console.WriteLine(openFileDialog.FileName);
                            FileInfo fileToImport = new FileInfo(openFileDialog.FileName);
                            if (fileToImport.Exists) {
                                Console.WriteLine("Le fichier « {0} » existe", openFileDialog.FileName);
                                LoadCSV(openFileDialog.FileName);
                            } else {
                                Console.WriteLine("Le fichier « {0} » n'existe pas", openFileDialog.FileName);
                            }
                        } else {
                            Console.WriteLine(@"Le répertoire « {0} » n'existe pas", CSVDir);
                        }
                    } catch (Exception e) {
                        Console.WriteLine("Erreur : {0}", e.Message);
                    }
                } else {
                    Console.WriteLine("Erreur ouverture boite dialogue lors de l'importation de fichier");
                }
            } else {
                // Message box Oui/Non/Annuler
                Console.WriteLine("Les donnees actuelles ne sont pas sauvegardees, voulez-vous continuer?");
            }
        }

        #endregion

        private static void SaveAsBinaryFormat(POI pio, string filename) {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
                binFormat.Serialize(fStream, pio);
            }
        }

        private static void LoadFromBinaryFormat(string filename)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = File.OpenRead(filename)) {
                POI carFromDisk = (POI)binFormat.Deserialize(fStream);
                Console.WriteLine(carFromDisk);
            }
        }

        private void LoadCSV(string fullFilePath)
        {
            byte[] bytes = new byte[0];

            // Read bytes in CSV file
            using (FileStream fsSource = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read)) {

                // Read the source file into a byte array.
                bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0) {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0) break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }
            
            // Clear map and object list
            foreach (ICartoObj iCartoObj in CartoObjs) {
                MyMap.Children.Remove((UIElement)iCartoObj.Tag);
            }
            CartoObjs.Clear();

            // Convert all bytes into one string
            string csv = "";
            for (int i = 0; i < bytes.Length; i++) {
                csv += Convert.ToChar(bytes[i]);
            }
            LastCSVSaved = csv;

            int lines = csv.Count(f => f == '\n');

            // Load new elements from the string into the object list and on the map
            if (lines == 0 || (lines == 1 && csv[csv.Length - 1] == 10 && csv[csv.Length - 2] == 13)) { // POI (Pushpin)
                if (lines == 1)
                    csv = csv.Remove(csv.Length - 2);
                POI newPOI = new POI(csv);
                Location pushpinLocation = new Location(newPOI.Latitude, newPOI.Longitude);
                Pushpin newPushpin = new Pushpin {
                    Location = pushpinLocation,
                    Background = new SolidColorBrush(newPOI.BackgroundColor),
                };
                newPOI.Tag = newPushpin;
                Add(newPOI);
                MyMap.Children.Add(newPushpin);
            } else { // Polyline (Travel)
                /*
                int commaPos = 0;
                while ((commaPos = csv.IndexOf(';')) != -1) {
                    string currentCoordCSV = csv.Remove(commaPos);
                    int commaCount = currentCoordCSV.Count(f => f == ';');

                }*/
            }

        }

    }
}
