using System;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.Base.Presenter
{
    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup))]
    public abstract class HiddenPresenterBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration;
        
        private CancellationTokenSourceProxy _cancellationTokenSource;
        
        protected void SetInitialOpacity(float value) => _canvasGroup.alpha = value;
        
        public virtual async UniTask<bool> ShowAsync(Action onStarted = null, Action onFinished = null) => 
            await DoFadingInAsync(onStarted, onFinished);
        public virtual async UniTask<bool> HideAsync(Action onStarted = null, Action onFinished = null) => 
            await DoFadingOutAsync(onStarted, onFinished);
        
        public virtual void Unload()
        {
            CancelIfRequired();
            
            try
            {
                _canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        private async UniTask<bool> DoFadingInAsync(Action onStarted = null, Action onFinished = null)
        {
            CancelIfRequired();
            
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            try
            {
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
            finally
            {
                if (_cancellationTokenSource is { IsDisposed: false })
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
            
            return true;
        }
        
        private async UniTask<bool> DoFadingOutAsync(Action onStarted = null, Action onFinished = null)
        {
            CancelIfRequired();
            
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            try
            {
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
            finally
            {
                if (_cancellationTokenSource is { IsDisposed: false })
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
            
            return true;
        }

        private void CancelIfRequired()
        {
            if (_cancellationTokenSource != null)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();
                
                if (!_cancellationTokenSource.IsDisposed)
                    _cancellationTokenSource.Dispose();
                
                _cancellationTokenSource = null;
            }
        }
    }
}