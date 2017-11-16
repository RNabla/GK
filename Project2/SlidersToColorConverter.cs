using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Project2
{
    class SlidersToColorConverter :IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var r = System.Convert.ToByte(values[0]);
            var g = System.Convert.ToByte(values[1]);
            var b = System.Convert.ToByte(values[2]);
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
