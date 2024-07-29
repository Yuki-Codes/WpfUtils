namespace WpfUtils.Controls;

using System;
using System.Windows;
using System.Windows.Controls;
using DependencyPropertyGenerator;

[DependencyProperty<string>("Label")]
[DependencyProperty<string>("Flag")]
[DependencyProperty<Enum>("Value")]
public partial class FlagCheckbox : UserControl
{
	public FlagCheckbox()
	{
		this.InitializeComponent();
		this.Checkbox.DataContext = this;
	}

	partial void OnValueChanged(Enum? newValue)
	{
		if (newValue == null || this.Flag == null || this.Value == null)
			return;

		Type enumType = this.Value.GetType();
		Enum flagValue = (Enum)Enum.Parse(enumType, this.Flag);

		this.Checkbox.IsChecked = newValue.HasFlag(flagValue);
	}

	private void OnChecked(object sender, RoutedEventArgs e)
	{
		if (this.Value == null || this.Flag == null)
			return;

		Type enumType = this.Value.GetType();
		Enum value = (Enum)Enum.Parse(enumType, this.Flag);

		if (Enum.GetUnderlyingType(enumType) != typeof(ulong))
		{
			this.Value = (Enum)Enum.ToObject(enumType, Convert.ToInt64(this.Value) | Convert.ToInt64(value));
		}
		else
		{
			this.Value = (Enum)Enum.ToObject(enumType, Convert.ToUInt64(this.Value) | Convert.ToUInt64(value));
		}
	}

	private void OnUnchecked(object sender, RoutedEventArgs e)
	{
		if (this.Value == null || this.Flag == null)
			return;

		Type enumType = this.Value.GetType();
		Enum value = (Enum)Enum.Parse(enumType, this.Flag);

		if (Enum.GetUnderlyingType(enumType) != typeof(ulong))
		{
			this.Value = (Enum)Enum.ToObject(enumType, Convert.ToInt64(this.Value) & ~Convert.ToInt64(value));
		}
		else
		{
			this.Value = (Enum)Enum.ToObject(enumType, Convert.ToUInt64(this.Value) & ~Convert.ToUInt64(value));
		}
	}
}
