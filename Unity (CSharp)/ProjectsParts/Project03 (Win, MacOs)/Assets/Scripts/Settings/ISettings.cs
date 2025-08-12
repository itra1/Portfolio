using Providers.Network.Common;
using Providers.SystemMessage.Common;

using Settings.Themes;
using UGui.Screens.Base;

namespace Settings
{
	public interface ISettings: IThemeSettings, IScreensSettings, INetworkSettings, ISystemMessageSettings
	{
	
	}
}
