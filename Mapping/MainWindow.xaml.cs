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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string firstname = TbFirstname.Text, lastname = TbLastname.Text, email = TbEmail.Text;
            if (firstname != "" && lastname != "" && email != "") {
                if (email.Length >= 5 && email.Contains("@") && email.Contains(".")) {
                    MyPersonnalMapData mapData = new MyPersonnalMapData(firstname, lastname, email);

                    MappingWindow mappingWindow = new MappingWindow(mapData);
                    mappingWindow.Show();

                    Close();
                } else {
                    MessageBox.Show("Veillez indiquer une adresse e-mail valide.", "Erreur!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } else {
                MessageBox.Show("Veillez compléter tous les champs", "Erreur!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
