namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

[DependencyProperty<ControlTemplate>("Template")]
[DependencyProperty<object>("Content")]
[AttachedDependencyProperty<object>("Header")]
[AttachedDependencyProperty<DataTemplate>("HeaderTemplate")]
[DependencyProperty<Thickness>("Padding")]
[DependencyProperty<Brush>("Background")]
[ContentProperty("Content")]
public partial class PopOut : Popup, IAddChild
{
	private readonly ContentControl contentPresenter = new();

	public PopOut()
	{
		this.contentPresenter.Padding = this.Padding;
		this.contentPresenter.Background = this.Background;
		this.Child = this.contentPresenter;
	}

	void IAddChild.AddChild(object value)
	{
		this.contentPresenter.Content = value;
	}

	protected override void OnOpened(EventArgs e)
	{
		base.OnOpened(e);

		this.HorizontalOffset = -this.Margin.Left;

		if (this.PlacementTarget is FrameworkElement el)
		{
			double fullWidth = el.ActualWidth - (this.Margin.Left + this.Margin.Right);

			if (fullWidth > this.MaxWidth)
			{
				double delta = fullWidth - this.MaxWidth;
				fullWidth = this.MaxWidth;

				this.HorizontalOffset -= delta / 2;
			}

			this.Width = fullWidth;
			this.VerticalOffset = -(el.ActualHeight / 2);
		}
	}

	partial void OnTemplateChanged(ControlTemplate? newValue)
	{
		this.contentPresenter.Template = newValue;
	}

	partial void OnContentChanged(object? newValue)
	{
		this.contentPresenter.Content = newValue;
	}

	partial void OnPaddingChanged(Thickness newValue)
	{
		this.contentPresenter.Padding = newValue;
	}

	partial void OnBackgroundChanged(Brush? newValue)
	{
		if (newValue == null)
			newValue = new SolidColorBrush(Colors.Transparent);

		this.contentPresenter.Background = newValue;
	}
}
