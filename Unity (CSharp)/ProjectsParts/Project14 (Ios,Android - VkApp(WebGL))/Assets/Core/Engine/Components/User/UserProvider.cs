using Core.Engine.Components.SaveGame;
using Core.Engine.Helpers;
using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;

using Cysharp.Threading.Tasks;

namespace Core.Engine.Components.User {
	/// <summary>
	/// Пользователь
	/// </summary>
	public class UserProvider : IUserProvider {

		private IPopupProvider _popupProvider;
		private IScreensProvider _screensProvider;
		private UserProviderSave _userProvider;

		public bool IsInitiated { get; set; } = false;
		public string UserName => _userProvider.Value.Name;
		public bool IsExistsName => !string.IsNullOrEmpty(_userProvider.Value.Name);
		public int AgeValue => _userProvider.Value.Age;
		public ulong PointsCount => _userProvider.Value.Points;

		public string AvatarName => _userProvider.Value.Avatar;

		public UserProvider(SaveGameProvider saveGameProvider) {
			_userProvider = (UserProviderSave)saveGameProvider.GetProperty<UserProviderSave>();
		}

		public void InitComponents(IPopupProvider popupProvider, IScreensProvider screensProvider) {
			_popupProvider = popupProvider;
			_screensProvider = screensProvider;
		}

		public void Initiate() {
			InitiateAsync();
		}

		private async void InitiateAsync() {
			await UniTask.Delay(1000);
			if (!IsExistsName) {
				//await ShowPopupEnterName();
				_ = EnterName("Player");
				await ShowPopupSelectAvatar();
			}
			await ShowPopupWelcome();
			IsInitiated = true;
		}

		public bool EnterName(string name) {
			int result = InputValidate.UserName(name);

			if (result == InputValidate.ErrorLenght) {
				ShowPopupInfo(StringConstants.ErronIncorrectLenght);
				return false;
			}
			if (result == InputValidate.ErrorFormat) {
				ShowPopupInfo(StringConstants.ErronIncorrectData);
				return false;
			}

			_userProvider.Value.Name = name;
			_userProvider.Save();

			return true;
		}

		private void ShowPopupInfo(string text) {
			var popup = (InfoPopup)_popupProvider.OpenPopup(PopupTypes.Info);
			popup.SetInfo(text);
		}

		private async UniTask ShowPopupEnterName() {
			var popup = (UserNamePopup)_popupProvider.OpenPopup(PopupTypes.UserName);

			bool isClose = false;
			_ = popup.OnHideComplete(() => {
				isClose = true;
			});

			await UniTask.WaitUntil(() => isClose);
		}

		private async UniTask ShowPopupSelectAvatar() {

			var screen = (AvatarsScreen)_screensProvider.OpenWindow(ScreenTypes.Avatar);

			bool isClose = false;
			_ = screen.OnHide(() => {
				isClose = true;
			});

			await UniTask.WaitUntil(() => isClose);
		}

		private async UniTask ShowPopupWelcome() {
			var popup = (WelcomePopup)_popupProvider.OpenPopup(PopupTypes.WelcomeUser);

			bool isClose = false;
			_ = popup.OnHideComplete(() => {
				isClose = true;
			});

			await UniTask.WaitUntil(() => isClose);
		}

		public void PointsAdd(ulong points) {
			_userProvider.Value.Points += points;
		}

		public void PointsSubtract(ulong points) {
			_userProvider.Value.Points -= points;
		}

		public void SetAvatarName(string nameAvatar) {
			_userProvider.Value.Avatar = nameAvatar;
			_userProvider.Save();
		}
	}

}