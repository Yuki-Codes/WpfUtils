namespace WpfUtils.Sequence;

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

public partial class Sequencer : UserControl
{
	public Sequencer()
	{
		this.InitializeComponent();
		this.ContentArea.DataContext = this;
	}

	public ObservableCollection<SequenceTrack> Tracks { get; init; } = new()
	{
		new("Hello"),
		new("world"),
		new("Hello"),
		new("world"),
		new("Hello"),
		new("world"),
		new("Hello"),
		new("world"),
		new("Hello"),
		new("world"),
	};

	public static double DurationToPixels(TimeSpan duration)
	{
		return duration.TotalSeconds * 50;
	}
}

public class SequenceTrack
{
	public SequenceTrack(string name)
	{
		this.Name = name;
	}

	public SequenceTrack()
	{
	}

	public string? Name { get; set; }
	public ObservableCollection<SequenceNode> Nodes { get; init; } = new()
	{
		new()
		{
			StartTime = TimeSpan.FromSeconds(0),
			Duration = TimeSpan.FromSeconds(4),
		},
		new()
		{
			StartTime = TimeSpan.FromSeconds(4),
			Duration = TimeSpan.FromSeconds(4),
		},
		new()
		{
			StartTime = TimeSpan.FromSeconds(10),
			Duration = TimeSpan.FromSeconds(10),
		},
	};
}

public class SequenceNode
{
	public TimeSpan StartTime { get; set; }
	public TimeSpan Duration { get; set; }

	public TimeSpan EndTime => this.StartTime + this.Duration;

	public Thickness Margin => new(Sequencer.DurationToPixels(this.StartTime), 0, 0, 0);
	public double Width => Sequencer.DurationToPixels(this.Duration);
}

public class SequenceNodeControl : UserControl
{
	public SequenceNodeControl()
	{
	}
}