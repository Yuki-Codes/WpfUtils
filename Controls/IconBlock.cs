namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using FontAwesome.Sharp;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

[DependencyProperty<IconChar>("Icon")]
[DependencyProperty<IconFont>("IconFont")]
public partial class IconBlock : TextBlock
{
	private readonly MethodInfo? fontForMethod;

	public IconBlock()
	{
		this.fontForMethod = typeof(IconHelper).GetMethod("FontFor", BindingFlags.NonPublic | BindingFlags.Static, [typeof(IconChar), typeof(IconFont)]);
	}

	partial void OnIconChanged(IconChar newValue)
	{
		FontFamily? fontFamily = this.FontFor();
		if (fontFamily == null)
			return;

		this.SetValue(TextBlock.FontFamilyProperty, fontFamily);
		this.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
		this.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
		this.SetValue(TextBlock.TextProperty, char.ConvertFromUtf32((int)newValue));
	}

	partial void OnIconFontChanged(IconFont newValue)
	{
		FontFamily? fontFamily = this.FontFor();
		if (fontFamily == null)
			return;

		this.SetValue(TextBlock.FontFamilyProperty, fontFamily);
	}

	private FontFamily? FontFor()
	{
		return this.fontForMethod?.Invoke(null, [this.Icon, this.IconFont]) as FontFamily;
	}
}