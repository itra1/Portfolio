#if UNITY_IOS
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;
using Apple.GameKit;

namespace Platforms.GameCenter.Adapters
{
	public class TDAppleAuthAdapter : ITDAuthAdapter
	{
		public const string LogKey = "[TDAppleAuthAdapter]";
		public UnityAction<bool> OnAuthResult { get; set; }

		private string _signature;
		private string _teamPlayerID;
		private string _salt;
		private string _publicKeyUrl;
		private ulong _timestamp;

		public bool IsSigned { get; private set; }
		public bool IsAuthorized { get; private set; }

		public TDAppleAuthAdapter()
		{
			Debug.Log($"{TDAppleAuthAdapter.LogKey} create adapter");
			_ = LoginAsync();
		}

		private async Task LoginAsync()
		{
			try{
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Start login");

			if (GKLocalPlayer.Local.IsAuthenticated)
			{
				Debug.Log($"{TDAppleAuthAdapter.LogKey} player already logged in.");
				return;
			}

			// Perform the authentication.
			var player = await GKLocalPlayer.Authenticate();
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Authentication: player {player}");

			// Grab the display name.
			var localPlayer = GKLocalPlayer.Local;
			Debug.Log($"Local Player: {localPlayer.DisplayName}");

			// Fetch the items.
			var fetchItemsResponse = await GKLocalPlayer.Local.FetchItems();

			_signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
			_teamPlayerID = localPlayer.TeamPlayerId;
			Debug.Log($"Team Player ID: {_teamPlayerID}");

			_salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
			_publicKeyUrl = fetchItemsResponse.PublicKeyUrl;
			_timestamp = fetchItemsResponse.Timestamp;

			Debug.Log($"{TDAppleAuthAdapter.LogKey} Authentication: signature => {_signature}");
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Authentication: publickeyurl => {_publicKeyUrl}");
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Authentication: salt => {_salt}");
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Authentication: Timestamp => {_timestamp}");
			}catch(System.Exception ex){
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Error auth {ex.Message}");
			}
		}

		public string GetUserId()
			=> AuthenticationService.Instance.PlayerId;

		public string GetUserDisplayName()
			=> AuthenticationService.Instance.PlayerName;

		public void Activate()
		{
			SignInOptions options = new() { CreateAccount = false };

			_ = AuthenticationService.Instance.SignInAnonymouslyAsync(options);
		}

		public void Authenticate(UnityAction<string> callback)
		{
			Debug.Log($"{TDAppleAuthAdapter.LogKey} Authenticate");
			_ = AuthenticateAsync(callback);
		}

		public void ManuallyAuthenticate(UnityAction<string> callback)
		{
			Debug.Log($"{TDAppleAuthAdapter.LogKey} ManuallyAuthenticate");
			_ = ManuallyAuthenticateAsync(callback);
		}

		private async Task AuthenticateAsync(UnityAction<string> callback)
		{
			await AuthenticationService.Instance.SignInWithAppleGameCenterAsync(_signature, _teamPlayerID, _publicKeyUrl, _salt, _timestamp);
			callback?.Invoke(AuthenticationService.Instance.AccessToken);
		}

		private async Task ManuallyAuthenticateAsync(UnityAction<string> callback)
		{
			await AuthenticationService.Instance.LinkWithAppleGameCenterAsync(_signature, _teamPlayerID, _publicKeyUrl, _salt, _timestamp);
			callback?.Invoke(AuthenticationService.Instance.AccessToken);
		}
	}
}
#endif
