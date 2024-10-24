namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

[DependencyProperty<ControlTemplate>("Template")]
[DependencyProperty<object>("Content")]
[DependencyProperty<DataTemplate>("ContentTemplate")]
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
		this.Placement = PlacementMode.Center;
	}

	public static PopOut Show(UIElement placementTarget, UIElement content)
	{
		PopOut host = new();
		host.Content = content;
		host.PlacementTarget = placementTarget;
		host.Placement = PlacementMode.Bottom;
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

		if (this.Placement == PlacementMode.Center
			&& this.PlacementTarget is FrameworkElement el
			&& this.Child is FrameworkElement childEl)
		{
			this.VerticalOffset = (el.ActualHeight / 2) + (childEl.ActualHeight / 2) + this.Margin.Top;
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
