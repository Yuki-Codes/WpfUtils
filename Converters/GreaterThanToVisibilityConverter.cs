namespace WpfUtils.Converters;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

public class GreaterThanToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		double dV = System.Convert.ToDouble(value);
		double dP = System.Convert.ToDouble(parameter);

		return dV > dP ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}