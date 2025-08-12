using UnityEngine.Events;

namespace Platforms.RateUs.Adapters
{
	public interface ITDRateUsAdapter
	{
		bool ReadyShow { get; }

		void RateUs(UnityAction<bool> completeCallback);
	}
}
