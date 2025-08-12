using UnityEngine.Events;

namespace Platforms.GameCenter.Adapters
{
	public interface ITDAuthAdapter
	{
		UnityAction<bool> OnAuthResult { get; set; }

		bool IsAuthorized { get; }
		bool IsSigned { get; }

		string GetUserId();
		string GetUserDisplayName();

		void Activate();
		void Authenticate(UnityAction<string> callback);
		void ManuallyAuthenticate(UnityAction<string> callback);
	}
}
