namespace WpfUtils.Shapes;

using DependencyPropertyGenerator;

[DependencyProperty<double>("Minimum")]
[DependencyProperty<double>("Maximum")]
[DependencyProperty<double>("Value")]
public partial class FillArc : Arc
{
	partial void OnMinimumChanged() => this.UpdateArc();
	partial void OnMaximumChanged() => this.UpdateArc();
	partial void OnValueChanged() => this.UpdateArc();

	private void UpdateArc()
	{
		this.EndAngle = this.StartAngle + (359.999 * (this.Value / (this.Maximum - this.Minimum)));
	}
}
