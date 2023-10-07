﻿namespace WpfUtils.Converters;

using System;
using System.Windows.Data;
using FontAwesome.Sharp;

[ValueConversion(typeof(string), typeof(IconChar))]
public class IconConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		if (value == null)
			return IconChar.None;

		return (IconChar)Enum.Parse(typeof(IconChar), (string)value, true);
	}

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
