using Game.Providers.Ui.Popups.Base;

namespace Game.Providers.Ui.Popups {
	public interface IPopupsProvider {
		Popup GetPopup(string name);
	}
}
