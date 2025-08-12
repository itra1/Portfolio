using Platforms.GameCenter.Adapters;
using UnityEngine.Events;

namespace Platforms.GameCenter
{
	public class TDGameCenterProvider
	{
		public const string LogKey = "[GameCenterProvider]";

		private ITDAuthAdapter _adapter;

		public TDGameCenterProvider()
		{
			_adapter = GetProvider();
		}

		private ITDAuthAdapter GetProvider()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			return new TDGoogleAuthAdapter();
#elif UNITY_IOS && !UNITY_EDITOR
			return new TDAppleAuthAdapter();
#elif UNITY_EDITOR
			return new TDEditorAuthAdapter();
#else
			throw new System.NotImplementedException($"{TDGameCenterProvider.LogKey} The provider for the current platform has not been implemented");
#endif
		}

		public void Activate()
		{
			_adapter.Activate();
		}

		public void ManuallyAuthenticate(UnityAction<string> callback)
		{
			_adapter.ManuallyAuthenticate(callback);
		}

		public void Authenticate(UnityAction<string> callback)
		{
			_adapter.Authenticate(callback);
		}
		public string GetUserId()
		{
			return _adapter.GetUserId();
		}
		public string GetUserDisplayName()
		{
			return _adapter.GetUserDisplayName();
		}
	}
}
