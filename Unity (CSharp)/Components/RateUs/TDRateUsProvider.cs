using Platforms.RateUs.Adapters;
using UnityEngine.Events;

namespace Platforms.RateUs
{
	public class TDRateUsProvider
	{
		public const string LogKey = "[RateUsProvider]";

		private ITDRateUsAdapter _adapter;

		public TDRateUsProvider()
		{
			CreateAdapter();
		}

		private void CreateAdapter()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			_adapter = new TDGoogleAdapter();
#elif UNITY_IOS && !UNITY_EDITOR
			_adapter = new TDAppleAdapter();
#elif UNITY_EDITOR
			_adapter = new TDEditorAdapter();
#else
			throw System.NotImplementedException($"{LogKey} no exists adapter");
#endif
		}

		public bool ReadyShow => _adapter.ReadyShow;

		public void RateUs(UnityAction<bool> completeCallback)
		{
			try
			{
				_adapter.RateUs(completeCallback);
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError($"{TDRateUsProvider.LogKey} Error proocess {ex.Message}");
				completeCallback?.Invoke(false);
			}
		}
	}
}
