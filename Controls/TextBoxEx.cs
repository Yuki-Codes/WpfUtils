namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using System.Windows;
using System.Windows.Controls;

[DependencyProperty<object>("Suffix")]
[DependencyProperty<object>("SuffixTemplate")]
[DependencyProperty<Thickness>("SuffixPosition")]
public partial class TextBoxEx : TextBox
{
	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		this.UpdateSuffix();
	}

	protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
	{
		base.OnRenderSizeChanged(sizeInfo);
		this.UpdateSuffix();
	}

	protected override void OnTextChanged(TextChangedEventArgs e)
	{
		base.OnTextChanged(e);
		this.UpdateSuffix();
	}

	partial void OnSuffixChanged()
	{
		this.UpdateSuffix();
	}

	private void UpdateSuffix()
	{
		Rect r = this.GetRectFromCharacterIndex(this.Text.Length, true);
		if (r != Rect.Empty)
		{
			Thickness m = this.SuffixPosition;
			m.Left = r.Left;
			m.Top = r.Top;
			this.SuffixPosition = m;
		}
	}
}
