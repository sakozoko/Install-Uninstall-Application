using System;
using System.Globalization;
using System.Windows.Data;

namespace URApplication.ViewModels.Convert
{
    [ValueConversion(typeof(double), typeof(string))]
    public class WeightConvert : IValueConverter
    {
        public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            if (obj is null) return "";
            if (!double.TryParse(obj.ToString(), out var value)) return "";
            var str = (value / 1024) switch
            {
                > 1 and < 1000 => $"{(float)(value / 1024):0.##}" + " MB",
                >= 1000 => $"{(float)(value / 1048576):0.##}" + " GB",
                _ => value == 0d ? "" : $"{value:0.##}" + " KB"
            };
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}