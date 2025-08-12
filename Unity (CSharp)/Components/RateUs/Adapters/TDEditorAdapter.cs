using UnityEngine.Events;

namespace Platforms.RateUs.Adapters
{
	public class TDEditorAdapter : ITDRateUsAdapter
	{
		public bool ReadyShow => true;

		public void RateUs(UnityAction<bool> completeCallback)
		{
			completeCallback?.Invoke(true);
		}
	}
}
