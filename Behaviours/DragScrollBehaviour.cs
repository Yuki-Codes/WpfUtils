namespace WpfUtils.Behaviours;

using DependencyPropertyGenerator;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
	private const double MaxVelocity = 50;

	private readonly ScrollViewer scrollViewer;
	private Point startDragPoint;
	private double startDragHorizontalOffset;
	private double startDragVerticalOffset;
	private bool isDragging = false;
	private Task? animationTask;
	private bool isMouseDown = false;

	public ScrollDragBehaviour(ScrollViewer scrollViewer)
	{
		this.scrollViewer = scrollViewer;

		this.scrollViewer.PreviewMouseDown += this.OnPreviewMouseDown;
		this.scrollViewer.PreviewMouseMove += this.OnMouseMove;
		this.scrollViewer.MouseUp += this.OnMouseUp;
	}

	private void OnPreviewMouseDown(object sender, MouseEventArgs e)
	{
		if (e.Handled)
			return;

		this.isMouseDown = true;
		this.startDragPoint = e.GetPosition(this.scrollViewer);
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
		Vector totalDelta = dragPoint - this.startDragPoint;

		if (!this.isDragging && totalDelta.Length <= 5)
			return;

		if (!this.isDragging)
		{
			this.startDragPoint = e.GetPosition(this.scrollViewer);
			this.startDragHorizontalOffset = this.scrollViewer.HorizontalOffset;
			this.startDragVerticalOffset = this.scrollViewer.VerticalOffset;

			if (this.scrollViewer.Content is FrameworkElement fe)
				fe.IsHitTestVisible = false;

			this.isDragging = true;
			this.scrollViewer.Cursor = Cursors.ScrollAll;

			this.Animate();
		}

		this.scrollViewer.ScrollToHorizontalOffset(this.startDragHorizontalOffset - totalDelta.X);
		this.scrollViewer.ScrollToVerticalOffset(this.startDragVerticalOffset - totalDelta.Y);
		e.Handled = true;
	}

	private void OnMouseUp(object sender, MouseEventArgs e)
	{
		this.isMouseDown = false;

		if (e.Handled)
			return;

		if (this.isDragging)
		{
			this.isDragging = false;

			e.Handled = true;
			this.scrollViewer.Cursor = null;

			if (this.scrollViewer.Content is FrameworkElement fe)
			{
				fe.IsHitTestVisible = true;
			}
		}
	}

	private void Animate()
	{
		if (this.animationTask == null || this.animationTask.IsCompleted)
		{
			this.animationTask = this.AnimateToTarget();
		}
	}

	private async Task AnimateToTarget()
	{
		Vector velocity = new Vector(0, 0);
		Point lastDragPoint = Mouse.GetPosition(this.scrollViewer);

		// drag
		do
		{
			Point dragPoint = Mouse.GetPosition(this.scrollViewer);
			velocity = dragPoint - lastDragPoint;
			lastDragPoint = dragPoint;

			await Task.Delay(100);
			await this.scrollViewer.Dispatcher.MainThread();
		}
		while (this.isMouseDown && this.isDragging);

		// release
		velocity *= 0.25;

		// slide
		do
		{
			velocity.X = Math.Clamp(velocity.X, -MaxVelocity, MaxVelocity);
			velocity.Y = Math.Clamp(velocity.Y, -MaxVelocity, MaxVelocity);

			this.scrollViewer.ScrollToHorizontalOffset(this.scrollViewer.HorizontalOffset - velocity.X);
			this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset - velocity.Y);

			velocity.X /= 1.05;
			velocity.Y /= 1.05;

			await Task.Delay(15);
			await this.scrollViewer.Dispatcher.MainThread();
		}
		while (velocity.Length > 0.01 && !this.isMouseDown);
	}
}