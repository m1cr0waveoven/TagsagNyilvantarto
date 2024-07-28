using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TagsagNyilvantarto.ViewModels;

namespace TagsagNyilvantarto;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private object _dgDataContext;
    private object _dataAccess;
    private PropertyInfo[] _propertyInfos;
    private static readonly Dictionary<string, string> ColumnHeaderToProperyName = new Dictionary<string, string>(capacity: 11);
    public App()
    {
        //Oszlop fejlécnevek és property nevek összerendelése
        ColumnHeaderToProperyName.Add("Id", "Id");
        ColumnHeaderToProperyName.Add("Email", "Email");
        ColumnHeaderToProperyName.Add("Név", "Nev");
        ColumnHeaderToProperyName.Add("Telefon", "Telefon");
        ColumnHeaderToProperyName.Add("Tisztség", "Tisztseg");
        ColumnHeaderToProperyName.Add("Tagság jogállása", "Jogallas");
        ColumnHeaderToProperyName.Add("AdatokTípusa", "AdatokTipusa");
        ColumnHeaderToProperyName.Add("Képviselő", "Kepviselo");
        ColumnHeaderToProperyName.Add("Admin", "Admin");
        ColumnHeaderToProperyName.Add("Születés", "Szuletes");
        ColumnHeaderToProperyName.Add("Tagság kezdete", "TagsagKezdete");
    }
    private void ComboBox_Loaded(object sender, RoutedEventArgs e)
    {
        switch (sender)
        {
            case DataGrid dataGrid:
                _dgDataContext = dataGrid.DataContext;
                return;
            case ComboBox comboBox:
                comboBox.DataContext = _dgDataContext;
                return;
            default: return;
        }
    }

    private void ComboBox_DropDownOpened(object sender, EventArgs e)
    {
        if (sender is ComboBox combobox && combobox.Tag is not null)
        {
            string[] tagSplitedByDot = combobox.Tag.ToString().Split('.');

            if (_propertyInfos is null)
            {
                _dataAccess = _dgDataContext.GetType().GetProperty(tagSplitedByDot[0]).GetValue(_dgDataContext, null);
                _propertyInfos = _dataAccess.GetType().GetProperties();
            }

            var itemsource = Array.Find(_propertyInfos, prop => prop.Name == tagSplitedByDot[1]).GetValue(_dataAccess, null);
            combobox.ItemsSource = itemsource as IEnumerable<string>;
        }
    }

    private void FilterComboBox_KeyDown(object sender, KeyEventArgs e)
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
        ColumnHeaderToProperyName.TryGetValue(columnHeaderText, out string propName);
        if (propName is not null)
        {
            PropertyInfo[] filterpops = tagokViewModel.SelectedFilterValues.GetType().GetProperties();
            Array.Find(filterpops, p => p.Name == propName).SetValue(tagokViewModel.SelectedFilterValues, comboBox.SelectedValue);
            tagokViewModel.Filter(tagokViewModel.Tagok);
            if (e is not null)
                e.Handled = true;
        }
    }
}
