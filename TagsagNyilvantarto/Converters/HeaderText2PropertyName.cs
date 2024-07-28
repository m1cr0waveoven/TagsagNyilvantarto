using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TagsagNyilvantarto.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    internal sealed class HeaderText2PropertyName : IValueConverter
    {
        private readonly Dictionary<string, string> _headerNameToPropertyName = new Dictionary<string, string>(capacity: 9);
        public HeaderText2PropertyName()
        {
            _headerNameToPropertyName.Add("Id", "DataAccess.Idk");
            _headerNameToPropertyName.Add("Név", "DataAccess.Nevek");
            _headerNameToPropertyName.Add("Születés", "DataAccess.Szuletesek");
            _headerNameToPropertyName.Add("Email", "DataAccess.Emailek");
            _headerNameToPropertyName.Add("Telefon", "DataAccess.Telefonok");
            _headerNameToPropertyName.Add("Tisztség", "DataAccess.Tisztsegek");
            _headerNameToPropertyName.Add("Tagság kezdete", "DataAccess.Tagsagkezdetek");
            _headerNameToPropertyName.Add("Tagság jogállása", "DataAccess.Tagsagjogallasok");
            _headerNameToPropertyName.Add("AdatokTípusa", "DataAccess.Adattipusok");
            _headerNameToPropertyName.Add("Képviselő", "DataAccess.Kepviselo");
            _headerNameToPropertyName.Add("Admin", "DataAccess.Admin");
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is String))
                return value;

            _headerNameToPropertyName.TryGetValue((string)value, out string propname);
            return propname;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Nincs visszafelé konvertálás
            throw new InvalidOperationException("Can not convert backwards");
        }
    }
}
