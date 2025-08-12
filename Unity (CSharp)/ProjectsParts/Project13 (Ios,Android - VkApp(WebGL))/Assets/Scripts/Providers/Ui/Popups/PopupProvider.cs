using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Factorys;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Popups {
	public class PopupProvider :IPopupsProvider {
		private PopupFactory _popupFactory;
		private SignalBus _signalBus;
		private IPopupsParent _popupParent;

		public PopupProvider(
		SignalBus signalBus
		, PopupFactory popupFactory
		, IPopupsParent popupParent) {
			_signalBus = signalBus;
			_popupFactory = popupFactory;
			_popupParent = popupParent;
		}

		public Popup GetPopup(string name) {
			var popup = _popupFactory.GetInstance(name, _popupParent.PopupsParent);

			var rt = popup.GetComponent<RectTransform>();
			rt.FullRect();
			rt.SetAsLastSibling();
			return popup;
		}
	}
}
