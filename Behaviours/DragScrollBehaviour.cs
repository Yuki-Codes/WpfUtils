namespace WpfUtils.Behaviours;

using DependencyPropertyGenerator;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUtils.Logging;

[AttachedDependencyProperty<bool, ScrollViewer>("DragScroll")]
public static partial class ScrollViewerExtensions
{
	static partial void OnDragScrollChanged(ScrollViewer scrollViewer, bool newValue)
	{
		if (newValue != true)
			return;

		new ScrollDragBehaviour(scrollViewer);
	}
}

public class ScrollDragBehaviour
{
	private readonly ScrollViewer scrollViewer;
	private Point startDragPoint;
	private double startDragHorizontalOffset;
	private double startDragVerticalOffset;
	private bool isDragging = false;

	public ScrollDragBehaviour(ScrollViewer scrollViewer)
	{
		this.scrollViewer = scrollViewer;

		this.scrollViewer.PreviewMouseDown += this.OnPreviewMouseDown;
		this.scrollViewer.MouseMove += this.OnMouseMove;
		this.scrollViewer.MouseUp += this.OnMouseUp;
	}

	private void OnPreviewMouseDown(object sender, MouseEventArgs e)
	{
		if (e.Handled)
			return;

		this.startDragPoint = e.GetPosition(this.scrollViewer);
		this.startDragHorizontalOffset = this.scrollViewer.HorizontalOffset;
		this.startDragVerticalOffset = this.scrollViewer.VerticalOffset;
	}

	private void OnMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed)
			return;

		if (e.Handled)
			return;

		if (this.scrollViewer.ScrollableHeight <= 1 && this.scrollViewer.ScrollableWidth <= 1)
			return;

		if (Mouse.Captured != null)
		{
			if (Mouse.Captured is not ItemsControl)
			{
				return;
			}
		}

		Point dragPoint = e.GetPosition(this.scrollViewer);
		Vector delta = dragPoint - this.startDragPoint;

		if (delta.Length <= 10)
			return;

		this.isDragging = true;
		this.scrollViewer.Cursor = Cursors.ScrollAll;

		this.scrollViewer.ScrollToHorizontalOffset(this.startDragHorizontalOffset - delta.X);
		this.scrollViewer.ScrollToVerticalOffset(this.startDragVerticalOffset - delta.Y);
		e.Handled = true;
	}

	private void OnMouseUp(object sender, MouseEventArgs e)
	{
		if (e.Handled)
			return;

		if (this.isDragging)
		{
			this.isDragging = false;

			e.Handled = true;
			this.scrollViewer.Cursor = null;
			this.scrollViewer.ReleaseMouseCapture();
		}
	}
}