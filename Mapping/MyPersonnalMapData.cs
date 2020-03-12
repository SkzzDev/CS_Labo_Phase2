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
using System.Xml.Serialization;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using MyCartographyObjects;

namespace Mapping
{
    public class MyPersonnalMapData
    {

        #region MemberVars

        private string _firstname = "";
        private string _lastname = "";
        private string _email = "";
        private ObservableCollection<ICartoObj> _cartoObjs = new ObservableCollection<ICartoObj>();
        private static readonly string CSVDir = @"C:\Compilations\C#\Labos\Phase2\csvs";
        private string _lastBinarySaved = "";

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

        public string LastBinarySaved
        {
            get { return _lastBinarySaved; }
            set { _lastBinarySaved = value; }
        }

        #endregion

        #region Constructors

        public MyPersonnalMapData(string firstname = "", string lastname = "")
        {
            Firstname = firstname;
            Lastname = lastname;

            // Default POIs present on the map
            Add(new POI(50.460554, 5.649703, "Maison"));
            Add(new POI(50.611265, 5.511353, "École"));
            Add(new POI(50.624466, 5.566776, "Liège Guillemin"));

            //LastBinarySaved = ToBinary();
        }

        public MyPersonnalMapData() : this("", "") { }

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

        public string ToBinary()
        {
            // Bug! Séréalisation d'Interfaces
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType(), new Type[] { typeof(Polygon), typeof(Polyline), typeof(POI) });
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        public void Export()
        {

        }

        public string GetFilenameToImport()
        {
            //bool force = false;
        //import:
            //if (ToBinary() == LastBinarySaved || force) {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Binary files (*.dat)|*.dat";
                if (openFileDialog.ShowDialog() == true) {
                    try {
                        if (Directory.Exists(CSVDir)) {
                            Console.WriteLine(openFileDialog.FileName);
                            FileInfo fileToImport = new FileInfo(openFileDialog.FileName);
                            if (fileToImport.Exists) {
                                return openFileDialog.FileName;
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
            //} else {
            //    if (MessageBox.Show("Les données actuelles ne sont pas sauvegardées. Voulez-vous continuer?", "Sauvegarde", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
            //        force = true;
            //        goto import;
            //    }
            //}
            return "";
        }

        #endregion

        public void SaveAsBinaryFormat(string filename) {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
                binFormat.Serialize(fStream, this);
            }
        }

        public void LoadFromBinaryFormat(string filename)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            using (Stream fStream = File.OpenRead(filename)) {
                MyPersonnalMapData myPersonnalMapData = (MyPersonnalMapData)binFormat.Deserialize(fStream);
                _cartoObjs = myPersonnalMapData.CartoObjs;
            }
        }

    }
}
