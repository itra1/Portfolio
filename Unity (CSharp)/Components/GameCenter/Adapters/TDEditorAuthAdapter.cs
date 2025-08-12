using Platforms.GameCenter.Common;
using UnityEngine.Events;

namespace Platforms.GameCenter.Adapters
{
	public class TDEditorAuthAdapter : ITDAuthAdapter
	{
		public UnityAction<bool> OnAuthResult { get; set; }

		public bool IsSigned { get; private set; }

		public bool IsAuthorized { get; private set; } = true;

		public void Activate() { }

		public void Authenticate(UnityAction<string> callback)
		{
			callback?.Invoke(TDSignInStatus.Success);
		}

		public string GetUserDisplayName()
			=> "Editor user";

		public string GetUserId()
			=> "000";

		public void ManuallyAuthenticate(UnityAction<string> callback)
		{
			callback.Invoke(TDSignInStatus.Success);
		}
	}
}
