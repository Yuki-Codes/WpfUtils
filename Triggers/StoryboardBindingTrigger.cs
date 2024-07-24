namespace WpfUtils.Triggers;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Animation;
using WpfUtils.Logging;

public class StoryboardBindingTrigger : EventTrigger
{
	public static readonly DependencyProperty BindingProperty = DependencyProperty.Register(
		nameof(StoryboardBindingTrigger.Binding),
		typeof(object),
		typeof(StoryboardBindingTrigger),
		new(null, OnBindingValueChanged));

	public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
		nameof(StoryboardBindingTrigger.Target),
		typeof(FrameworkElement),
		typeof(StoryboardBindingTrigger),
		new(null, OnTargetChanged));

	private static readonly MethodInfo? ActionInvokeMethod;

	private static readonly RoutedEvent DummyEvent = EventManager.RegisterRoutedEvent(
	   "BindMatched",
	   RoutingStrategy.Bubble,
	   typeof(RoutedEventHandler),
	   typeof(StoryboardBindingTrigger));

	private bool? isEntered = null;

	static StoryboardBindingTrigger()
	{
		ActionInvokeMethod = typeof(TriggerAction).GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(FrameworkElement)]);
	}

	public StoryboardBindingTrigger()
		: base(DummyEvent)
	{
	}

	public object? Binding
	{
		get => this.GetValue(BindingProperty);
		set => this.SetValue(BindingProperty, value);
	}

	public bool? Value { get; set; }

	public FrameworkElement? Target
	{
		get => (FrameworkElement)this.GetValue(TargetProperty);
		set => this.SetValue(TargetProperty, value);
	}

	public Storyboard? EnterStoryboard { get; set; }
	public Storyboard? ExitStoryboard { get; set; }

	private static void OnBindingValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is StoryboardBindingTrigger trigger)
		{
			if (trigger.Binding == null || trigger.Value == null)
				return;

			if (trigger.Binding.Equals(trigger.Value))
			{
				trigger.OnEnter();
			}
			else if (trigger.isEntered != false)
			{
				trigger.OnExit();
			}
		}
	}

	private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is StoryboardBindingTrigger trigger)
		{
			trigger.isEntered = null;
			OnBindingValueChanged(d, default);
		}
	}

	private void OnEnter()
	{
		this.isEntered = true;

		if (this.Target == null)
			return;

		try
		{
			this.EnterStoryboard?.Begin(this.Target);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error executing enter storyboard");
		}
	}

	private void OnExit()
	{
		this.isEntered = false;

		if (this.Target == null)
			return;

		try
		{
			this.ExitStoryboard?.Begin(this.Target);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error executing exit storyboard");
		}
	}
}