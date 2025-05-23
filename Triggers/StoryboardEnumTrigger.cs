﻿namespace WpfUtils.Triggers;

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Animation;

public class StoryboardEnumTrigger : StoryboardBindingTriggerBase
{
	public StoryboardEnumTrigger()
		: base()
	{
	}

	public List<Storyboard> Storyboards { get; init; } = new();

	protected override void OnBindingValueChanged(object? newValue)
	{
		string? name = newValue?.ToString();

		if (this.Target == null)
			return;

		foreach (Storyboard storyboard in this.Storyboards)
		{
			storyboard.Stop(this.Target);
		}

		foreach (Storyboard storyboard in this.Storyboards)
		{
			if (storyboard.Name == name)
			{
				storyboard.FillBehavior = FillBehavior.HoldEnd;
				storyboard.Completed += (s, e) =>
				{
					this.Binding = "None";
				};

				storyboard.Begin(this.Target);
			}
		}
	}
}
