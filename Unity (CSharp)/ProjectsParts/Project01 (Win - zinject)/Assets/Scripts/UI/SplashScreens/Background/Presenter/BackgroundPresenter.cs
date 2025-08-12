using System;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.ScreenModes;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Elements.ScreenModes.Controller;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = Core.Logging.Debug;

namespace UI.SplashScreens.Background.Presenter
{
	[DisallowMultipleComponent]
	public class BackgroundPresenter : MonoBehaviour, IBackgroundPresenter
	{
		[SerializeField] private RawImage _image;
		[SerializeField] private VideoPlayer _videoPlayer;
		
		private IScreenMode _screenMode;
		private Texture _defaultTexture;
		private CancellationTokenSource _delayCancellationTokenSource;
		
		public void Initialize(IScreenMode screenMode)
		{
			_screenMode = screenMode;
			
			MessageDispatcher.AddListener(MessageType.IntroComplete, OnIntroCompleted);
			MessageDispatcher.AddListener(MessageType.ScreenModeChange, OnScreenModeChanged);
		}
		
		private void Awake()
		{
			_defaultTexture = _image.texture;
			
			var graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(TextureFormat.RGBA32, false);
			
			_videoPlayer.source = VideoSource.Url;
			_videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
			_videoPlayer.playOnAwake = false;
			_videoPlayer.isLooping = true;
			_videoPlayer.waitForFirstFrame = true;
			_videoPlayer.skipOnDrop = false;
			_videoPlayer.renderMode = VideoRenderMode.RenderTexture;
			_videoPlayer.playbackSpeed = 1f;
			_videoPlayer.targetTexture = new RenderTexture(4098, 792, 0, graphicsFormat, 0);
			_videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
			
			_videoPlayer.started += OnVideoPlayerStarted;
			_videoPlayer.errorReceived += OnVideoPlayerError;
		}

		public void Unload()
		{
			if (_defaultTexture == null)
				return;
			
			if (_delayCancellationTokenSource != null)
			{
				_delayCancellationTokenSource.Cancel();
				_delayCancellationTokenSource.Dispose();
				_delayCancellationTokenSource = null;
			}
			
			MessageDispatcher.RemoveListener(MessageType.IntroComplete, OnIntroCompleted);
			MessageDispatcher.RemoveListener(MessageType.ScreenModeChange, OnScreenModeChanged);
			
			_videoPlayer.started -= OnVideoPlayerStarted;
			_videoPlayer.errorReceived -= OnVideoPlayerError;
			
			ShowDefaultState();
			
			StopIfPlaying();
			
			if (_videoPlayer.targetTexture != null)
			{
				Destroy(_videoPlayer.targetTexture);
				_videoPlayer.targetTexture = null;
			}
			
			_screenMode = null;
			_defaultTexture = null;
		}

		private void StopIfPlaying()
		{
			if (_videoPlayer.isPlaying)
				_videoPlayer.Stop();
		}
		
		private void PlayDesktopBackground()
		{
			StopIfPlaying();
			_videoPlayer.url = $"{Application.streamingAssetsPath}/BackgroundDesktop.mp4";
			_videoPlayer.Play();
		}
		
		private void PlayAnyBackground()
		{
			StopIfPlaying();
			_videoPlayer.url = $"{Application.streamingAssetsPath}/BackgroundAny.mp4";
			_videoPlayer.Play();
		}
		
		private void ShowDefaultState()
		{
			StopIfPlaying();
			_image.texture = _defaultTexture;
		}
		
		private void HandleScreenModeState(ScreenMode screenMode)
		{
			switch (screenMode)
			{
				case ScreenMode.Presentation:
				case ScreenMode.Status:
				{
					PlayAnyBackground();
					break;
				}
				default:
				{
					PlayDesktopBackground();
					break;
				}
			}
		}
		
		private void OnVideoPlayerStarted(VideoPlayer source) => OnVideoPlayerStartedAsync().Forget();

		private async UniTaskVoid OnVideoPlayerStartedAsync()
		{
			_image.texture = _videoPlayer.targetTexture;
			_delayCancellationTokenSource = new CancellationTokenSource();
			
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: _delayCancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
			finally
			{
				if (_delayCancellationTokenSource != null)
				{
					_delayCancellationTokenSource.Dispose();
					_delayCancellationTokenSource = null;
				}
			}
			
			MessageDispatcher.SendMessage(MessageType.BackgroundStartPlaying);
		}
		
		private void OnVideoPlayerError(VideoPlayer source, string message)
		{
			Debug.LogError(message);
			ShowDefaultState();
		}
		
		private void OnIntroCompleted(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.IntroComplete, OnIntroCompleted);
			
			gameObject.SetActive(true);
			
			HandleScreenModeState(_screenMode.CurrentMode);
		}
		
		private void OnScreenModeChanged(IMessage message) => HandleScreenModeState((ScreenMode) message.Data);
	}
}