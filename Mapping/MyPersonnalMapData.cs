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
