namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System.Windows;
using System.Windows.Input;

[DependencyProperty<double>("RelativeValue", DefaultValue = 0, DefaultBindingMode = DefaultBindingMode.TwoWay)]
[DependencyProperty<double>("RelativeRange")]
public partial class RelativeSlider : Slider
{
	private double relativeSliderStart;

	public RelativeSlider()
	{
		this.PreviewMouseDown += this.OnPreviewMouseDown;
		this.PreviewMouseUp += this.OnPreviewMouseUp;
		this.ValueChanged += this.OnValueChanged;
	}

	protected override void OnPreviewKeyDown(object sender, KeyEventArgs e)
	{
		this.relativeSliderStart = this.RelativeValue;
		base.OnPreviewKeyDown(sender, e);
	}

	protected override void OnPreviewKeyUp(object sender, KeyEventArgs e)
	{
		this.relativeSliderStart = this.RelativeValue;
		base.OnPreviewKeyUp(sender, e);
	}

	partial void OnRelativeRangeChanged(double newValue)
	{
		this.Minimum = -newValue;
		this.Maximum = newValue;
	}

	private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		this.RelativeValue = this.relativeSliderStart + this.Value;
	}

	private void OnPreviewMouseDown(object? sender, MouseButtonEventArgs e)
	{
		this.relativeSliderStart = this.RelativeValue;
	}

	private void OnPreviewMouseUp(object? sender, MouseButtonEventArgs e)
	{
		this.relativeSliderStart = this.RelativeValue;
		this.Value = 0;
	}
}
