using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyCartographyObjects;

namespace Mapping
{
    public class MyPersonnalMapData
    {

        #region MemberVars

        private string _firstname;
        private string _lastname;
        private ObservableCollection<ICartoObj> _cartoObjs = new ObservableCollection<ICartoObj>();

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

        public ObservableCollection<ICartoObj> CartoObjs
        {
            get { return _cartoObjs;  }
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
        }

        #endregion

        #region Functions

        public void Add(ICartoObj cartoObj)
        {
            CartoObjs.Add(cartoObj);
        }

        public void Remove(ICartoObj cartoObj)
        {
            CartoObjs.Remove(cartoObj);
        }

        public void Export()
        {

        }

        public void Import(FileInfo fileToImport)
        {

        }

        #endregion

    }
}
