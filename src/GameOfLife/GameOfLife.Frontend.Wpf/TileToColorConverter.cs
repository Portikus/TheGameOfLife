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
            if (value.FirstOrDefault(x => x is Entity) is Entity entity)
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
            var red = temperature.Value <= temperature.MedianValue ? 0 : byte.MaxValue;
            var blue = temperature.Value >= temperature.MedianValue ? 0 : byte.MaxValue;

            var medianBorderDifference = Math.Abs(temperature.Minimum - temperature.MedianValue);
            var valueMedianDifference = temperature.Value < 0
                ? Math.Abs(Math.Abs(temperature.MedianValue) + Math.Abs(temperature.Value))
                : Math.Abs(Math.Abs(temperature.MedianValue) - Math.Abs(temperature.Value));

            var factor = valueMedianDifference / medianBorderDifference;
            var alpha = factor * byte.MaxValue;

            return new SolidColorBrush(new Color
            {
                R = (byte) red,
                G = 0,
                B = (byte) blue,
                A = (byte) alpha
            });
        }

        private object GetPlayerColor(Entity entity)
        {
            return ColorConversionHelper.CalculatePlayerBrush(entity.Owner.Name);
        }
    }
}