namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUtils.Logging;

using DrawPoint = System.Drawing.Point;
using WinCur = System.Windows.Forms.Cursor;
using WinPoint = System.Windows.Point;

[DependencyProperty<double>("Value", DefaultValue=0, DefaultBindingMode = DefaultBindingMode.TwoWay)]
[DependencyProperty<double>("TickFrequency", DefaultValue = 1)]
[DependencyProperty<SliderModes>("Slider", DefaultValue = SliderModes.None)]
[DependencyProperty<bool>("Buttons", DefaultValue = false)]
[DependencyProperty<double>("Minimum", DefaultValue = double.MinValue)]
[DependencyProperty<double>("Maximum", DefaultValue = double.MaxValue)]
[DependencyProperty<bool>("Wrap", DefaultValue = false)]
[DependencyProperty<double>("ValueOffset", DefaultValue = 0)]
[DependencyProperty<bool>("UncapTextInput", DefaultValue = false)]
[DependencyProperty<object>("Prefix")]
[DependencyProperty<object>("Suffix")]
public partial class NumberBox : UserControl, INotifyPropertyChanged
{
	private string? inputString = "0";
	private Key keyHeld = Key.None;
	private double relativeSliderStart;
	private double relativeSliderCurrent;

	public NumberBox()
	{
		this.InitializeComponent();
		this.ContentArea.DataContext = this;
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public enum SliderModes
	{
		None,
		Absolute,
		Relative,
	}

	public double DisplayValue
	{
		get
		{
			return this.Value + this.ValueOffset;
		}

		set
		{
			double newVal = value - this.ValueOffset;

			if (!this.UncapTextInput)
			{
				if (this.Wrap)
				{
					double range = this.Maximum - this.Minimum;

					if (newVal > this.Maximum)
						newVal = this.Minimum + ((newVal - this.Maximum) % range);

					if (newVal < this.Minimum)
						newVal = this.Maximum - ((this.Maximum - newVal) % range);
				}

				newVal = Math.Max(this.Minimum, newVal);
				newVal = Math.Min(this.Maximum, newVal);
			}

			this.Value = newVal;
			this.PropertyChanged?.Invoke(this, new(nameof(NumberBox.DisplayValue)));
		}
	}

	public string? Text
	{
		get
		{
			return this.inputString;
		}

		set
		{
			if (value == this.inputString)
				return;

			this.inputString = value;

			double val;
			if (double.TryParse(value, out val))
			{
				this.DisplayValue = val;
				this.ErrorDisplay.Visibility = Visibility.Collapsed;
			}
			else
			{
				try
				{
					val = Convert.ToDouble(new DataTable().Compute(value, null));
					this.DisplayValue = val;
					this.ErrorDisplay.Visibility = Visibility.Collapsed;
				}
				catch (Exception)
				{
					this.ErrorDisplay.Visibility = Visibility.Visible;
				}
			}

			this.PropertyChanged?.Invoke(this, new(nameof(NumberBox.Text)));
		}
	}

	public double SliderValue
	{
		get
		{
			if (this.Slider == SliderModes.Absolute)
			{
				return this.DisplayValue;
			}
			else
			{
				return this.relativeSliderCurrent;
			}
		}
		set
		{
			if (this.Slider == SliderModes.Absolute)
			{
				this.DisplayValue = value;
			}
			else
			{
				this.relativeSliderCurrent = value;

				if (Keyboard.IsKeyDown(Key.LeftShift))
					value *= 10;

				if (Keyboard.IsKeyDown(Key.LeftCtrl))
					value /= 10;

				this.DisplayValue = this.relativeSliderStart + value;
			}

			this.PropertyChanged?.Invoke(this, new(nameof(NumberBox.SliderValue)));
		}
	}

	public double SliderMinimum
	{
		get
		{
			if (this.Slider == SliderModes.Absolute)
			{
				return this.Minimum;
			}
			else
			{
				return -(this.TickFrequency * 30);
			}
		}
	}

	public double SliderMaximum
	{
		get
		{
			if (this.Slider == SliderModes.Absolute)
			{
				return this.Maximum;
			}
			else
			{
				return this.TickFrequency * 30;
			}
		}
	}

	protected override void OnPreviewKeyDown(KeyEventArgs e)
	{
		bool focused = this.InputBox.IsKeyboardFocused || this.InputSlider.IsKeyboardFocused;
		if (!focused)
			return;

		if (e.Key == Key.Return)
		{
			this.Commit(true);
			e.Handled = true;
		}

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
		if (!this.InputBox.IsFocused)
			return;

		e.Handled = true;
		this.TickValue(e.Delta > 0);
	}

	partial void OnValueChanged(double newValue)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderValue)));

		int caretIndex = this.InputBox.CaretIndex;
		this.Text = this.DisplayValue.ToString("0.###");
		this.InputBox.CaretIndex = caretIndex;
	}

	partial void OnSliderChanged(SliderModes newValue)
	{
		this.InputSlider.Visibility = newValue != SliderModes.None ? Visibility.Visible : Visibility.Collapsed;

		int inputColumnWidth = 64;
		if (this.Buttons)
			inputColumnWidth += 48;

		this.SliderColumn.Width = newValue != SliderModes.None ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
		this.InputBoxColumn.Width = newValue != SliderModes.None ? new GridLength(inputColumnWidth) : new GridLength(1, GridUnitType.Star);

		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderMaximum)));
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderMinimum)));
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderValue)));
	}

	partial void OnButtonsChanged(bool newValue)
	{
		this.DownButton.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
		this.UpButton.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
	}

	partial void OnMinimumChanged(double newValue)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderMinimum)));
	}

	partial void OnMaximumChanged(double newValue)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderMaximum)));
	}

	partial void OnTickFrequencyChanged(double newValue)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderMaximum)));
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderMinimum)));
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumberBox.SliderValue)));
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		Window window = Window.GetWindow(this);
		if (window != null)
		{
			window.MouseDown += this.OnWindowMouseDown;
			window.Deactivated += this.OnWindowDeactivated;
		}

		this.OnSliderChanged(this.Slider);
		this.OnButtonsChanged(this.Buttons);
		this.OnTickFrequencyChanged(this.TickFrequency);

		////this.Text = this.DisplayValue.ToString("0.###");
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

		////if (this.TickFrequency != 0)
		////	v = Math.Round(v / this.TickFrequency) * this.TickFrequency;

		return v;
	}

	private void OnLostFocus(object sender, RoutedEventArgs e)
	{
		this.Text = this.DisplayValue.ToString("0.###");
		////this.Commit(false);
	}

	private void Commit(bool refocus)
	{
		try
		{
			this.DisplayValue = Convert.ToDouble(new DataTable().Compute(this.inputString, null));
			this.ErrorDisplay.Visibility = Visibility.Collapsed;
		}
		catch (Exception)
		{
			this.ErrorDisplay.Visibility = Visibility.Visible;
		}

		this.Text = this.DisplayValue.ToString("0.###");

		if (refocus)
		{
			this.InputBox.Focus();
			this.InputBox.CaretIndex = int.MaxValue;
		}
	}

	private async Task TickHeldKey()
	{
		while (this.keyHeld != Key.None)
		{
			await Application.Current.Dispatcher.InvokeAsync(() =>
			{
				this.TickKey(this.keyHeld);
			});

			await Task.Delay(10);
		}
	}

	private void TickKey(Key key)
	{
		if (key == Key.Up)
		{
			this.TickValue(true);
			this.Commit(true);
		}
		else if (key == Key.Down)
		{
			this.TickValue(false);
			this.Commit(true);
		}
	}

	private void TickValue(bool increase)
	{
		double delta = increase ? this.TickFrequency : -this.TickFrequency;

		if (Keyboard.IsKeyDown(Key.LeftShift))
			delta *= 10;

		if (Keyboard.IsKeyDown(Key.LeftCtrl))
			delta /= 10;

		double value = this.DisplayValue;
		double newValue = value + delta;
		newValue = this.Validate(newValue);

		if (newValue == value)
			return;

		this.DisplayValue = newValue;
	}

	private void OnDownClick(object sender, RoutedEventArgs e)
	{
		this.TickValue(false);
	}

	private void OnUpClick(object sender, RoutedEventArgs e)
	{
		this.TickValue(true);
	}

	private void OnSliderMouseMove(object sender, MouseEventArgs e)
	{
		if (this.Slider != SliderModes.Absolute)
			return;

		if (e.LeftButton == MouseButtonState.Pressed && this.Wrap)
		{
			WinPoint rightEdge = this.InputSlider.PointToScreen(new WinPoint(this.InputSlider.ActualWidth - 5, this.InputSlider.ActualHeight / 2));
			WinPoint leftEdge = this.InputSlider.PointToScreen(new WinPoint(6, this.InputSlider.ActualHeight / 2));

			if (WinCur.Position.X > rightEdge.X)
			{
				WinCur.Position = new DrawPoint((int)leftEdge.X, (int)leftEdge.Y);
			}

			if (WinCur.Position.X < leftEdge.X)
			{
				WinCur.Position = new DrawPoint((int)rightEdge.X, (int)rightEdge.Y);
			}
		}
	}

	private void OnWindowMouseDown(object? sender, MouseButtonEventArgs e)
	{
		FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
		Keyboard.ClearFocus();
	}

	private void OnWindowDeactivated(object? sender, EventArgs e)
	{
		FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), null);
		Keyboard.ClearFocus();
	}

	private void OnSliderPreviewMouseDown(object? sender, MouseButtonEventArgs e)
	{
		this.relativeSliderStart = this.DisplayValue;
	}

	private void OnSliderPreviewMouseUp(object? sender, MouseButtonEventArgs e)
	{
		if (this.Slider == SliderModes.Relative)
		{
			this.relativeSliderStart = this.DisplayValue;
			this.SliderValue = 0;
		}
	}
}
