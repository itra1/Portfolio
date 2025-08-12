using Core.Engine.Components.Audio;
using Core.Engine.Components.Avatars.Common;
using Core.Engine.Components.Leaderboard;
using Core.Engine.Components.Shop;
using Core.Engine.Components.Skins;
using Core.Engine.Components.Themes.Common;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;

namespace Core.Engine.App.Settings {
	public interface IAppPaths : IPopupSettings, IShopSettings, IScreenSettings, ISkinSettings, ISoundSettings, ILeaderboardSettings, IAvatarsSettings, IThemeSettings {
	}
}