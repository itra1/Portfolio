#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Platforms.GameCenter.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Platforms.GameCenter.Adapters
{
	public class TDGoogleAuthAdapter : ITDAuthAdapter
	{
		public UnityAction<bool> OnAuthResult { get; set; }

		private string _token;
		private string _error;

		public bool IsAuthorized { get; private set; }
		public bool IsSigned { get; private set; }

		public TDGoogleAuthAdapter()
		{
			//_ = PlayGamesPlatform.Activate();
			//Login();
		}

		public string GetUserId()
			=> PlayGamesPlatform.Instance.GetUserId();

		public string GetUserDisplayName()
			=> PlayGamesPlatform.Instance.GetUserDisplayName();

		public void Activate()
		{
			_ = PlayGamesPlatform.Activate();
		}

		public void Authenticate(UnityAction<string> callback)
		{
			PlayGamesPlatform.Instance.Authenticate(status =>
			{

				if (status != SignInStatus.Success)
				{
					_error = "Failed to retrieve Google play games authorization code";
					Debug.Log("Login Unsuccessful");
					callback.Invoke(OverrideStatus(status));
				}

				Debug.Log("Login with Google Play games successful.");

				PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
				{
					Debug.Log("Authorization code: " + code);
					_token = code;
					// This token serves as an example to be used for SignInWithGooglePlayGames
					callback.Invoke(OverrideStatus(SignInStatus.Success));
				});

			});
		}

		public void ManuallyAuthenticate(UnityAction<string> callback)
		{
			PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
			{
				callback?.Invoke(OverrideStatus(status));
			});
		}

		private string OverrideStatus(SignInStatus status)
			=> status switch
			{
				SignInStatus.Canceled => TDSignInStatus.Canceled,
				SignInStatus.InternalError => TDSignInStatus.InternalError,
				_ => TDSignInStatus.Success
			};
	}
}
#endif