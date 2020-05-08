using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using Microsoft.Win32;
using MyCartographyObjects;

namespace MyCartographyObjects
{
    [Serializable]
    public class MyPersonnalMapData
    {

        #region MemberVars

        private string _firstname = "";
        private string _lastname = "";
        private string _email = "";
        private ObservableCollection<ICartoObj> _iCartoObjs = new ObservableCollection<ICartoObj>();
        public static string CSVS_DIR = "";
        public static string BINARIES_DIR = "";

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

        public ObservableCollection<ICartoObj> ICartoObjs
        {
            get { return _iCartoObjs;  }
        }

        #endregion

        #region Constructors

        public MyPersonnalMapData(string firstname = "", string lastname = "", string email = "")
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;

            string dir = Directory.GetCurrentDirectory();
            int pos = dir.LastIndexOf("\\Mapping\\bin");

            BINARIES_DIR = dir.Substring(0, pos) + "\\binaries";
            CSVS_DIR = dir.Substring(0, pos) + "\\CSVS";
        }

        public MyPersonnalMapData() : this("", "") { }

        #endregion

        #region Functions

        public string GetSessionFilename()
        {
            return Firstname + "_" + Lastname + ".dat";
        }

        public void Add(ICartoObj iCartoObj)
        {
            ICartoObjs.Add(iCartoObj);
        }

        public void Remove(ICartoObj iCartoObj)
        {
            ICartoObjs.Remove(iCartoObj);
        }

        public void Clear()
        {
            ICartoObjs.Clear();
        }

        public string ToBinary()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType(), new Type[] { typeof(Polygon), typeof(Polyline), typeof(POI) });
            using (StringWriter textWriter = new StringWriter()) {
                xmlSerializer.Serialize(textWriter, ICartoObjs);
                return textWriter.ToString();
            }
        }

        public void Export()
        {

        }

        public string GetFilenameToOpen(string ext = "dat")
        {
            string dir = (ext == "dat") ? BINARIES_DIR : CSVS_DIR;
            if (Directory.Exists(dir)) {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (ext == "dat")
                    openFileDialog.Filter = "Binary files (*.dat)|*.dat";
                else if (ext == "csv")
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                openFileDialog.InitialDirectory = dir;
                if (openFileDialog.ShowDialog() == true) {
                    return openFileDialog.FileName;
                } else {
                    Console.WriteLine("Erreur ouverture boite dialogue lors de l'importation de fichier");
                }
            } else {
                Console.WriteLine("Le répertoire « " + dir + " » n'existe pas");
            }
            return "";
        }

        #endregion

        public void SaveAsBinaryFormat(string filename)
        {
            if (File.Exists(filename)) {
                List<object> tags = new List<object>();
                foreach (ICartoObj iCartoObj in ICartoObjs) {
                    tags.Add(iCartoObj.Tag);
                    iCartoObj.Tag = null;
                }
                BinaryFormatter binFormat = new BinaryFormatter();
                using (Stream fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    binFormat.Serialize(fStream, ICartoObjs);
                }
                for (int i = 0; i < ICartoObjs.Count(); i++) {
                    ICartoObjs[i].Tag = tags[i];
                }
            }
        }

        public void LoadFromBinaryFormat(string filename)
        {
            if (File.Exists(filename)) {
                BinaryFormatter binFormat = new BinaryFormatter();
                ObservableCollection<ICartoObj> newCollection = new ObservableCollection<ICartoObj>();
                using (Stream fStream = File.OpenRead(filename)) {
                    if (fStream.Length != 0) {
                        newCollection = (ObservableCollection<ICartoObj>)binFormat.Deserialize(fStream);
                    }
                }
                Clear();
                foreach (ICartoObj iCartoObj in newCollection) {
                    Add(iCartoObj);
                }
            }
        }

        public bool LoadFromCsvFormat(string filename, string type, List<Coordonnees> coordList)
        {
            coordList.Clear();
            using (var reader = new StreamReader(filename)) {
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    string[] values = line.Split(';');
                    if (values.Length == 2) {
                        Coordonnees newCoord = new Coordonnees(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]));
                        coordList.Add(newCoord);
                    } else if (values.Length == 3) {
                        POI newPOI = new POI(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), values[2]);
                        coordList.Add(newPOI);
                    }
                }
            }
            if (coordList.Count() > 0) {
                if (type == "POI") {
                    return (coordList.Count() == 1);
                } else if (type == "Travel") {
                    return (coordList.Count() > 1);
                }
                return false;
            }
            return true;
        }

    }

}
