namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using FontAwesome.Sharp;
using System.Windows.Controls;

[DependencyProperty<IconChar>("Icon")]
[DependencyProperty<string>("Text")]
public partial class Header : UserControl
{
	public Header()
	{
		this.InitializeComponent();
		this.ContentArea.DataContext = this;
	}
}
