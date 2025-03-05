namespace WpfUtils.Extensions;

using System.Windows;
using System.Windows.Controls;

public static class ItemsControlExtensions
{
	public static int GetVisibleItemsCount(this ItemsControl self)
	{
		Rect controlBounds = new Rect(0.0, 0.0, self.ActualWidth, self.ActualHeight);
		int count = 0;

		foreach (object? item in self.Items)
		{
			DependencyObject container = self.ItemContainerGenerator.ContainerFromItem(item);

			if (container is FrameworkElement el && el.IsVisible)
			{
				Rect containerBounds = el.TransformToAncestor(self).TransformBounds(new Rect(0, 0, el.ActualWidth, el.ActualHeight));

				if (controlBounds.Contains(containerBounds.TopLeft) && controlBounds.Contains(containerBounds.BottomRight))
				{
					count++;
				}
			}
		}

		return count;
	}

	public static void HideClippedItems(this ItemsControl self, long opacity = 0)
	{
		Rect controlBounds = new Rect(0.0, 0.0, self.ActualWidth, self.ActualHeight);

		foreach (object? item in self.Items)
		{
			DependencyObject container = self.ItemContainerGenerator.ContainerFromItem(item);

			if (container is FrameworkElement el && el.IsVisible)
			{
				Rect containerBounds = el.TransformToAncestor(self).TransformBounds(new Rect(0, 0, el.ActualWidth, el.ActualHeight));

				if (controlBounds.Contains(containerBounds.TopLeft) != controlBounds.Contains(containerBounds.BottomRight))
				{
					el.Opacity = opacity;
				}
				else
				{
					el.Opacity = 1.0;
				}
			}
		}
	}
}
