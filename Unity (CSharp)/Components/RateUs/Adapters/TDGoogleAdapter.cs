#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
using UnityEngine;
using UnityEngine.Events;

namespace Platforms.RateUs.Adapters
{
	public class TDGoogleAdapter : ITDRateUsAdapter
	{
		private readonly ReviewManager _reviewManager;
		private PlayReviewInfo _playReviewInfo;

		public bool ReadyShow => _playReviewInfo != null;

		public TDGoogleAdapter()
		{
			Debug.LogWarning($"{TDRateUsProvider.LogKey} TDGoogleAdapter create");
			_reviewManager ??= new();
			LoadData();
		}

		private void LoadData()
		{
			var requestFlowOperation = _reviewManager.RequestReviewFlow();
			requestFlowOperation.Completed += (request) =>
			{
				if (request.Error != ReviewErrorCode.NoError)
				{
					Debug.LogError($"{TDRateUsProvider.LogKey} Error in loading review: {request.Error}");
					return;
				}
				Debug.LogWarning($"{TDRateUsProvider.LogKey} Review info load complete");
				_playReviewInfo = request.GetResult();
				Debug.Log($"{TDRateUsProvider.LogKey} {Newtonsoft.Json.JsonConvert.SerializeObject(_playReviewInfo)}");
			};
		}

		public void RateUs(UnityAction<bool> completeCallback)
		{
			if (_playReviewInfo == null)
			{
				Debug.LogWarning($"{TDRateUsProvider.LogKey} Review is not ready yet.");
				completeCallback?.Invoke(false);
				return;
			}


			var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
			launchFlowOperation.Completed += (launch) =>
			{
				if (launch.Error != ReviewErrorCode.NoError)
				{
					Debug.LogError($"{TDRateUsProvider.LogKey} Error in launching review: {launch.Error}");
					completeCallback?.Invoke(false);
					return;
				}

				Debug.Log($"{TDRateUsProvider.LogKey} Rate Us complete");
				completeCallback?.Invoke(true);
			};
		}
	}
}
#endif