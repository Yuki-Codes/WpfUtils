﻿namespace System.Windows.Controls;

using System.Windows;

public static class UserControlExtensions
{
	public static T GetValue<T>(this UserControl self, DependencyProperty dp)
	{
		return (T)self.GetValue(dp);
	}
}
