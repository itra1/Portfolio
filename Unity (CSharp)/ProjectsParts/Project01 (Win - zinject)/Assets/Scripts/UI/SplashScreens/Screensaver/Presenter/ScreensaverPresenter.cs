using com.ootii.Messages;
using Core.FileResources;
using Core.Materials.Loading.Loader;
using Core.Materials.Storage;
using Core.Messages;
using Core.Options.Offsets;
using Cysharp.Threading.Tasks;
using Elements.Windows.VideoSplit.Presenter.VideoPlayer.Factory;
using UI.Base.Presenter;
using UI.SplashScreens.Screensaver.Presenter.Components;
using UnityEngine;
using Utils;
using Debug = Core.Logging.Debug;

namespace UI.SplashScreens.Screensaver.Presenter
{
    public class ScreensaverPresenter : HiddenPresenterBase, IScreensaverPresenter
    {
        [SerializeField] private ScreensaverBackground _background;
        [SerializeField] private EmbeddedScreensaver _embedded;
        [SerializeField] private SplittedScreensaver _splitted;
        
        private IScreenOffsets _screenOffsets;
        
        private RectTransform _rectTransform;
        private IScreensaver _current;

        public bool Active { get; private set; }

        private RectTransform RectTransform => 
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public void Initialize(IScreenOffsets screenOffsets,
            IResourceProvider resources, 
            IMaterialDataLoader materialLoader,
            IMaterialDataStorage materials,
            IVideoSplitPlayerFactory playerFactory)
        {
            _screenOffsets = screenOffsets;
            
            _background.Initialize();
            _embedded.Initialize();
            _splitted.Initialize(resources, materialLoader, materials, playerFactory);
            
            SetInitialOpacity(1f);
        }

        public async UniTaskVoid SetAsync(bool isVisible, string type, ulong? materialId)
        {
            if (isVisible)
            {
                gameObject.SetActive(true);
                Active = true;
                
                if (_current != null)
                {
                    MessageDispatcher.SendMessage(this, MessageType.ScreensaverActivity, false, EnumMessageDelay.IMMEDIATE);
                    
                    if (await _current.HideAsync())
                        _current.StopIfPlaying();
                }
                else
                {
                    _background.ShowAsync().Forget();
                }
                
                if (materialId == null)
                {
                    _current = _embedded;
                    
                    if (!await _embedded.StartPlayingAsync(type))
                    {
                        Debug.LogError($"Failed attempt detected to start playback of the embedded screensaver by type {type}");
                        return;
                    }
                }
                else
                {
                    _current = _splitted;
                    
                    _splitted.ForcedShow();
                    
                    if (!await _splitted.StartPlayingAsync(materialId.Value))
                    {
                        Debug.LogError($"Failed attempt detected to start playback of the splitted screensaver by material id: {materialId.Value}");
                        return;
                    }
                }
                
                await _current.ShowAsync();
                
                MessageDispatcher.SendMessage(this, MessageType.ScreensaverActivity, true, EnumMessageDelay.IMMEDIATE);
            }
            else
            {
                MessageDispatcher.SendMessage(this, MessageType.ScreensaverActivity, false, EnumMessageDelay.IMMEDIATE);
                
                if (_current != null)
                {
                    _background.HideAsync().Forget();
                    
                    if (!await _current.HideAsync())
                        return;
                    
                    _current.StopIfPlaying();
                    _current = null;
                }
                
                gameObject.SetActive(false);
                Active = false;
            }
        }
        
        public void TogglePlayPause() => _current?.TogglePlayPause();
        
        public override void Unload()
        {
            _embedded.Unload();
            _splitted.Unload();
            
            _current = null;
            _rectTransform = null;
            _screenOffsets = null;
            
            Active = false;
            
            base.Unload();
        }
        
        private void Awake() => AlignToParent();

        private void AlignToParent()
        {
            var rectTransform = RectTransform;
            
            rectTransform.ResetAnchors(Vector2.one * 0.5f);
            rectTransform.Reset();
			
            var sizeDeltaX = -(_screenOffsets.Left + _screenOffsets.Right);
            var sizeDeltaY = -(_screenOffsets.Bottom + _screenOffsets.Top);
            var anchoredPositionX = sizeDeltaX * 0.5f;
            var anchoredPositionY = -sizeDeltaY * 0.5f;
			
            rectTransform.anchoredPosition = new Vector2(anchoredPositionX, anchoredPositionY);
            rectTransform.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
        }
    }
}