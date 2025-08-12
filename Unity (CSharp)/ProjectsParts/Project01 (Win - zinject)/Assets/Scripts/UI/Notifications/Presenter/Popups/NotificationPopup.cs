using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Debug = Core.Logging.Debug;

namespace UI.Notifications.Presenter.Popups
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class NotificationPopup : MonoBehaviour, INotificationPopup
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _body;
        [SerializeField] private Button _closeButton;
        
        private RectTransform _rectTransform;
        private CancellationTokenSource _cancellationTokenSource;
        private float _activityTime;
        private float _fadeDuration;
        private Tween _moveAnimation;
        
        public event EventHandler Shown;
        public event EventHandler Hidden;
        
        public Vector2 Size => RectTransform.rect.size;
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        private bool Visible => gameObject.activeSelf;
        
        public void SetName(string value) => gameObject.name = value;
        
        public void Initialize(string text, Color color, float activityTime, float fadeDuration)
        {
            _body.text = text;
            _background.color = color;
            _cancellationTokenSource = new CancellationTokenSource();
            _activityTime = activityTime;
            _fadeDuration = fadeDuration;
            
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        
        public void SetParentOnInitialize(RectTransform parent) => RectTransform.SetParent(parent);
        
        public void AlignToParent()
        {
            var rectTransform = RectTransform;
            var sizeDelta = rectTransform.sizeDelta;
            
            rectTransform.ResetAnchors(new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f));
            rectTransform.Reset();
            
            rectTransform.sizeDelta = sizeDelta;
        }
        
        public async UniTask<bool> ShowAsync(Action onStarted = null, Action onFinished = null)
        {
            if (Visible)
                return false;
            
            Shown?.Invoke(this, EventArgs.Empty);
            
            RunTimeOutAsync().Forget();
            
            return await DoFadingInAsync(onStarted, onFinished);
        }
        
        public void MoveTo(Vector2 anchoredPosition, float? duration = null)
        {
            var rectTransform = RectTransform;
            
            if (duration is null or <= 0f)
            {
                rectTransform.anchoredPosition = anchoredPosition;
            }
            else
            {
                if (_moveAnimation != null)
                {
                    duration -= _moveAnimation.position;
                    
                    _moveAnimation.Kill();
                    _moveAnimation = null;
                }
                
                _moveAnimation = rectTransform.DOAnchorPos(anchoredPosition, duration.Value)
                    .SetEase(Ease.InOutCubic)
                    .OnComplete(() => _moveAnimation = null);
            }
        }
        
        public async UniTask<bool> HideAsync(Action onStarted = null, Action onFinished = null)
        {
            try
            {
                if (!Visible)
                    return false;
                
                var result = await DoFadingOutAsync(onStarted, onFinished);
                
                Hidden?.Invoke(this, EventArgs.Empty);
                
                return result;
            }
            catch
            {
                // ignored
            }

            return false;
        }
        
        public void Dispose()
        {
            try
            {
                if (_moveAnimation != null)
                {
                    _moveAnimation.Kill(true);
                    _moveAnimation = null;
                }
                
                if (_cancellationTokenSource is { IsCancellationRequested: false })
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
                
                _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
                _rectTransform = null;
                
                Destroy(gameObject);
            }
            catch
            {
                // ignored
            }
        }
        
        private async UniTask<bool> DoFadingInAsync(Action onStarted = null, Action onFinished = null)
        {
            try
            {
                if (_cancellationTokenSource == null)
                    return false;
                
                gameObject.SetActive(true);
                
                onStarted?.Invoke();
                
                var initialAlpha = _canvasGroup.alpha;
                var duration = _fadeDuration * (1f - initialAlpha);
                
                if (duration <= 0f)
                    return true;
                
                var cancellationToken = _cancellationTokenSource.Token;
                var beginTime = Time.time;
                
                while (Time.time - beginTime < duration)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _canvasGroup.alpha = initialAlpha + (1f - initialAlpha) * ((Time.time - beginTime) / duration);
                    await UniTask.NextFrame(cancellationToken);
                }
                
                _canvasGroup.alpha = 1f;
                
                onFinished?.Invoke();
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
            
            return true;
        }
        
        private async UniTask<bool> DoFadingOutAsync(Action onStarted = null, Action onFinished = null)
        {
            try
            {
                if (_cancellationTokenSource == null)
                    return false;
                
                onStarted?.Invoke();
                
                var initialAlpha = _canvasGroup.alpha;
                var duration = _fadeDuration * initialAlpha;
                
                if (duration <= 0f)
                    return true;
                
                var cancellationToken = _cancellationTokenSource.Token;
                var beginTime = Time.time;
                
                while (Time.time - beginTime < duration)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _canvasGroup.alpha = initialAlpha - initialAlpha * ((Time.time - beginTime) / duration);
                    await UniTask.NextFrame(cancellationToken);
                }
                
                _canvasGroup.alpha = 0f;

                onFinished?.Invoke();

                gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
            
            return true;
        }

        private async UniTaskVoid RunTimeOutAsync()
        {
            try
            {
                if (_cancellationTokenSource != null)
                    await UniTask.Delay(TimeSpan.FromSeconds(_activityTime), cancellationToken: _cancellationTokenSource.Token);
                
                HideAsync().Forget();
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void OnCloseButtonClicked() => HideAsync().Forget();
    }
}