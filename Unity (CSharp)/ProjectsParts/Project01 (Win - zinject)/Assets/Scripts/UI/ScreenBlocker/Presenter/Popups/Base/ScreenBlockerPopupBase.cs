using System;
using DG.Tweening;
using UnityEngine;

namespace UI.ScreenBlocker.Presenter.Popups.Base
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public abstract class ScreenBlockerPopupBase : MonoBehaviour, IScreenBlockerPopup
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _animationDuration;
        
        private RectTransform _rectTransform;
        private Tween _fadeAnimation;
        
        public event Action Shown;
        public event Action Hidden;
        
        public bool Visible => gameObject.activeSelf;
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        private RectTransform Parent => RectTransform.parent as RectTransform;
        
        protected void Show()
        {
            if (!Visible)
                DoFadingIn();
        }
        
        protected void Hide()
        {
            if (Visible)
                DoFadingOut();
        }
        
        public virtual void Dispose()
        {
            DoFadingReset(true);
            
            try
            {
                Destroy(gameObject);
            }
            catch
            {
                // ignored
            }
        }

        private void AlignToParent()
        {
            var rectTransform = RectTransform;
            
            if (rectTransform.rect.width / Parent.rect.width > 0.21f)
                rectTransform.localScale = Vector3.one * 0.21f;
        }
        
        private void DoFadingIn()
        {
            DoFading(1f, 
                () =>
                {
                    gameObject.SetActive(true);
                    AlignToParent();
                    Shown?.Invoke();
                }, 
                () => _fadeAnimation = null);
        }

        private void DoFadingOut()
        {
            DoFading(0f, 
                null,
                () => 
                { 
                    _fadeAnimation = null;
                    
                    try
                    {
                        gameObject.SetActive(false);
                        Hidden?.Invoke();
                    }
                    catch
                    {
                        // ignored
                    }
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
                duration = _animationDuration;
            }
            
            _fadeAnimation = _canvasGroup.DOFade(opacity, duration);
            
            if (onStarted != null)
                _fadeAnimation.OnStart(onStarted);
            
            if (onCompleted != null)
                _fadeAnimation.OnComplete(onCompleted);
        }
    }
}