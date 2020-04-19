using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace Gundam.Spike.ScreenInfo
{
    public class RatioConvert : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 15f;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarginConvert : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Screen> screenArray)
            {
                return new Thickness(
                    0 - screenArray.Sum(x => x.Bounds.Size.Width)/ 15f ,
                    0 - screenArray.Sum(x => x.Bounds.Size.Height)/ 15f,
                    0, 0
                    );
            }
            return new Thickness();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
