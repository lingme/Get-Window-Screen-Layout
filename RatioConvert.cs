using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gundam.Spike.ScreenInfo
{
    public class RatioConvert : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Rect rect)
            {
                return (string)parameter == "X" ? rect.Location.X / 15f - rect.Size.Width / 15f / 2 : rect.Location.Y / 15f - rect.Size.Height / 15f / 2;
            }
            else
            {
                return (double)value / 15f;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
