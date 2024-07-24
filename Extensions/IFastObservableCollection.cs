namespace WpfUtils.Extensions;

using System.Collections;

public interface IFastObservableCollection
{
	public void Replace(IEnumerable other);
}