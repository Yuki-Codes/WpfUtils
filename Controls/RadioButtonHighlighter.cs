namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

[AttachedDependencyProperty<bool>("UseGroupHighlight")]
[DependencyProperty<double>("Left")]
[DependencyProperty<double>("Right")]
[DependencyProperty<double>("Top")]
[DependencyProperty<double>("Bottom")]
[DependencyProperty<Orientation>("Orientation")]
public partial class RadioButtonHighlighter : ContentControl
{
	private readonly List<RadioButton> radioButtons = new();
	private readonly Storyboard storyboard;
	private readonly DoubleAnimation leftAnimation;
	private readonly DoubleAnimation rightAnimation;
	private readonly DoubleAnimation topAnimation;
	private readonly DoubleAnimation bottomAnimation;
	private FrameworkElement? highlight;
	private int currentIndex = 0;

	public RadioButtonHighlighter()
	{
		this.storyboard = new Storyboard();

		this.leftAnimation = this.CreateAnimation(LeftProperty);
		this.rightAnimation = this.CreateAnimation(RightProperty);
		this.topAnimation = this.CreateAnimation(TopProperty);
		this.bottomAnimation = this.CreateAnimation(BottomProperty);
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		this.highlight = this.GetTemplateChild("PART_Highlight") as FrameworkElement;

		this.UpdateBounds();
	}

	protected void AddButton(RadioButton button)
	{
		this.radioButtons.Add(button);
		button.Checked += this.OnButtonChecked;
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);

		this.UpdateBounds();
	}

	static partial void OnUseGroupHighlightChanged(DependencyObject dependencyObject, bool newValue)
	{
		if (newValue == false)
			return;

		if (dependencyObject is RadioButton btn)
		{
			RadioButtonHighlighter? highlighter = btn.FindParent<RadioButtonHighlighter>();
			if (highlighter == null)
				throw new Exception("No Radio Button highlighter in hierarchy for UseGroupHighlight");

			highlighter.AddButton(btn);
		}
	}

	private DoubleAnimation CreateAnimation(DependencyProperty property)
	{
		DoubleAnimation animation = new();
		animation.Duration = new(TimeSpan.FromMilliseconds(500));
		this.storyboard.Children.Add(animation);
		CubicEase ease = new();
		ease.EasingMode = EasingMode.EaseInOut;
		animation.EasingFunction = ease;
		Storyboard.SetTarget(animation, this);
		Storyboard.SetTargetProperty(animation, new PropertyPath(property.Name));
		return animation;
	}

	private void OnButtonChecked(object sender, RoutedEventArgs e)
	{
		if (sender is RadioButton button)
		{
			this.storyboard.Stop();

			int fromIndex = this.currentIndex;
			this.currentIndex = this.radioButtons.IndexOf(button);

			GeneralTransform transform = button.TransformToAncestor(this);
			Rect bounds = transform.TransformBounds(new(0, 0, button.ActualWidth, button.ActualHeight));

			if (this.highlight != null)
			{
				if (fromIndex > this.currentIndex)
				{
					this.leftAnimation.AccelerationRatio = 0.1;
					this.leftAnimation.DecelerationRatio = 0.5;
					this.rightAnimation.AccelerationRatio = 0.5;
					this.rightAnimation.DecelerationRatio = 0.1;

					this.topAnimation.AccelerationRatio = 0.1;
					this.topAnimation.DecelerationRatio = 0.5;
					this.bottomAnimation.AccelerationRatio = 0.5;
					this.bottomAnimation.DecelerationRatio = 0.1;
				}
				else
				{
					this.leftAnimation.AccelerationRatio = 0.5;
					this.leftAnimation.DecelerationRatio = 0.1;
					this.rightAnimation.AccelerationRatio = 0.1;
					this.rightAnimation.DecelerationRatio = 0.5;

					this.topAnimation.AccelerationRatio = 0.5;
					this.topAnimation.DecelerationRatio = 0.1;
					this.bottomAnimation.AccelerationRatio = 0.1;
					this.bottomAnimation.DecelerationRatio = 0.5;
				}

				this.leftAnimation.To = bounds.Left;
				this.rightAnimation.To = this.ActualWidth - bounds.Right;
				this.topAnimation.To = bounds.Top;
				this.bottomAnimation.To = this.ActualHeight - bounds.Bottom;
				this.storyboard.Begin();
			}
		}
	}

	private void UpdateBounds()
	{
		if (this.highlight == null)
			return;

		if (this.currentIndex < 0 || this.currentIndex >= this.radioButtons.Count)
			return;

		RadioButton button = this.radioButtons[this.currentIndex];
		Rect bounds;

		try
		{
			GeneralTransform transform = button.TransformToAncestor(this);
			bounds = transform.TransformBounds(new(0, 0, button.ActualWidth, button.ActualHeight));
		}
		catch (InvalidOperationException)
		{
		}

		Thickness margin = this.highlight.Margin;
		margin.Left = bounds.Left;
		margin.Right = this.ActualWidth - bounds.Right;
		margin.Top = bounds.Top;
		margin.Bottom = this.ActualHeight - bounds.Bottom;
		this.highlight.Margin = margin;
	}

	partial void OnLeftChanged(double newValue)
	{
		if (this.highlight == null)
			return;

		Thickness margin = this.highlight.Margin;
		margin.Left = newValue;
		this.highlight.Margin = margin;
	}

	partial void OnRightChanged(double newValue)
	{
		if (this.highlight == null)
			return;

		Thickness margin = this.highlight.Margin;
		margin.Right = newValue;
		this.highlight.Margin = margin;
	}

	partial void OnTopChanged(double newValue)
	{
		if (this.highlight == null)
			return;

		Thickness margin = this.highlight.Margin;
		margin.Top = newValue;
		this.highlight.Margin = margin;
	}

	partial void OnBottomChanged(double newValue)
	{
		if (this.highlight == null)
			return;

		Thickness margin = this.highlight.Margin;
		margin.Bottom = newValue;
		this.highlight.Margin = margin;
	}
}
