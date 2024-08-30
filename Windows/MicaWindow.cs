// https://tvc-16.science/mica-wpf.html
namespace WpfUtils.Windows;

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

public class MicaWindow
{
	[Flags]
	private enum DwmWindowAttribute : uint
	{
		DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
		DWMWA_SYSTEMBACKDROP_TYPE = 38,
	}

	public static void ApplyBackdrop(Window window)
	{
		int one = 1;
		int four = 4;
		DwmSetWindowAttribute(new WindowInteropHelper(window).Handle, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref one, Marshal.SizeOf<int>());
		DwmSetWindowAttribute(new WindowInteropHelper(window).Handle, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref four, Marshal.SizeOf<int>());
	}

	[DllImport("dwmapi.dll")]
	private static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);
}
