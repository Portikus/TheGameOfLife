using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using GameOfLife.Api.Model;

namespace GameOfLife.Frontend.Wpf
{
    public class TileToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var entity = value.FirstOrDefault(x => x is Entity) as Entity;
            if (entity != null)
            {
                return GetPlayerColor(entity);
            }
            var temp = value.FirstOrDefault(x => x is Temperature) as Temperature;
            return GetTemperatureColor(temp);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private object GetTemperatureColor(Temperature temperature)
        {
            var redValue = temperature.Maximum - temperature.Value;
            var blueValue = temperature.Value - temperature.Minimum;
            var minMaxDifference = Math.Abs(temperature.Maximum) + Math.Abs(temperature.Minimum);
            var factor = byte.MaxValue / minMaxDifference;
            return new SolidColorBrush(new Color
            {
                R = (byte) (byte.MaxValue - redValue * factor),
                G = 0,
                B = (byte) (byte.MaxValue - blueValue * factor),
                A = byte.MaxValue
            });
        }

        private object GetPlayerColor(Entity entity)
        {
            var name = entity.Owner.Name;
            var hash = name.GetHashCode() + name.GetHashCode().ToString();
            var r = (byte) int.Parse(hash.Substring(0, 2));
            var g = (byte) int.Parse(hash.Substring(2, 4));
            var b = (byte) int.Parse(hash.Substring(4, 6));
            return new SolidColorBrush(new Color {R = r, G = g, B = b, A = byte.MaxValue});
        }
    }
}