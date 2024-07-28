using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TagsagNyilvantarto.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    internal class HeaderText2PropertyName : IValueConverter
    {
        private Dictionary<string, string> keyValuePairs = new Dictionary<string, string>(capacity: 9);
        public HeaderText2PropertyName()
        {
            keyValuePairs.Add("Id", "DataAccess.Idk");
            keyValuePairs.Add("Név", "DataAccess.Nevek");
            keyValuePairs.Add("Születés", "DataAccess.Szuletesek");
            keyValuePairs.Add("Email", "DataAccess.Emailek");
            keyValuePairs.Add("Telefon", "DataAccess.Telefonok");
            keyValuePairs.Add("Tisztség", "DataAccess.Tisztsegek");
            keyValuePairs.Add("Tagság kezdete", "DataAccess.Tagsagkezdetek");
            keyValuePairs.Add("Tagság jogállása", "DataAccess.Tagsagjogallasok");
            keyValuePairs.Add("AdatokTípusa", "DataAccess.Adattipusok");
            keyValuePairs.Add("Képviselő", "DataAccess.Kepviselo");
            keyValuePairs.Add("Admin", "DataAccess.Admin");
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is String))
                return value;

            keyValuePairs.TryGetValue((string)value, out string propname);
            return propname;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Ninc svisszafelé konvertálás
            throw new NotImplementedException();
        }
    }
}
