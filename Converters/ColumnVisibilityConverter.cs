namespace WpfUtils.Converters;

using System.Windows;

public class ColumnVisibilityConverter : ConverterBase<bool, GridLength>
{
	protected override GridLength Convert(bool value)
	{
		GridLength length = GridLength.Auto;

		if (this.Parameter != null)
		{
			GridLengthConverter lConverter = new();
			object? paramLengthObj = lConverter.ConvertFrom(this.Parameter);

			if (paramLengthObj is GridLength gl)
				length = gl;
		}

		return value ? length : new GridLength(0);
	}
}
