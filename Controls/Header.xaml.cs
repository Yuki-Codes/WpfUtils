namespace WpfUtils.Controls;

using DependencyPropertyGenerator;
using FontAwesome.Sharp.Pro;
using System.Windows.Controls;

[DependencyProperty<ProIcons>("Icon")]
[DependencyProperty<string>("Text")]
public partial class Header : UserControl
{
	public Header()
	{
		this.InitializeComponent();
		this.ContentArea.DataContext = this;
	}
}
