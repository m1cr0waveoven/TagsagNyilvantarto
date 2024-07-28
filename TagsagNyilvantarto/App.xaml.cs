using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TagsagNyilvantarto.ViewModels;

namespace TagsagNyilvantarto
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        object _dgDataContext;
        object _dataAccess;
        PropertyInfo[] _propertyInfos;
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(capacity: 9);
        public App()
        {
            //Oszlop fejlécnevek és property nevek összerendelése
            keyValuePairs.Add("Id", "Id");
            keyValuePairs.Add("Email", "Email");
            keyValuePairs.Add("Név", "Nev");
            keyValuePairs.Add("Telefon", "Telefon");
            keyValuePairs.Add("Tisztség", "Tisztseg");
            keyValuePairs.Add("Tagság jogállása", "Jogallas");
            keyValuePairs.Add("AdatokTípusa", "AdatokTipusa");
            keyValuePairs.Add("Képviselő", "Kepviselo");
            keyValuePairs.Add("Admin", "Admin");
            keyValuePairs.Add("Születés", "Szuletes");
            keyValuePairs.Add("Tagság kezdete", "TagsagKezdete");
        }
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.DataGrid datagrid)
            {
                _dgDataContext = datagrid.DataContext;
            }
            else if (sender is System.Windows.Controls.ComboBox combobox)
            {
                combobox.DataContext = _dgDataContext;
            }
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox combobox)
            {
                if (combobox.Tag == null)
                    return;

                string[] arr = combobox.Tag.ToString().Split('.');

                if (_propertyInfos == null)
                {
                    //var a = _dgDataContext.GetType().GetProperties();
                    _dataAccess = _dgDataContext.GetType().GetProperty(arr[0]).GetValue(_dgDataContext, null);
                    _propertyInfos = _dataAccess.GetType().GetProperties();

                }

                IEnumerable<string> itemsource = (IEnumerable<string>)_propertyInfos.Single(prop => prop.Name == arr[1]).GetValue(_dataAccess, null);
                combobox.ItemsSource = itemsource;
            }
        }

        private void FilterComboBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                ComboBox_SelectionChanged(sender, null);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            TagokViewModel tagokViewModel = (TagokViewModel)comboBox.DataContext;
            string columnHeaderText = ((TextBlock)((StackPanel)comboBox.Parent).Children[0]).Text;
            keyValuePairs.TryGetValue(columnHeaderText, out string propname);
            if(propname!=null)
            {
                PropertyInfo[] filterpops = tagokViewModel.SelectedFilterValues.GetType().GetProperties();
                filterpops.Single(p => p.Name == propname).SetValue(tagokViewModel.SelectedFilterValues, comboBox.SelectedValue);
                tagokViewModel.Filter(tagokViewModel.Tagok);
                if (e != null)
                    e.Handled = true;
            }
           
        }
    }
}
