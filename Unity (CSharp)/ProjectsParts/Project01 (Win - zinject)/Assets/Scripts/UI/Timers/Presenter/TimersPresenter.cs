using System;
using System.Collections.Generic;
using System.Threading;
using Core.Options.Offsets;
using Core.UI.Timers.Data;
using Cysharp.Threading.Tasks;
using UI.Base.Presenter;
using UI.Timers.Controller.Items.Base;
using UI.Timers.Presenter.Widgets;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.Timers.Presenter
{
    public class TimersPresenter : HiddenPresenterBase, ITimersPresenter
    {
        [SerializeField] private AudioSource _alarm;

        private IScreenOffsets _screenOffsets;
        
        private RectTransform _rectTransform;
        private IDictionary<TimerType, ITimerWidget> _widgets;
        private CancellationTokenSource _updateCancellationTokenSource;
        
        public bool Active { get; private set; }
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        private RectTransform Parent => RectTransform.parent as RectTransform;
        
        public bool IsAnyWidgetVisible
        {
            get
            {
                foreach (var widget in _widgets.Values)
                {
                    if (widget.Visible)
                        return true;
                }
                
                return false;
            }
        }
        
        public void Initialize(IScreenOffsets screenOffsets, IEnumerable<ITimerInfo> infoList)
        {
            _screenOffsets = screenOffsets;
            
            SetInitialOpacity(0f);
            
            _alarm.playOnAwake = false;
            
            var widgets = GetComponentsInChildren<ITimerWidget>(true);
            
            if (widgets == null || widgets.Length == 0)
            {
                Debug.LogError("The list of timer widgets is empty when trying to initialize the timers presenter");
                return;
            }
            
            _widgets = new Dictionary<TimerType, ITimerWidget>();
            
            foreach (var widget in widgets)
            {
                var type = widget.Type;
                
                if (!_widgets.TryAdd(type, widget))
                    Debug.LogError($"An attempt was detected to add a timer widget with duplicate type {type} to the dictionary of available timer widgets");
            }

            foreach (var info in infoList)
            {
                var type = info.Type;
                
                if (!_widgets.TryGetValue(type, out var widget))
                {
                    Debug.LogError($"A widget with a timer type \"{type}\" is missing when trying to initialize the timers presenter");
                    continue;
                }
                
                widget.Initialize(info);
            }
        }
        
        public void AlignToParent()
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
            
            rectTransform.anchoredPosition = new Vector2(offsetLeft, offsetBottom);
            rectTransform.sizeDelta = new Vector2(parentRect.width - (offsetLeft + offsetRight), parentRect.height - (offsetBottom + offsetTop));
        }
        
        public void PlayAlarm()
        {
            if (_alarm == null)
            {
                Debug.LogError("Audio source is missing when trying to play alarm");
                return;
            }
            
            _alarm.Play();
        }
        
        public void Activate()
        {
            if (Active)
                return;
            
            Active = true;
            
            UpdateAsync().Forget();
        }

        public void Deactivate()
        {
            if (!Active)
                return;
            
            Active = false;
            
            if (_updateCancellationTokenSource != null)
            {
                _updateCancellationTokenSource.Cancel();
                _updateCancellationTokenSource.Dispose();
                _updateCancellationTokenSource = null;
            }
            
            if (_widgets != null)
            {
                foreach (var widget in _widgets.Values)
                {
                    if (widget.Visible)
                        widget.Hide();
                }
            }
        }

        public bool ShowWidget(in TimerType type)
        {
            if (!_widgets.TryGetValue(type, out var widget))
            {
                Debug.LogError($"A widget with a timer type \"{type}\" is missing when trying to show it");
                return false;
            }
            
            if (widget.Visible)
                return false;
            
            widget.Show();
            return true;
        }
        
        public bool HideWidget(in TimerType type)
        {
            if (!_widgets.TryGetValue(type, out var widget))
            {
                Debug.LogError($"A widget with a timer type \"{type}\" is missing when trying to hide it");
                return false;
            }
            
            if (!widget.Visible)
                return false;
            
            widget.Hide();
            return true;
        }
        
        public override void Unload()
        {
            Deactivate();
            
            if (_widgets != null)
            {
                _widgets.Clear();
                _widgets = null;
            }
            
            try
            {
                _rectTransform = null;
            }
            catch (Exception)
            {
                // ignored
            }

            _screenOffsets = null;
            
            base.Unload();
        }
        
        private async UniTaskVoid UpdateAsync()
        {
            _updateCancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                var cancellationToken = _updateCancellationTokenSource.Token;
                
                while (_updateCancellationTokenSource != null)
                {
                    foreach (var widget in _widgets.Values)
                    {
                        if (widget.Visible)
                            widget.Refresh();
                    }
					
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
    }
}