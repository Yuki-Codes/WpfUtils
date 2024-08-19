namespace WpfUtils.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public class ListBoxEx : ListBox
{
	protected override DependencyObject GetContainerForItemOverride()
	{
		return new ListBoxItemEx();
	}
}

public class ListBoxItemEx : ListBoxItem
{
	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		// intentional don't call base so that selection happens on release
	}

	protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		// intentional flip so that selection happens on release
		base.OnMouseLeftButtonDown(e);
	}
}
