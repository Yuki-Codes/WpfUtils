namespace WpfUtils.Triggers;

using System.Reflection;
using System.Windows;
using System.Windows.Media.Animation;

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
		new(null));

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
			if (e.NewValue == null || trigger.Value == null)
				return;

			if (e.NewValue.Equals(trigger.Value))
			{
				trigger.OnEnter();
			}
			else if (trigger.isEntered != false)
			{
				trigger.OnExit();
			}
		}
	}

	private void OnEnter()
	{
		this.isEntered = true;

		this.EnterStoryboard?.Begin(this.Target);
	}

	private void OnExit()
	{
		this.isEntered = false;

		this.ExitStoryboard?.Begin(this.Target);
	}
}