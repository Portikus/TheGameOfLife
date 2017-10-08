using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GameOfLife.Api.Model;

namespace GameOfLife.Frontend.Wpf
{
    internal static class ColorConversionHelper
    {
        internal static SolidColorBrush CalculatePlayerBrush(string name)
        {
            var hash = name.GetHashCode() + name.GetHashCode().ToString();
            var r = (byte)int.Parse(hash.Substring(0, 2));
            var g = (byte)int.Parse(hash.Substring(2, 4));
            var b = (byte)int.Parse(hash.Substring(4, 6));
            return new SolidColorBrush(new Color { R = r, G = g, B = b, A = byte.MaxValue });
        }
    }
}
