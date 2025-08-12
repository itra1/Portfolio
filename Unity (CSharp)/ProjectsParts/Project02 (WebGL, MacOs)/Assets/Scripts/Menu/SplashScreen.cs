using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.UI;
using com.ootii.Geometry;
using System.Threading.Tasks;

public class SplashScreen : Singleton<SplashScreen>
{
	[SerializeField] private VideoPlayer _videoPlayer;
	[SerializeField] private RectTransform _rect;
	[SerializeField] private Image Image;

	private CanvasGroup _cg;
	private bool _onePlay = false;
	private bool _isPlay;
	private bool _loadComplete;

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS

	private void Awake()
	{
#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
		if (Image == null)
			Image = GetComponentInChildren<Image>(true);
#endif
		ShowImage();
	}

	private void ShowImage()
	{
		Image.gameObject.SetActive(true);
		_cg = Image.gameObject.GetOrAddComponent<CanvasGroup>();
		_cg.alpha = 1;

#if UNITY_ANDROID || UNITY_EDITOR || UNITY_IOS
		WaitLoadServers(1,false);
#else
		WaitLoadServers(7,true);
#endif
	}

	private void WaitLoadServers(float delayTime = 1, bool forceWait = false)
	{
		if (forceWait || !ServerManager.ExistsServers)
		{
			DOVirtual.DelayedCall(delayTime, () =>
			{
				WaitLoadServers();
			});
			return;
		}

		DOTween.To(() => _cg.alpha, (x) => _cg.alpha = x, 0f, 1f).OnComplete(() =>
		{
			Image.gameObject.SetActive(false);
		});
	}

	public void LoadComplete()
	{
		//CheckComplete();
	}

	//private async void CheckComplete()
	//{
	//	await Task.Delay(1000);

	//	DOTween.To(() => _cg.alpha, (x) => _cg.alpha = x, 0f, 1f).OnComplete(() =>
	//	{
	//		Image.gameObject.SetActive(false);
	//	});
	//}

#else


	private void Awake()
	{
		Play();
	}

	public void LoadComplete(){
		_loadComplete = true;
		CheckComplete();
	}

	private void CheckComplete(){
		if (!_loadComplete || _isPlay || !_onePlay) return;

		PlayComplete();
	}
	[ContextMenu("Play")]
	public void Play(){
		_isPlay = true;

		_rect.gameObject.SetActive(true);
		_videoPlayer.Play();
	}

	private void Update()
	{
		if (!_isPlay) return;

		if (_videoPlayer.isPlaying)
			_onePlay = true;

		if (!_videoPlayer.isPlaying && _onePlay)
		{

			_isPlay = false;
			CheckComplete();

		}

	}

	private void PlayComplete(){

		CanvasGroup cg = _videoPlayer.GetComponent<CanvasGroup>();

		DOTween.To(() => cg.alpha, (x) => cg.alpha = x, 0f, 0.5f).OnComplete(() =>
		{
			_rect.gameObject.SetActive(false);
		}).SetDelay(3);

	}
#endif

}