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
[DependencyProperty<DataTemplate>("ContentTemplate")]
[AttachedDependencyProperty<object>("Header")]
[AttachedDependencyProperty<DataTemplate>("HeaderTemplate")]
[DependencyProperty<Thickness>("Padding")]
[DependencyProperty<Brush>("Background")]
[DependencyProperty<bool>("FitTarget", DefaultValue = true)]
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

	public static PopOut Show(UIElement placementTarget, UIElement content)
	{
		PopOut host = new();
		host.Content = content;
		host.PlacementTarget = placementTarget;
		host.Placement = PlacementMode.Bottom;
		host.FitTarget = false;
		host.Width = 300;
		host.AllowsTransparency = true;

		host.IsOpen = true;
		return host;
	}

	void IAddChild.AddChild(object value)
	{
		this.contentPresenter.Content = value;
	}

	protected override void OnOpened(EventArgs e)
	{
		base.OnOpened(e);

		if (this.Placement == PlacementMode.Bottom)
		{
			if (this.FitTarget)
			{
				this.HorizontalOffset = -this.Margin.Left;
			}

			if (this.PlacementTarget is FrameworkElement el)
			{
				double fullWidth = this.Width;

				if (this.FitTarget)
					fullWidth = el.ActualWidth;

				fullWidth = fullWidth - (this.Margin.Left + this.Margin.Right);

				if (fullWidth > this.MaxWidth)
				{
					double delta = fullWidth - this.MaxWidth;
					fullWidth = this.MaxWidth;

					this.HorizontalOffset -= delta / 2;
				}

				if (this.FitTarget)
					this.Width = fullWidth;

				this.VerticalOffset = -(el.ActualHeight / 2);
			}

			if (!this.FitTarget)
			{
				this.HorizontalOffset = 40;
			}
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
