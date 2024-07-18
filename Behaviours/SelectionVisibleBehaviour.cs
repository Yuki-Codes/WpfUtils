namespace WpfUtils.Behaviours;
using System.Windows;
using System.Windows.Controls;

public class SelectionVisibleBehaviour : Behaviour
{
	public SelectionVisibleBehaviour(DependencyObject host)
		: base(host)
	{
	}

	public SelectionVisibleBehaviour(DependencyObject el, object? value)
		: base(el, value)
	{
	}

	public static void SetSelectionVisible(DependencyObject host, bool enable)
	{
		host.AttachHandler<SelectionVisibleBehaviour>(enable);
	}

	public override void OnLoaded()
	{
		base.OnLoaded();

		ListBox? listBox = this.Host as ListBox;

		if (listBox == null)
			listBox = this.Host.FindChild<ListBox>();

		if (listBox == null)
			return;

		listBox.SelectionChanged += (s, e) =>
		{
			if (e.AddedItems != null && e.AddedItems.Count > 0)
			{
				listBox.ScrollIntoView(e.AddedItems[0]);
			}
		};
	}
}
