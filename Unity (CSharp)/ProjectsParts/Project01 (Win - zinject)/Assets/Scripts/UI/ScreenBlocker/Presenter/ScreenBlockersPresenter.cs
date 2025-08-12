using System.Collections.Generic;
using System.Linq;
using com.ootii.Messages;
using Core.Messages;
using Core.Options.Offsets;
using UI.ScreenBlocker.Presenter.Popups.Base;
using UI.ScreenBlocker.Presenter.Popups.Common.Factory;
using UI.ScreenBlocker.Presenter.Popups.Dialog;
using UnityEngine;
using Utils;

namespace UI.ScreenBlocker.Presenter
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class ScreenBlockersPresenter : MonoBehaviour, IScreenBlockersPresenter
    {
        [SerializeField] private RectTransform _content;
        
        private IScreenOffsets _screenOffsets;
        private RectTransform _rectTransform;
        private IScreenBlockerPopupFactory _factory;
        private ISet<IScreenBlockerPopup> _popups;
        
        private IScreenBlockerPopup _currentPopup;
        
        private bool Visible => gameObject.activeSelf;
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public void Initialize(IScreenOffsets screenOffsets, IScreenBlockerPopupFactory factory)
        {
            _screenOffsets = screenOffsets;
            _factory = factory;
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }
        
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

        private void Show()
        {
            if (Visible)
                return;
            
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            try
            {
                if (!Visible)
                    return;

                gameObject.SetActive(false);
            }
            catch
            {
                // ignored
            }
        }
        
        public void Unload()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            RemovePopups();
            
            Hide();

            _screenOffsets = null;
            _factory = null;
        }
        
        private void RegisterPopups()
        {
            _popups = new HashSet<IScreenBlockerPopup>();
            _popups.Add(CreatePopup<DialogPopup>());
        }

        private void RemovePopups()
        {
            if (_popups == null)
                return;
            
            foreach (var popup in _popups)
                RemovePopup(popup);
            
            _popups.Clear();
            _popups = null;
        }
        
        private IScreenBlockerPopup CreatePopup<TScreenBlockerPopup>() 
            where TScreenBlockerPopup : IScreenBlockerPopup
        {
            var popup = _factory.Create<TScreenBlockerPopup>(_content);
            
            popup.Shown += OnPopupShown;
            popup.Hidden += OnPopupHidden;
            
            return popup;
        }
        
        private void RemovePopup(IScreenBlockerPopup popup)
        {
            popup.Shown -= OnPopupShown;
            popup.Hidden -= OnPopupHidden;
            
            popup.Dispose();
        }

        private void OnPopupShown()
        {
            if (!Visible)
                Show();
        }
        
        private void OnPopupHidden()
        {
            if (_popups.All(popup => !popup.Visible) && Visible)
                Hide();
        }

        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            AlignToParent();
            RegisterPopups();
        }
    }
}