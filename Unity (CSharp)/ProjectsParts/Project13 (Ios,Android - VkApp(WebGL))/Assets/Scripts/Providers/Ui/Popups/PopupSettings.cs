using Game.Providers.Ui.Popups.Base;
using UnityEngine;

namespace Game.Providers.Ui.Popups {
	[System.Serializable]
	public class PopupSettings :IPopupSettings {
		[SerializeField] private string _popupsFolder = "Prefabs/UI/Popups";
		[SerializeField] private AnimationCurve _popapCurveAnimation;
		public string PopupsFolder => _popupsFolder;
		public AnimationCurve PopupCurveAnimation => _popapCurveAnimation;
	}
}
