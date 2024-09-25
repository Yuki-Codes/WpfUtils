namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUtils;

[DependencyProperty<double>("X", DefaultBindingMode = DefaultBindingMode.TwoWay)]
[DependencyProperty<double>("Y", DefaultBindingMode = DefaultBindingMode.TwoWay)]
[DependencyProperty<double>("Z", DefaultBindingMode = DefaultBindingMode.TwoWay)]
[DependencyProperty<double>("TickFrequency", DefaultValue = 1)]
[DependencyProperty<double>("Minimum", DefaultValue = double.MinValue)]
[DependencyProperty<double>("Maximum", DefaultValue = double.MaxValue)]
[DependencyProperty<bool>("Wrap")]
[DependencyProperty<object>("Prefix")]
[DependencyProperty<int>("DecimalPlaces", DefaultValue = 3)]
public partial class MultiNumberBox : TextBox
{
	private Key keyHeld = Key.None;
	private string? currentEditString = null;
	private bool isPropagatingValueChange = false;

	public MultiNumberBox()
	{
		this.Text = this.Display;
		this.TextChanged += this.OnTextChanged;
	}

	public string Display
	{
		get
		{
			if (this.currentEditString != null)
				return this.currentEditString;

			string xStr = Math.Round(this.X, this.DecimalPlaces).ToString().Replace("-0", "0");
			string yStr = Math.Round(this.Y, this.DecimalPlaces).ToString().Replace("-0", "0");
			string zStr = Math.Round(this.Z, this.DecimalPlaces).ToString().Replace("-0", "0");

			return $"{xStr}, {yStr}, {zStr}";
		}

		set
		{
			string[] parts = value.Split(',');
			if (parts.Length == 3
				&& double.TryParse(parts[0], out var x)
				&& double.TryParse(parts[1], out var y)
				&& double.TryParse(parts[2], out var z))
			{
				this.currentEditString = null;
				this.X = x;
				this.Y = y;
				this.Z = z;
			}
			else
			{
				this.currentEditString = value;
			}
		}
	}

	protected override void OnPreviewKeyDown(KeyEventArgs e)
	{
		if (!this.IsKeyboardFocused)
			return;

		if (e.Key == Key.Up || e.Key == Key.Down)
		{
			e.Handled = true;

			if (e.IsRepeat)
			{
				if (this.keyHeld == e.Key)
					return;

				this.keyHeld = e.Key;
				Task.Run(this.TickHeldKey);
			}
			else
			{
				this.TickKey(e.Key);
			}
		}
	}

	protected override void OnPreviewKeyUp(KeyEventArgs e)
	{
		if (this.keyHeld == e.Key)
		{
			e.Handled = true;
			this.keyHeld = Key.None;
		}
	}

	protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
	{
		if (!this.IsFocused)
			return;

		e.Handled = true;
		this.TickValue(e.Delta > 0);
	}

	partial void OnXChanged()
	{
		this.OnValueChanged();
	}

	partial void OnYChanged()
	{
		this.OnValueChanged();
	}

	partial void OnZChanged()
	{
		this.OnValueChanged();
	}

	private void OnValueChanged()
	{
		this.isPropagatingValueChange = true;

		int caretIndex = this.CaretIndex;
		this.Text = this.Display;
		this.CaretIndex = caretIndex;

		this.isPropagatingValueChange = false;
	}

	private void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		if (this.isPropagatingValueChange)
			return;

		this.Display = this.Text;
	}

	private async Task TickHeldKey()
	{
		while (this.keyHeld != Key.None)
		{
			await this.Dispatcher.MainThread();
			this.TickKey(this.keyHeld);
			await Task.Delay(10);
		}
	}

	private void TickKey(Key key)
	{
		if (key == Key.Up)
		{
			this.TickValue(true);
		}
		else if (key == Key.Down)
		{
			this.TickValue(false);
		}
	}

	private double Validate(double v)
	{
		if (this.Wrap)
		{
			if (v > this.Maximum)
			{
				v = this.Minimum;
			}

			if (v < this.Minimum)
			{
				v = this.Maximum;
			}
		}
		else
		{
			v = Math.Min(v, this.Maximum);
			v = Math.Max(v, this.Minimum);
		}

		return v;
	}

	private void TickValue(bool increase)
	{
		double delta = increase ? this.TickFrequency : -this.TickFrequency;

		if (Keyboard.IsKeyDown(Key.LeftShift))
			delta *= 10;

		if (Keyboard.IsKeyDown(Key.LeftCtrl))
			delta /= 10;

		// Find which number block the caret is in
		int caretIndex = this.CaretIndex;
		string str = this.Display;
		int boxNum = 0;
		for (int i = 0; i < caretIndex; i++)
		{
			if (str[i] == ',')
			{
				boxNum++;
			}
		}

		double value = -1;
		if (boxNum == 0)
		{
			value = this.X;
		}
		else if (boxNum == 1)
		{
			value = this.Y;
		}
		else if (boxNum == 2)
		{
			value = this.Z;
		}

		double newValue = value + delta;
		newValue = this.Validate(newValue);

		if (newValue == value)
			return;

		if (boxNum == 0)
		{
			this.X = newValue;
		}
		else if (boxNum == 1)
		{
			this.Y = newValue;
		}
		else if (boxNum == 2)
		{
			this.Z = newValue;
		}

		// restore the caret index as changing the display may reset it.
		this.CaretIndex = caretIndex;
	}
}
