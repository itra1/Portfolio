using Cysharp.Threading.Tasks;

using Providers.Biometric;
using Providers.Network.Common;
using Providers.Network.Materials;

using UGui.Screens;
using UGui.Screens.Base;
using UGui.Screens.Elements;

namespace Providers.User
{
	public partial class UserProvider : IUserProvider, IUserAuth
	{

		private readonly IScreenProvider _screenProvider;
		private readonly INetworkApi _api;
		private readonly IBiometricProvider _biometric;
		private UserData _user;

		public bool IsAuthorized { get; set; } = false;
		public string Token => _user == null ? null : _user.token.access_token;

		public UserProvider(IScreenProvider screenProvider, INetworkApi api, IBiometricProvider biometric)
		{
			_api = api;
			_biometric = biometric;
			_screenProvider = screenProvider;
			Load();
		}

		public void SetUserData(UserData userData)
		{
			_user = userData;
			IsAuthorized = true;
		}

		private async UniTask<bool> Authorization()
		{
		// Биометрическая авторизация
			if (_biometric.SuccessAuth && _biometric.CanAuthenticate)
			{
				bool readyAuth = await _biometric.Authenticate();
				if (!readyAuth)
					return false;
			}

			(bool result, object data) = await AuthorizationRequest(_save.AuthLogin, _save.AuthPassword);

			if (!result)
				return false;

			UserData userData = (UserData)data;
			SetUserData(userData);
			return true;
		}

		public async UniTask Initialize()
		{
			if (_save != null && _save.AuthLogin != null)
			{
				await Authorization();

				if (IsAuthorized) return;
			}

		}

		public async UniTask RunUI()
		{
			if (IsAuthorized) return;

			var authorizationScreen = (AuthorizationScreen)_screenProvider.OpenWindow(ScreenTypes.Authorization);

			await UniTask.WaitUntil(() => IsAuthorized);
		}
	}
}
