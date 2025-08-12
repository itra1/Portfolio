using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Popups.Elements {
	[PrefabName(PopupsNames.AddCoins)]
	internal class AddCoinsPopup :Popup {
		[SerializeField] private Button _closeButton;

		protected override void Awake() {
			base.Awake();

			_closeButton.onClick.AddListener(() => { Hide().Forget(); });

		}
	}
}
