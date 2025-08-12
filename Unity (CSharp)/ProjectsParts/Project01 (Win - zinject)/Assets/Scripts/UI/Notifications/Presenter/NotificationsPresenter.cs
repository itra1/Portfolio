using System;
using System.Collections.Generic;
using com.ootii.Messages;
using Core.Messages;
using Core.Options.Offsets;
using Core.UI.Notifications.Data;
using Cysharp.Threading.Tasks;
using UI.Notifications.Presenter.Popups;
using UI.Notifications.Presenter.Popups.Factory;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.Notifications.Presenter
{
    public class NotificationsPresenter : MonoBehaviour, INotificationsPresenter
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private Color[] _colors;
        [SerializeField] private float _activityTime;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private float _verticalDistanceBetween;
        
        private RectTransform _rectTransform;
        private IScreenOffsets _screenOffsets;
        private INotificationPopupFactory _factory;
        private IList<INotificationPopup> _popups;
        
        private bool Visible => gameObject.activeSelf;
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        private RectTransform Parent => RectTransform.parent as RectTransform;
        
        public void Initialize(IScreenOffsets screenOffsets, INotificationPopupFactory factory)
        {
            _screenOffsets = screenOffsets;
            _factory = factory;
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }
        
        private void AlignToParent()
        {
            var parent = Parent;

            if (parent == null)
            {
                Debug.LogError("Parent not found when trying to align the timers presenter to the bounds of its parent");
                return;
            }
            
            var rectTransform = RectTransform;
            var parentRect = parent.rect;
            
            var offsetLeft = _screenOffsets.Left;
            var offsetRight = _screenOffsets.Right;
            var offsetTop = _screenOffsets.Top;
            var offsetBottom = _screenOffsets.Bottom;
            
            rectTransform.anchoredPosition = new Vector2(-offsetRight, offsetBottom);
            rectTransform.sizeDelta = new Vector2(parentRect.width - (offsetRight + offsetLeft), parentRect.height - (offsetBottom + offsetTop));
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
            MessageDispatcher.RemoveListener(MessageType.NotificationDisplay, OnNotificationDisplayed);
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            RemovePopups();

            Hide();
            
            _screenOffsets = null;
            _factory = null;
        }

        private bool TryGetColor(NotificationType type, out Color color)
        {
            var id = (int) type;
            
            if (id < 0 || id >= _colors.Length)
            {
                color = default;
                return false;
            }
            
            color = _colors[id];
            return true;
        }
        
        private void RegisterPopups() => _popups = new List<INotificationPopup>();
        
        private void RemovePopups()
        {
            if (_popups == null)
                return;
            
            foreach (var popup in _popups)
                RemovePopup(popup);
            
            _popups.Clear();
            _popups = null;
        }
        
        private INotificationPopup AttemptToCreatePopup(NotificationType type, string text)
        {
            if (!TryGetColor(type, out var color))
            {
                Debug.LogError($"Color value is missing for notification type {type} when trying to create a notification popup");
                return null;
            }
            
            var popup = _factory.Create(_content, text, color, _activityTime, _fadeDuration);
            
            popup.Shown += OnPopupShown;
            popup.Hidden += OnPopupHidden;
            
            popup.ShowAsync().Forget();
            
            return popup;
        }
        
        private void RemovePopup(INotificationPopup popup)
        {
            popup.Shown -= OnPopupShown;
            popup.Hidden -= OnPopupHidden;
            
            popup.Dispose();
        }
        
        private void OnPopupShown(object sender, EventArgs args)
        {
            if (!Visible)
                Show();
        }
        
        private void OnPopupHidden(object sender, EventArgs args)
        {
            var popup = (INotificationPopup) sender;
            
            RemovePopup(popup);
            
            _popups.Remove(popup);
            
            if (_popups.Count == 0 && Visible)
            {
                Hide();
            }
            else
            {
                for (var i = _popups.Count - 1; i >= 0; i--)
                    _popups[i].MoveTo(new Vector2(-50, _verticalDistanceBetween + i * (popup.Size.y + _verticalDistanceBetween)), 0.5f);
            }
        }

        private void OnNotificationDisplayed(IMessage message)
        {
            var info = (NotificationInfo) message.Data;
            
            var popup = AttemptToCreatePopup(info.Type, info.Text);
            
            if (popup == null)
                return;
            
            popup.MoveTo(new Vector2(-50, _verticalDistanceBetween + _popups.Count * (popup.Size.y + _verticalDistanceBetween)));
            
            _popups.Add(popup);
        }
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            AlignToParent();
            RegisterPopups();
            
            MessageDispatcher.AddListener(MessageType.NotificationDisplay, OnNotificationDisplayed);
        }
    }
}