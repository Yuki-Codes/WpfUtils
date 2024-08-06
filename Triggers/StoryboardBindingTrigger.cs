namespace WpfUtils.Triggers;

using System;
using System.Windows.Media.Animation;
using WpfUtils.Logging;

public class StoryboardBindingTrigger : StoryboardBindingTriggerBase
{
	private bool? isEntered = null;

	public Storyboard? EnterStoryboard { get; set; }
	public Storyboard? ExitStoryboard { get; set; }

	public bool? Value { get; set; }

	protected override void OnBindingValueChanged(object? newValue)
	{
		{
			if (this.Binding == null || this.Value == null)
				return;

			if (this.Binding.Equals(this.Value))
			{
				this.OnEnter();
			}
			else if (this.isEntered != false)
			{
				this.OnExit();
			}
		}
	}

	protected override void OnTargetChanged()
	{
		this.isEntered = null;
		base.OnTargetChanged();
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