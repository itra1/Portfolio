#if FACEBOOK_SERVICE
using Facebook.Unity;
#endif
using UnityEngine;

namespace Core.Engine.Services.FBService
{
#if FACEBOOK_SERVICE
	public class FacebookLogin : IFacebookLogin
	{
		public bool IsLogin => !string.IsNullOrEmpty(_token);

		private string _token;
		private IFacebookPermissions _permissions;

		public FacebookLogin(IFacebookPermissions permissions)
		{
			_permissions = permissions;
		}


		public void Login()
		{
			if (IsLogin) return;

			FbLogin();
		}

		public void FbLogin()
		{
			FB.LogInWithReadPermissions(_permissions.Permissions, AuthCallback);
		}

		private void AuthCallback(ILoginResult result)
		{
			if (FB.IsLoggedIn)
			{
				var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				Debug.Log(aToken.UserId);
				foreach (string perm in aToken.Permissions)
				{
					Debug.Log(perm);
				}
			}
			else
			{
				Debug.Log("User cancelled login");
			}
		}

	}
#endif
}