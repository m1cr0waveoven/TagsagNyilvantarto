using System.Windows;
using System.Windows.Controls;

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
            // For the future
        }

        private void Tagok_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // For the future
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
