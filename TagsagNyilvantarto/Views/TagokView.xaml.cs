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

namespace TagsagNyilvantarto.Views
{
    /// <summary>
    /// Interaction logic for Tagok.xaml
    /// </summary>
    public partial class TagokView : UserControl
    {
        public TagokView()
        {
            InitializeComponent();
        }

        private void Tagok_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Tagok_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Ötlet: Adatmodelbe egy kiválasztva bool mező felvétele, majd oszlop értékének hozzákötése
            //for (int i = 0; i < Tagok.Items.Count; i++)
            //{
            //    DataGridRow row = (DataGridRow)Tagok.ItemContainerGenerator.ContainerFromIndex(i);
            //}
            //System.Data.DataRowCollection dataRowCollection= ((System.Data.DataView)Tagok.ItemsSource).ToTable().Rows;
            //var a=dataRowCollection[0].ItemArray[0];
        }
    }
}
