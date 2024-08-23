namespace WpfUtils.MarkupExtensions;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

public class RoutedEventExtension(string path)
	: MarkupExtension
{
	public override object? ProvideValue(IServiceProvider serviceProvider)
	{
		int separatorIndex = path.LastIndexOf('.');
		if (separatorIndex == -1)
			throw new NotSupportedException("Invalid routed event path");

		string typeName = path.Substring(0, separatorIndex);
		string name = path.Substring(separatorIndex + 1);

		IXamlTypeResolver? xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
		if (xamlTypeResolver == null)
			throw new InvalidOperationException("Missing Xaml Type Resolver");

		Type? type = xamlTypeResolver.Resolve(typeName);
		if (type == null)
			throw new InvalidOperationException($"Unable to locate type: '{typeName}'");

		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
		foreach (FieldInfo field in fields)
		{
			if (field.FieldType == typeof(RoutedEvent))
			{
				RoutedEvent? evt = field.GetValue(null) as RoutedEvent;
				if (evt == null)
					continue;

				if (evt.Name == name)
				{
					return evt;
				}
			}
		}

		throw new InvalidOperationException($"Unable to locate event: '{name}'");
	}
}