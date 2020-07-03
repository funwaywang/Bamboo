using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Bamboo
{
    [ValueConversion(typeof(BitmapSource), typeof(double))]
    public class ImageWidthConverter : IValueConverter
    {
        public double? MaxWidth { get; set; }

        public double? MinWidth { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapSource ms)
            {
                var width = ms.Width;
                if (MaxWidth.HasValue)
                {
                    width = Math.Min(width, MaxWidth.Value);
                }
                if (MinWidth.HasValue)
                {
                    width = Math.Max(width, MinWidth.Value);
                }
                return width;
            }

            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
