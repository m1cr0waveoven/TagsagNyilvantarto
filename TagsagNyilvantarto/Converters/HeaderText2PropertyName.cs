using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TagsagNyilvantarto.Converters;

[ValueConversion(typeof(string), typeof(string))]
internal sealed class HeaderText2PropertyName : IValueConverter
{
    private static readonly Dictionary<string, string> HeaderNameToPropertyName = new(capacity: 11);
    public HeaderText2PropertyName()
    {
        HeaderNameToPropertyName.Add("Id", "DataAccess.Idk");
        HeaderNameToPropertyName.Add("Név", "DataAccess.Nevek");
        HeaderNameToPropertyName.Add("Születés", "DataAccess.Szuletesek");
        HeaderNameToPropertyName.Add("Email", "DataAccess.Emailek");
        HeaderNameToPropertyName.Add("Telefon", "DataAccess.Telefonok");
        HeaderNameToPropertyName.Add("Tisztség", "DataAccess.Tisztsegek");
        HeaderNameToPropertyName.Add("Tagság kezdete", "DataAccess.Tagsagkezdetek");
        HeaderNameToPropertyName.Add("Tagság jogállása", "DataAccess.Tagsagjogallasok");
        HeaderNameToPropertyName.Add("AdatokTípusa", "DataAccess.Adattipusok");
        HeaderNameToPropertyName.Add("Képviselő", "DataAccess.Kepviselo");
        HeaderNameToPropertyName.Add("Admin", "DataAccess.Admin");
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not String)
            return value;

        HeaderNameToPropertyName.TryGetValue((string)value, out string propname);
        return propname;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Nincs visszafelé konvertálás
        throw new InvalidOperationException("Can not convert backwards");
    }
}
