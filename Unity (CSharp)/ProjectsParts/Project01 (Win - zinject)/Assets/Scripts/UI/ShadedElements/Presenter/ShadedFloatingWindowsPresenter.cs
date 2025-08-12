using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.ShadedElements.Presenter.Base;
using UI.ShadedElements.Presenter.Targets.Base;
using UnityEngine;
using Zenject;

namespace UI.ShadedElements.Presenter
{
    public class ShadedFloatingWindowsPresenter : ShadedElementsPresenterBase, IShadedFloatingWindowsPresenter
    {
        private IDictionary<ITarget, Func<bool, UniTask>> _targets;
        private Func<bool, UniTask> _onSizeRestoredAsync;
        private Tween _fadeAnimation;
        private CancellationTokenSource _disposeCancellationTokenSource;
        
        [Inject]
        private void Initialize()
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
            
            if (_targets.Count == 1)
                DoFadingIn();
            
            return true;
        }
        
        public bool Remove(IFocusCapable target)
        {
            if (_disposeCancellationTokenSource.IsCancellationRequested || target == null || !_targets.TryGetValue(target, out var onUnfocusedAsync))
                return false;
            
            _targets.Remove(target);
            
            RemoveTargetAsync(target, onUnfocusedAsync).Forget();
            
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
            
            target.Hidden += OnTargetDisabled;
            target.Unloaded += OnTargetDisabled;
            
            onAdded?.Invoke();
        }
        
        private async UniTaskVoid RemoveTargetAsync(ITarget target, Func<bool, UniTask> onRemovedAsync, bool forcibly = false)
        {
            target.Hidden -= OnTargetDisabled;
            target.Unloaded -= OnTargetDisabled;
            
            if (onRemovedAsync != null)
            {
                if (!forcibly)
                {
                    await onRemovedAsync.Invoke(false);
                    
                    if (!_disposeCancellationTokenSource.IsCancellationRequested && !_targets.ContainsKey(target))
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
                if (!forcibly && _targets.Count == 0)
                {
                    try
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(AnimationDuration), cancellationToken: _disposeCancellationTokenSource.Token);
                        
                        if (!_targets.ContainsKey(target))
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
                    
                    if (_targets.Count == 0) 
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
                RemoveTargetAsync(target, onUnfocusedAsync, true).Forget();
            
            if (_targets.Count == 0)
            {
                DoFadingReset(true);
                ResetImageToOriginalState();
            }
        }
    }
}