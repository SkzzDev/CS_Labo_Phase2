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
        private static readonly string CSVDir = @"C:\Compilations\C#\Labos\Phase2\csvs";
        private string _binariesDir = @"C:\Compilations\C#\Labos\Phase2\binaries";

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

        public string BinariesDir
        {
            get { return _binariesDir; }
            set { _binariesDir = value; }
        }

        #endregion

        #region Constructors

        public MyPersonnalMapData(string firstname = "", string lastname = "", string email = "")
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
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

        public string GetFilenameToOpen()
        {
            if (Directory.Exists(BinariesDir)) {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Binary files (*.dat)|*.dat";
                openFileDialog.InitialDirectory = BinariesDir;
                if (openFileDialog.ShowDialog() == true) {
                    return openFileDialog.FileName;
                } else {
                    Console.WriteLine("Erreur ouverture boite dialogue lors de l'importation de fichier");
                }
            } else {
                Console.WriteLine(@"Le répertoire « {0} » n'existe pas", BinariesDir);
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
                using (Stream fStream = File.OpenRead(filename)) {
                    ObservableCollection<ICartoObj> newCollection = new ObservableCollection<ICartoObj>();
                    if (fStream.Length != 0) {
                        newCollection = (ObservableCollection<ICartoObj>)binFormat.Deserialize(fStream);
                    }
                    Clear();
                    foreach (ICartoObj iCartoObj in newCollection) {
                        Add(iCartoObj);
                    }
                }
            }
        }

    }
}
