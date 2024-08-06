namespace WpfUtils.Triggers;

using System.Reflection;
using System.Windows;

public abstract class StoryboardBindingTriggerBase : EventTrigger
{
	public static readonly DependencyProperty BindingProperty = DependencyProperty.Register(
		nameof(StoryboardBindingTriggerBase.Binding),
		typeof(object),
		typeof(StoryboardBindingTriggerBase),
		new(null, OnBindingValueChanged));

	public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
		nameof(StoryboardBindingTriggerBase.Target),
		typeof(FrameworkElement),
		typeof(StoryboardBindingTriggerBase),
		new(null, OnTargetChanged));

	private static readonly MethodInfo? ActionInvokeMethod;

	private static readonly RoutedEvent DummyEvent = EventManager.RegisterRoutedEvent(
	   "BindMatched",
	   RoutingStrategy.Bubble,
	   typeof(RoutedEventHandler),
	   typeof(StoryboardBindingTriggerBase));

	static StoryboardBindingTriggerBase()
	{
		ActionInvokeMethod = typeof(TriggerAction).GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(FrameworkElement)]);
	}

	public StoryboardBindingTriggerBase()
		: base(DummyEvent)
	{
	}

	public object? Binding
	{
		get => this.GetValue(BindingProperty);
		set => this.SetValue(BindingProperty, value);
	}

	public FrameworkElement? Target
	{
		get => (FrameworkElement)this.GetValue(TargetProperty);
		set => this.SetValue(TargetProperty, value);
	}

	protected abstract void OnBindingValueChanged(object? newValue);

	protected virtual void OnTargetChanged()
	{
		this.OnBindingValueChanged(this.Binding);
	}

	private static void OnBindingValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is StoryboardBindingTriggerBase trigger)
		{
			trigger.OnBindingValueChanged(trigger.Binding);
		}
	}

	private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is StoryboardBindingTriggerBase trigger)
		{
			trigger.OnTargetChanged();
		}
	}
}
