using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Video;

namespace Providers.Splash.Presenter
{
	public class SplashPresenter: MonoBehaviour, ISplashPresenter
	{
		[SerializeField] private VideoPlayer _videoPlayer;

		public void Play(UnityAction onComplete)
		{
			_videoPlayer.Play();

			_videoPlayer.loopPointReached += async (res)=> {
				await UniTask.Delay(1000);
				onComplete?.Invoke();
				gameObject.SetActive(false);
			};
		}
	}
}
