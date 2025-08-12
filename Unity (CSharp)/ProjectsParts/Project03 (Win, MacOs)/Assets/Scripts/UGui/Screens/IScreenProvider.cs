
using UGui.Screens.Base;

namespace UGui.Screens
{
	public interface IScreenProvider
	{
		Screen OpenWindow(string name);
	}
}
