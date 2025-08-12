
using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Popups
{
	public class PopupProvider : IPopupProvider
	{
		private PopupFactory _popupFactory;
		private SignalBus _signalBus;
		private IPopupsParent _popupParent;

		public PopupProvider(
		SignalBus signalBus
		, PopupFactory popupFactory
		, IPopupsParent popupParent)
		{
			_signalBus = signalBus;
			_popupFactory = popupFactory;
			_popupParent = popupParent;
		}

		public Popup OpenPopup(string name, bool avtoShow = true)
		{
			var popup = _popupFactory.GetInstance(name, _popupParent.PopupParent);

			RectTransform rt = popup.GetComponent<RectTransform>();
			rt.FullRect();
			rt.SetAsLastSibling();
			if (avtoShow)
				popup.Show();
			return popup;
		}

	}
}
