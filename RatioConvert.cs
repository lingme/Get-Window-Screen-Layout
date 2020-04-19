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
                var maxW = screenArray.OrderByDescending(x => x.Bounds.Location.X).FirstOrDefault();
                var maxH = screenArray.OrderByDescending(x => x.Bounds.Location.Y).FirstOrDefault();
                return new Thickness(
                    0 - (maxW.Bounds.Size.Width + maxW.Bounds.Location.X) / 15f,
                    0 - (maxH.Bounds.Size.Height + maxH.Bounds.Location.Y) /15f,
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
