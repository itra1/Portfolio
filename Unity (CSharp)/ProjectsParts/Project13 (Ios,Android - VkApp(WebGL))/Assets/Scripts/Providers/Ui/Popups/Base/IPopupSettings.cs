using UnityEngine;

namespace Game.Providers.Ui.Popups.Base {
	public interface IPopupSettings {
		string PopupsFolder { get; }
		AnimationCurve PopupCurveAnimation { get; }
	}
}
