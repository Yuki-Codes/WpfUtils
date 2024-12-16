namespace WpfUtils.Converters;

public class MultiplyConverter : ConverterBase<double, double>
{
	protected override double Convert(double value)
	{
		double p = System.Convert.ToDouble(value);
		return value * p;
	}

	protected override double ConvertBack(double value)
	{
		double p = System.Convert.ToDouble(value);
		return value / p;
	}
}
