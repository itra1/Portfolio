using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Elements.Widgets.Base.Presenter;
using UI.ShadedElements.Presenter.Base;
using UI.ShadedElements.Presenter.Targets.Base;
using Zenject;
using Debug = Core.Logging.Debug;

namespace UI.ShadedElements.Presenter
{
    public class ShadedScreenModesPresenter : ShadedElementsPresenterBase, IShadedScreenModesPresenter
    {
        private IDictionary<ITarget, Func<bool, UniTask>> _targets; //IFocusCapable
        private ITarget _target; //IMaximizable
        private Func<bool, UniTask> _onSizeRestoredAsync;
        private Tween _fadeAnimation;
        private CancellationTokenSource _disposeCancellationTokenSource;
        
        [Inject]
        public void Initialize()
        {
            _targets = new Dictionary<ITarget, Func<bool, UniTask>>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
        }
        
        public bool Contains(IFocusCapable target) => 
            !_disposeCancellationTokenSource.IsCancellationRequested && target != null && _targets.ContainsKey(target);
        
        public bool Add(IFocusCapable target, Action onFocused = null, Func<bool, UniTask> onUnfocusedAsync = null)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested || target == null || _targets.ContainsKey(target))
                return false;
            
            _targets.Add(target, onUnfocusedAsync);
            
            AddTarget(target, onFocused, -1);
            
            if (_targets.Count == 1 && _target == null)
                DoFadingIn();
            
            return true;
        }
        
        public bool Remove(IFocusCapable target)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested || target == null || !_targets.TryGetValue(target, out var onUnfocusedAsync))
                return false;
            
            _targets.Remove(target);
            
            RemoveTargetAsync(target, onUnfocusedAsync).Forget();
            
            if (_targets.Count == 0 && _target == null)
                DoFadingOut();
            
            return true;
        }
        
        public bool IsMaximized(IMaximizable target) => 
            !_disposeCancellationTokenSource.IsCancellationRequested && target != null && _target == target;
        
        public bool Maximize(IMaximizable target, Action onMaximized = null, Func<bool, UniTask> onSizeRestoredAsync = null)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested || target == null)
                return false;
            
            if (_target == null)
            {
                _target = target;
                _onSizeRestoredAsync = onSizeRestoredAsync;
                
                AddTarget(_target, onMaximized, 1);
                
                if (_targets.Count == 0)
                    DoFadingIn();
            }
            else
            {
                var t = _target;
                var func = _onSizeRestoredAsync;
                
                _target = null;
                _onSizeRestoredAsync = null;
                
                RemoveTargetAsync(t, func).Forget();
                
                _target = target;
                _onSizeRestoredAsync = onSizeRestoredAsync;
                
                AddTarget(_target, onMaximized, 1);
            }
            
            return true;
        }
        
        public bool RestoreSize(IMaximizable target)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested || target == null || _target == null || _target != target)
                return false;
            
            var onSizeRestoredAsync = _onSizeRestoredAsync;
            
            _target = null;
            _onSizeRestoredAsync = null;
            
            RemoveTargetAsync(target, onSizeRestoredAsync).Forget();
            
            if (_targets.Count == 0)
                DoFadingOut();
            
            return true;
        }
        
        public override void Unload()
        {
            if (_disposeCancellationTokenSource is { IsCancellationRequested: true })
                return;
            
            _disposeCancellationTokenSource.Cancel();
            _disposeCancellationTokenSource.Dispose();
            
            if (_targets.Count > 0)
            {
                foreach (var (target, onUnfocusedAsync) in _targets)
                    RemoveTargetAsync(target, onUnfocusedAsync, true).Forget();
                
                _targets.Clear();
            }
            
            if (_target != null)
            {
                var target = _target;
                var onSizeRestoredAsync = _onSizeRestoredAsync;
                
                _target = null;
                _onSizeRestoredAsync = null;
                
                RemoveTargetAsync(target, onSizeRestoredAsync, true).Forget();
            }
            
            DoFadingReset(true);
            
            base.Unload();
        }
        
        private void AddTarget(ITarget target, Action onAdded, int relativeIndexOrder = 0)
        {
            var targetRectTransform = target.RectTransform;
            
            targetRectTransform.SetParent(Content);
            
            switch (relativeIndexOrder)
            {
                case > 0:
                    targetRectTransform.SetAsLastSibling();
                    break;
                case < 0:
                    targetRectTransform.SetAsFirstSibling();
                    break;
            }

            if (target is IWidgetEventDispatcher widget)
                widget.Reset += OnTargetDisabled;
            
            target.Hidden += OnTargetDisabled;
            target.Unloaded += OnTargetDisabled;
            
            onAdded?.Invoke();
        }
        
        private async UniTaskVoid RemoveTargetAsync(ITarget target, Func<bool, UniTask> onRemovedAsync, bool forcibly = false)
        {
            if (target is IWidgetEventDispatcher widget)
                widget.Reset -= OnTargetDisabled;
            
            target.Hidden -= OnTargetDisabled;
            target.Unloaded -= OnTargetDisabled;
            
            if (onRemovedAsync != null)
            {
                if (!forcibly)
                {
                    await onRemovedAsync.Invoke(false);
                    
                    if (!_disposeCancellationTokenSource.IsCancellationRequested && !_targets.ContainsKey(target) && _target != target)
                        RestoreOriginalParentFor(target);
                }
                else
                {
                    onRemovedAsync.Invoke(true).Forget();
                    RestoreOriginalParentFor(target);
                }
            }
            else
            {
                if (!forcibly && _targets.Count == 0 && _target == null)
                {
                    try
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(AnimationDuration), cancellationToken: _disposeCancellationTokenSource.Token);
                        
                        if (!_targets.ContainsKey(target) && _target != target)
                            RestoreOriginalParentFor(target);
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        Debug.LogException(exception);
                    }
                }
                else
                {
                    RestoreOriginalParentFor(target);
                }
            }
        }
        
        private void RestoreOriginalParentFor(ITarget target) => 
            target.RectTransform.SetParent(target.OriginalParent);
        
        private void DoFadingIn() => 
            DoFading(MaxOpacity, () => Image.enabled = true, () => _fadeAnimation = null);
        
        private void DoFadingOut()
        {
            DoFading(0f, 
                null,
                () => 
                { 
                    _fadeAnimation = null;
                    
                    if (_targets.Count == 0 && _target == null) 
                        ResetImageToOriginalState();
                });
        }
        
        private void DoFadingReset(bool complete = false)
        {
            if (_fadeAnimation == null) 
                return;
            
            _fadeAnimation.Kill(complete);
            _fadeAnimation = null;
        }
        
        private void DoFading(float opacity, TweenCallback onStarted, TweenCallback onCompleted)
        {
            float duration;
            
            if (_fadeAnimation != null)
            {
                duration = _fadeAnimation.position;
                DoFadingReset();
            }
            else
            {
                duration = AnimationDuration;
            }

            _fadeAnimation = Image.DOFade(opacity, duration);
            
            if (onStarted != null)
                _fadeAnimation.OnStart(onStarted);
            
            if (onCompleted != null)
                _fadeAnimation.OnComplete(onCompleted);
        }
        
        private void OnTargetDisabled(object sender, EventArgs args)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested)
                return;
            
            var target = (ITarget) sender;
            
            if (_targets.Remove(target, out var onUnfocusedAsync))
            {
                RemoveTargetAsync(target, onUnfocusedAsync, true).Forget();
            }
            else if (_target == target)
            {
                var onSizeRestoredAsync = _onSizeRestoredAsync;
                
                _target = null;
                _onSizeRestoredAsync = null;
                
                RemoveTargetAsync(target, onSizeRestoredAsync, true).Forget();
            }
            
            if (_targets.Count == 0 && _target == null)
            {
                DoFadingReset(true);
                ResetImageToOriginalState();
            }
        }
    }
}