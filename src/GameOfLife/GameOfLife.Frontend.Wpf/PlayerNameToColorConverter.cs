using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using GameOfLife.Api.Model;

namespace GameOfLife.Frontend.Wpf
{
    public class PlayerNameToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Entity entity)
            {
                var name = entity.Owner.Name;
                var hash = name.GetHashCode() + name.GetHashCode().ToString();
                var r = (byte)int.Parse(hash.Substring(0, 2));
                var g = (byte)int.Parse(hash.Substring(2, 4));
                var b = (byte)int.Parse(hash.Substring(4, 6));
                return new SolidColorBrush(new Color {R = r, G = g, B = b, A = byte.MaxValue});
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}