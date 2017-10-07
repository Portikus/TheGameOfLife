using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GameOfLife.Api.Model;

namespace GameOfLife.Frontend.Wpf
{
    public class NeighborConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var entity = value.FirstOrDefault(x => x is Entity) as Entity;
            if (entity != null)
            {
                return new Thickness
                {
                    Top = entity.NeighborInfo.HasNorthNeighbor ? 0 : 1,
                    Bottom = entity.NeighborInfo.HasSouthNeighbor ? 0 : 1,
                    Left = entity.NeighborInfo.HasWestNeighbor ? 0 : 1,
                    Right = entity.NeighborInfo.HasEastNeighbor ? 0 : 1
                };
            }
            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}