using com.ootii.Messages;
using Core.Messages;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = Core.Logging.Debug;

namespace UI.SplashScreens.Intro.Presenter
{
    [DisallowMultipleComponent, RequireComponent(typeof(RawImage), typeof(VideoPlayer), typeof(CanvasGroup))]
    public class IntroPresenter : MonoBehaviour, IIntroPresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RawImage _image;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private float _fadeDuration;

        private Texture _defaultTexture;
        private Tween _fadeAnimation;
        
        private void Awake()
        {
            _defaultTexture = _image.texture;
            
            var graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(TextureFormat.RGBA32, false);
            
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = $"{Application.streamingAssetsPath}/Intro.mp4";
            _videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = false;
            _videoPlayer.waitForFirstFrame = true;
            _videoPlayer.skipOnDrop = false;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.playbackSpeed = 1f;
            _videoPlayer.targetTexture = new RenderTexture(4098, 792, 0, graphicsFormat, 0);
            _videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
            
            _videoPlayer.started += OnVideoPlayerStarted;
            _videoPlayer.loopPointReached += OnVideoPlayerLoopPointReached;
            _videoPlayer.errorReceived += OnVideoPlayerError;
            
            MessageDispatcher.AddListener(MessageType.BackgroundStartPlaying, OnBackgroundStartPlaying);
            
            _videoPlayer.Play();
        }

        private void CleanUp()
        {
            MessageDispatcher.RemoveListener(MessageType.BackgroundStartPlaying, OnBackgroundStartPlaying);

            if (_fadeAnimation != null)
            {
                _fadeAnimation.Kill();
                _fadeAnimation = null;
            }
            
            if (_defaultTexture != null)
            {
                _image.texture = _defaultTexture;
                _defaultTexture = null;
            }
            
            _videoPlayer.started -= OnVideoPlayerStarted;
            _videoPlayer.loopPointReached -= OnVideoPlayerLoopPointReached;
            _videoPlayer.errorReceived -= OnVideoPlayerError;
            
            if (_videoPlayer.isPlaying)
                _videoPlayer.Stop();

            if (_videoPlayer.targetTexture != null)
            {
                Destroy(_videoPlayer.targetTexture);
                _videoPlayer.targetTexture = null;
            }
            
            _videoPlayer.url = string.Empty;
            
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;
            
            gameObject.SetActive(false);
        }
        
        public void Unload()
        {
            if (_defaultTexture != null)
                CleanUp();
        }
        
        private void DoFadingOut()
        {
            if (_canvasGroup != null && _fadeDuration > 0)
            {
                _fadeAnimation = _canvasGroup.DOFade(0f, _fadeDuration)
                    .SetAutoKill(false)
                    .OnComplete(OnFadeAnimationCompleted);
            }
            else
            {
                CleanUp();
            }
        }

        private void DispatchMessageThatIntroCompleted()
        {
            _videoPlayer.loopPointReached -= OnVideoPlayerLoopPointReached;
            MessageDispatcher.SendMessage(MessageType.IntroComplete);
        }
        
        private void OnVideoPlayerStarted(VideoPlayer source)
        {
            _videoPlayer.started -= OnVideoPlayerStarted;
            _image.texture = _videoPlayer.targetTexture;
        }

        private void OnVideoPlayerLoopPointReached(VideoPlayer source) => DispatchMessageThatIntroCompleted();
        
        private void OnVideoPlayerError(VideoPlayer source, string message)
        {
            _videoPlayer.errorReceived -= OnVideoPlayerError;
            
            Debug.LogError(message);
            
            _image.texture = _defaultTexture;
            _defaultTexture = null;
        }
        
        private void OnBackgroundStartPlaying(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.BackgroundStartPlaying, OnBackgroundStartPlaying);
            DoFadingOut();
        }

        private void OnFadeAnimationCompleted() => CleanUp();
    }
}