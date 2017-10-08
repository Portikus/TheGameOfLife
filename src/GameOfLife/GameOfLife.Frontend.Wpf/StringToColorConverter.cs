using System;
using System.Globalization;
using System.Windows.Data;

namespace GameOfLife.Frontend.Wpf
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string name)
            {
                return ColorConversionHelper.CalculatePlayerBrush(name);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
