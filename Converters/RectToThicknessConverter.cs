namespace WpfUtils.Converters;

using System.Windows;

public class RectToThicknessConverter : ConverterBase<Rect, Thickness>
{
	protected override Thickness Convert(Rect value)
	{
		return new Thickness(value.Left, value.Top, value.Right, value.Bottom);
	}

	protected override Rect ConvertBack(Thickness value)
	{
		return new Rect(value.Left, value.Top, value.Right, value.Bottom);
	}
}
