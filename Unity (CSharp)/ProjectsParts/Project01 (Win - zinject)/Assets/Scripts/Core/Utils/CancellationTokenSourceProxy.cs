using System;
using System.Threading;

namespace Core.Utils
{
    public class CancellationTokenSourceProxy : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        
        public bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;
        public bool IsDisposed { get; private set; }
        
        public CancellationTokenSource Original => _cancellationTokenSource;
        public CancellationToken Token => _cancellationTokenSource.Token;
        
        public CancellationTokenSourceProxy(CancellationTokenSource cancellationTokenSource) => 
            _cancellationTokenSource = cancellationTokenSource;
        
        public CancellationTokenSourceProxy() => 
            _cancellationTokenSource = new CancellationTokenSource();
        
        public CancellationTokenSourceProxy(TimeSpan delay) => 
            _cancellationTokenSource = new CancellationTokenSource(delay);
        
        public CancellationTokenSourceProxy(int millisecondsDelay) => 
            _cancellationTokenSource = new CancellationTokenSource(millisecondsDelay);
        
        public void Cancel() => _cancellationTokenSource.Cancel();
        public void Cancel(bool throwOnFirstException) => _cancellationTokenSource.Cancel(throwOnFirstException);
        
        public void CancelAfter(TimeSpan delay) => _cancellationTokenSource.CancelAfter(delay);
        public void CancelAfter(int millisecondsDelay) => _cancellationTokenSource.CancelAfter(millisecondsDelay);
        
        public void Dispose()
        {
            if (IsDisposed)
                return;
            
            _cancellationTokenSource.Dispose();
            IsDisposed = true;
        }
    } 
}