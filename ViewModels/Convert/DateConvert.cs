using System;
using System.Globalization;
using System.Windows.Data;

namespace URApplication.ViewModels.Convert
{
    public class DateConvert : IValueConverter
    {
        private string _date;

        public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            var value = _date = (string)obj;
            if (value is null or "") return "";
            if (DateTime.TryParse(value, out var dateTime)) return dateTime.ToString("dd.MM.yy");

            var newValue = value[..4] + "." + value[4..6] + "." + value[6..];
            return DateTime.TryParse(newValue, out dateTime) ? dateTime.ToString("dd.MM.yy") : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _date;
        }
    }
}