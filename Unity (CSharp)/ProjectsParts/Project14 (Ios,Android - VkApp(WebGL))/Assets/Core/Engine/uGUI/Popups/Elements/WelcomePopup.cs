using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.User;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Zenject;

namespace Core.Engine.uGUI.Popups {
	[PrefabName(PopupTypes.WelcomeUser)]
	public class WelcomePopup : Popup {
		[SerializeField] private TMPro.TMP_Text _userNameLabel;

		private IUserProvider _userProvider;

		[Inject]
		public void Initialize(IUserProvider userProvider) {
			_userProvider = userProvider;
		}

		protected override void BeforeShow() {
			base.BeforeShow();
			_userNameLabel.text = _userProvider.UserName;
		}

		protected override void AfterShow() {
			base.AfterShow();
			DelayVisible().Forget();
		}

		private async UniTaskVoid DelayVisible() {

			await UniTask.Delay(1000);
			_ = Hide();
		}

	}
}