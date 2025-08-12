using System.Collections.Generic;
using Core.UI.SplashScreens.Screensaver.Data.Consts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = Core.Logging.Debug;

namespace UI.SplashScreens.Screensaver.Presenter.Components
{
    public class EmbeddedScreensaver : ScreensaverBase, IScreensaver
    {
        [SerializeField] private RawImage _image;
        [SerializeField] private VideoPlayer _videoPlayer;

        private IDictionary<string, string> _fileNamesByType;
        private Texture _defaultTexture;
        private long? _pauseFrame;
        
        public void Initialize()
        {
            _fileNamesByType = new Dictionary<string, string>
            {
                {ScreensaverType.Dynamic, "ScreensaverLong.mp4"},
                {ScreensaverType.Loop, "ScreensaverShortLoop.mp4"},
            };
            
            SetInitialOpacity(0f);
        }
        
        public UniTask<bool> StartPlayingAsync(string type)
        {
            StopIfPlaying();
            
            if (!_fileNamesByType.TryGetValue(type, out var fileName))
                return new UniTask<bool>(type == ScreensaverType.Static);
            
            var graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(TextureFormat.RGBA32, false);
            var renderTexture = new RenderTexture(4098, 792, 0, graphicsFormat, 0);
            
            _defaultTexture = _image.texture;
            _image.texture = renderTexture;
            
            _image.enabled = true;
            
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = $"{Application.streamingAssetsPath}/{fileName}";
            _videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = false;
            _videoPlayer.waitForFirstFrame = true;
            _videoPlayer.skipOnDrop = false;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.playbackSpeed = 1f;
            _videoPlayer.targetTexture = renderTexture;
            _videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
            _videoPlayer.started += OnVideoPlayerStarted;
            _videoPlayer.errorReceived += OnVideoPlayerError;
            
            gameObject.SetActive(true);
            
            _videoPlayer.Play();
            
            return new UniTask<bool>(true);
        }

        public void TogglePlayPause()
        {
            if (_videoPlayer == null)
                return;
            
            if (_pauseFrame != null)
            {
                _image.enabled = false;
                _videoPlayer.Play();
            }
            else
            {
                _pauseFrame = _videoPlayer.frame;
                
                _videoPlayer.Pause();
            }
        }

        public void StopIfPlaying()
        {
            if (string.IsNullOrEmpty(_videoPlayer.url))
                return;
            
            _pauseFrame = null;
            
            if (_defaultTexture != null)
            {
                _image.texture = _defaultTexture;
                _defaultTexture = null;
            }

            _image.enabled = true;
            
            _videoPlayer.started -= OnVideoPlayerStarted;
            _videoPlayer.errorReceived -= OnVideoPlayerError;
            
            if (_videoPlayer.isPlaying)
                _videoPlayer.Stop();
            
            if (_videoPlayer.targetTexture != null)
            {
                Destroy(_videoPlayer.targetTexture);
                _videoPlayer.targetTexture = null;
            }
            
            _videoPlayer.url = string.Empty;
        }
        
        public override void Unload()
        {
            StopIfPlaying();
            base.Unload();
        }
        
        private void OnVideoPlayerError(VideoPlayer source, string message)
        {
            _videoPlayer.errorReceived -= OnVideoPlayerError;
            Debug.LogError(message);
        }
        
        private void OnVideoPlayerStarted(VideoPlayer source)
        {
            if (_pauseFrame == null) 
                return;
            
            _videoPlayer.frame = _pauseFrame.Value;
            _pauseFrame = null;
            
            _image.enabled = true;
        }
    }
}