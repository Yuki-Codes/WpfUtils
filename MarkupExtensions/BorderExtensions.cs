namespace WpfUtils.MarkupExtensions;

using DependencyPropertyGenerator;
using System.Windows;
using System.Windows.Controls;

public enum Edges
{
	Top,
	Bottom,
	Left,
	Right,
}

[AttachedDependencyProperty<Edges>("FlattenedCorners")]
public partial class BorderExtensions
{
	static partial void OnFlattenedCornersChanged(DependencyObject dependencyObject, Edges newValue)
	{
		if (dependencyObject is Border border)
		{
			CornerRadius radius = border.CornerRadius;

			switch (newValue)
			{
				case Edges.Top:
				{
					radius.TopLeft = 0;
					radius.TopRight = 0;
					break;
				}

				case Edges.Bottom:
				{
					radius.BottomLeft = 0;
					radius.BottomRight = 0;
					break;
				}

				case Edges.Left:
				{
					radius.TopLeft = 0;
					radius.BottomLeft = 0;
					break;
				}

				case Edges.Right:
				{
					radius.TopRight = 0;
					radius.BottomRight = 0;
					break;
				}
			}

			border.CornerRadius = radius;
		}
	}
}