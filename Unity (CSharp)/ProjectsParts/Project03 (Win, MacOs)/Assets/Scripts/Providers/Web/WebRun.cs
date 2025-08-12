using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Video;

namespace Providers.Web
{
	public class WebRun : MonoBehaviour
	{
		[SerializeField] private UniWebView _webView;
		[SerializeField] private VideoPlayer _videoPlayer;
		//[SerializeField] private CanvasGroup _cg;
		private bool _isSplash;
		private bool _isLoad;

		private void Awake()
		{
			_isSplash = false;
			_isLoad = false;
			//_videoPlayer.gameObject.SetActive(true);
			_videoPlayer.loopPointReached += OnPlayComplete;
			_webView.OnPageFinished += OnPageFinished;
			_videoPlayer.Play();
			//_cg.alpha = 0;
			//_videoPlayer.started += FirstFrameReady;
		}
		private void Start()
		{
			_webView.Frame = new Rect(Screen.width * 2, 0, Screen.width, Screen.height - 200);
		}

		private void FirstFrameReady(VideoPlayer source)
		{
			_videoPlayer.started -= FirstFrameReady;
			//_cg.alpha = 1;
		}

		private void OnPageFinished(UniWebView webView, int statusCode, string url)
		{
			_webView.OnPageFinished -= OnPageFinished;
			_isLoad = true;
			CheckComplete();

		}
		private void OnPlayComplete(VideoPlayer source)
		{
			_isSplash = true;
			//_cg.alpha = 1f;
			Invoke("CheckComplete", 5);
		}

		private void CheckComplete()
		{
			if (!_isSplash || !_isLoad) return;
			_webView.Frame = new Rect(0, Screen.height * 0.04f, Screen.width, Screen.height * 0.96f);
			_videoPlayer.gameObject.SetActive(false);
		}
	}
}