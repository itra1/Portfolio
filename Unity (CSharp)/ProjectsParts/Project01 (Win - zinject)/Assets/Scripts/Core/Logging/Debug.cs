using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Logging
{
    public static class Debug
    {
        private static readonly ConcurrentQueue<(LogType, object)> _buffer;
        
        private static CancellationTokenSource _updateCancellationTokenSource;
        
        static Debug()
        {
            _buffer = new ConcurrentQueue<(LogType, object)>();
            UpdateAsync().Forget();
        }
        
        public static void Log(object message)
        {
            if (Thread.CurrentThread.IsBackground && _updateCancellationTokenSource != null)
                _buffer.Enqueue((LogType.Log, message));
            else
                UnityEngine.Debug.Log(message);
        }
        
        public static void LogWarning(object message)
        {
            if (Thread.CurrentThread.IsBackground && _updateCancellationTokenSource != null)
                _buffer.Enqueue((LogType.Warning, message));
            else
                UnityEngine.Debug.LogWarning(message);
        }
        
        public static void LogError(object message)
        {
            if (Thread.CurrentThread.IsBackground && _updateCancellationTokenSource != null)
                _buffer.Enqueue((LogType.Error, message));
            else
                UnityEngine.Debug.LogError(message);
        }
        
        public static void LogException(Exception exception)
        {
            if (Thread.CurrentThread.IsBackground && _updateCancellationTokenSource != null)
                _buffer.Enqueue((LogType.Exception, exception));
            else
                UnityEngine.Debug.LogException(exception);
        }
        
        public static void LogAssertion(object message)
        {
            if (Thread.CurrentThread.IsBackground && _updateCancellationTokenSource != null)
                _buffer.Enqueue((LogType.Assert, message));
            else
                UnityEngine.Debug.LogAssertion(message);
        }
        
        public static void Dispose()
        {
            if (_updateCancellationTokenSource is { IsCancellationRequested: false })
            {
                _updateCancellationTokenSource.Cancel();
                _updateCancellationTokenSource.Dispose();
                _updateCancellationTokenSource = null;
            }
            
            _buffer.Clear();
        }
        
        private static async UniTaskVoid UpdateAsync()
        {
            _updateCancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                var cancellationToken = _updateCancellationTokenSource.Token;
                
                while (_updateCancellationTokenSource != null)
                {
                    if (_buffer.TryDequeue(out var item))
                    {
                        switch (item.Item1)
                        {
                            case LogType.Log:
                                UnityEngine.Debug.Log(item.Item2);
                                break;
                            case LogType.Warning:
                                UnityEngine.Debug.LogWarning(item.Item2);
                                break;
                            case LogType.Error:
                                UnityEngine.Debug.LogError(item.Item2);
                                break;
                            case LogType.Exception:
                                UnityEngine.Debug.LogException((Exception) item.Item2);
                                break;
                            case LogType.Assert:
                                UnityEngine.Debug.LogAssertion(item.Item2);
                                break;
                        }
                    }
                    
                    if (_updateCancellationTokenSource != null)
                        await UniTask.NextFrame(cancellationToken);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                UnityEngine.Debug.LogException(exception);
            }
        }
    }
}