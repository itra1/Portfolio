using System;
using Base.Presenter;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.Windows.Base.Data.Utils;
using Elements.Windows.Base.Presenter;
using Elements.Windows.Video.Presenter.Components;
using Elements.Windows.Video.Presenter.VideoPlayer;
using UI.Switches;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Video
{
    public abstract class VideoPresenterAdapterBase<TWindowPresenter, TWindowHeader> : WindowPresenterAdapterBase<TWindowPresenter>,
        INonRenderedCapable, IPointerEnterHandler, IPointerExitHandler
        where TWindowPresenter : IWindowPresenter, INonRenderedCapable
        where TWindowHeader : WindowHeader
    {
        [SerializeField] private TWindowHeader _header;
        
        private ICustomTriggerSwitch _triggerSwitch;
        
        private bool _isPointerInside;
        private double _currentTime;
        
        protected IAdjustableWidthBar Header => (IAdjustableWidthBar) _header;
        
        protected abstract IVideoPlayer Player { get; }
        
        [Inject]
        private void Initialize(ICustomTriggerSwitch triggerSwitch) => _triggerSwitch = triggerSwitch;
        
        public override void SetVideoStateInOutgoingState(string uuid)
        {
            var player = Player;
            
            var isPlaying = player.IsPlaying;
            var isLooping = player.IsLooping;
            var currentTime = (long) player.CurrentTime;
            
			OutgoingState.Context.SetFloatingVideoWindowState(uuid, Material, isPlaying, isLooping, currentTime);
        }
        
        public void SetNonRenderedContainer(INonRenderedContainer container) => 
            Adaptee.SetNonRenderedContainer(container);
        
        public override UniTask<bool> PreloadAsync()
        {
            _header.SetTitle(Material.Name);
            _header.SetIconSprite(Settings.GetWindowMaterialIconSprite(Material.GetIconType()));
			
            _header.Closed += OnHeaderClosed;

            return base.PreloadAsync();
        }
        
        public override bool Hide()
        {
            if (!base.Hide())
                return false;
            
            _triggerSwitch.Reset(Adaptee.TriggerKey);
            
            _isPointerInside = false;
            _currentTime = 0.0;
            
            return true;
        }
        
        public override void Unlock()
        {
            base.Unlock();
            
            if (!_isPointerInside && IsTriggerOn)
                _triggerSwitch.Disable(Adaptee.TriggerKey);
        }
        
        public override void Unload()
        {
            _header.Closed -= OnHeaderClosed;
            
            if (_triggerSwitch != null)
            {
                _triggerSwitch.Reset(Adaptee.TriggerKey);
                _triggerSwitch = null;
            }
            
            _isPointerInside = false;
            _currentTime = 0.0;
			
            base.Unload();
        }
        
        protected override void EnableComponents()
        {
            base.EnableComponents();
            
            var player = Player;
            
            if (player != null)
            {
                player.PlaybackStarted += OnPlaybackStarted;
                player.PlaybackPaused += OnPlaybackPaused;
                player.PlaybackStopped += OnPlaybackStopped;
                player.PlaybackLooped += OnPlaybackLooped;
                player.PlaybackTimeChanged += OnPlaybackTimeChanged;
            }
            
            if (_header != null && !_header.Visible && IsTriggerOn)
                _header.Show(true);
        }
        
        protected override void DisableComponents()
        {
            var player = Player;

            if (player != null)
            {
                player.PlaybackStarted -= OnPlaybackStarted;
                player.PlaybackPaused -= OnPlaybackPaused;
                player.PlaybackStopped -= OnPlaybackStopped;
                player.PlaybackLooped -= OnPlaybackLooped;
                player.PlaybackTimeChanged -= OnPlaybackTimeChanged;
            }
            
            if (_header != null && _header.Visible)
                _header.Hide(true);
            
            base.DisableComponents();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;
            
            if (!Locked)
                _triggerSwitch.Enable(Adaptee.TriggerKey);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Locked)
                _triggerSwitch.Disable(Adaptee.TriggerKey);
            
            _isPointerInside = false;
        }

        private void OnPlaybackStarted()
        {
            _currentTime = 0.0;
            DispatchStateChangedEvent();
        }

        private void OnPlaybackPaused()
        {
            _currentTime = 0.0;
            DispatchStateChangedEvent();
        }
        
        private void OnPlaybackStopped()
        {
            _currentTime = 0.0;
            DispatchStateChangedEvent();
        }
        
        private void OnPlaybackLooped(bool looping) => DispatchStateChangedEvent();
        
        private void OnPlaybackTimeChanged(double currentTime, double duration)
        {
            if (Math.Abs(currentTime - _currentTime) >= double.Epsilon)
            {
                _currentTime = currentTime;
                DispatchStateChangedEvent();
            }
        }
        
        protected override void OnWindowPanelsToggled(bool visible)
        {
            base.OnWindowPanelsToggled(visible);
			
            if (_header != null)
            {
                if (visible)
                    _header.Show();
                else
                    _header.Hide();
            }
        }
        
        private void OnHeaderClosed() => DispatchClosedEvent();
    }
}