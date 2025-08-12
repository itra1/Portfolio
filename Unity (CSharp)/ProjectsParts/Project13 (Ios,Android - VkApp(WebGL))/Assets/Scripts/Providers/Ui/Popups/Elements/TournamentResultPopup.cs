using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;

namespace Game.Providers.Ui.Popups.Elements {
	[PrefabName(PopupsNames.TournamentWin)]
	public class TournamentResultPopup :Popup {
		[SerializeField] private RectTransform _winLogo;
		[SerializeField] private RectTransform _lossLogo;
		[SerializeField] private RectTransform _itemsParent;

		private bool _isWin;

		public void SetIsWin(bool isWin) {
			_isWin = isWin;
			_winLogo.gameObject.SetActive(_isWin);
			_lossLogo.gameObject.SetActive(_isWin);
		}

		public void ContinueButtonTouch() {
			Hide().Forget();
		}
	}
}
