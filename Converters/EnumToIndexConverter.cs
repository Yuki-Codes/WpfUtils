namespace WpfUtils.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

[ValueConversion(typeof(Enum), typeof(int))]
public class EnumToIndexConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
			return -1;

		return (int)value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not int iVal)
			return Binding.DoNothing;

		if (!targetType.IsEnum)
			throw new Exception("Enum converter can only be used on an enum type");

		return Enum.ToObject(targetType, iVal);
	}
}
