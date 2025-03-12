namespace WpfUtils.Markup;

using DependencyPropertyGenerator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

[ContentProperty("Entries")]
public partial class TemplateSelector : DataTemplateSelector
{
	public List<TemplateSelectorEntry> Entries { get; set; } = new();

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (container is FrameworkElement element && item != null)
		{
			foreach (TemplateSelectorEntry entry in this.Entries)
			{
				if (entry.TargetType == item.GetType())
				{
					return entry.Template;
				}
			}

			foreach (TemplateSelectorEntry entry in this.Entries)
			{
				if (entry.TargetType == null)
					continue;

				if (item.GetType().IsSubclassOf(entry.TargetType))
				{
					return entry.Template;
				}
			}
		}

		return new DataTemplate();
	}
}

[DependencyProperty<Type>("TargetType")]
[DependencyProperty<DataTemplate>("Template")]
public partial class TemplateSelectorEntry : DependencyObject
{
}

[ContentProperty("Entries")]
public partial class InlineTemplateSelector : DataTemplateSelector
{
	public List<DataTemplate> Entries { get; set; } = new();

	public override DataTemplate? SelectTemplate(object item, DependencyObject container)
	{
		if (container is FrameworkElement element && item != null)
		{
			foreach (DataTemplate entry in this.Entries)
			{
				if ((Type)entry.DataType == item.GetType())
				{
					return entry;
				}
			}

			foreach (DataTemplate entry in this.Entries)
			{
				if (item.GetType().IsSubclassOf((Type)entry.DataType))
				{
					return entry;
				}
			}
		}

		return new DataTemplate();
	}
}