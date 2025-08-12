using Providers.Network.Materials;
using Providers.User;

using UGui.Screens.Common;
using UGui.Screens.Elements;

using UnityEngine;

using Zenject;

namespace UGui.Screens.Pages
{
	public class RegistrationCompletePage : MonoBehaviour, IZInjection
	{
		private UserData _userData;
		private IUserProvider _userProvider;

		[Inject]
		private void Initialize(IUserProvider userProvider)
		{
			_userProvider = userProvider;
		}
		public void SetData(UserData userData)
		{
			_userData = userData;
		}
		public void OnButtonTouch()
		{
			_userProvider.SetUserData(_userData);
			GetComponentInParent<Screens.Base.Screen>().Hide();
		}
	}
}
