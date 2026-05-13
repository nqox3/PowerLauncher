using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MinecraftLauncher.Models;

namespace MinecraftLauncher.Converters;

public class LoaderToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ModLoaderTag tag && tag != ModLoaderTag.Any)
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
