using System;
using Base.Presenter;
using com.ootii.Messages;
using Core.Messages;
using UI.FullScreen.Presenter.Targets.Base;
using UnityEngine;
using Zenject;

namespace UI.FullScreen.Presenter
{
    public class WindowFullScreenPresenter : PresenterBase, IWindowFullScreenToggle, IDisposable
    {
        private IFullScreenCapable _currentTarget;
        
        [Inject]
        private void Initialize()
        {
            SetName("WindowFullScreen");
            AlignToParent();
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }
        
        public bool IsInFullScreenMode(IFullScreenCapable target) => target.RectTransform.parent == Content;
        
        public void Toggle(IFullScreenCapable target)
        {
            if (IsInFullScreenMode(target))
            {
                RestoreParent();
            }
            else
            {
                if (_currentTarget != null)
                    RestoreParent();
                
                SetParentFor(target, Content);
                SetCurrentTarget(target);
            }
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_currentTarget != null)
                RestoreParent();
        }
        
        private void SetParentFor(IFullScreenCapable target, Transform parent)
        {
            target.RectTransform.SetParent(parent);
            target.AlignToParent();
        }
        
        private void SetCurrentTarget(IFullScreenCapable target)
        {
            _currentTarget = target;
            _currentTarget.Hidden += OnTargetDisabled;
            _currentTarget.Unloaded += OnTargetDisabled;
        }
        
        private void ClearCurrentTarget()
        {
            _currentTarget.Hidden -= OnTargetDisabled;
            _currentTarget.Unloaded -= OnTargetDisabled;
            _currentTarget = null;
        }
        
        private void RestoreParent()
        {
            SetParentFor(_currentTarget, _currentTarget.OriginalParent);
            ClearCurrentTarget();
        }
        
        private void OnTargetDisabled(object sender, EventArgs args) => RestoreParent();
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            AlignToParent();
            
            if (!gameObject.activeSelf) 
                Show();
        }
    }
}