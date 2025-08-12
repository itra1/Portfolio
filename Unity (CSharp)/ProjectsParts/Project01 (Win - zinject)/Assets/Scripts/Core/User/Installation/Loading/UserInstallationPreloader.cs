using System;
using System.Threading;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.User.Installation.Parsing;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.User.Installation.Loading
{
    public class UserInstallationPreloader : IUserInstallationPreloader, IDisposable
    {
        private readonly IHttpRequestAsync _requestAsync;
        private readonly IUserInstallationParser _parser;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public UserInstallationPreloader(IHttpRequestAsync requestAsync, IUserInstallationParser parser)
        {
            _requestAsync = requestAsync;
            _parser = parser;
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            MessageDispatcher.AddListener(MessageType.AuthorizationConfirm, OnAuthorizationConfirmed);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AuthorizationConfirm, OnAuthorizationConfirmed);
            
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
        }
        
        private async UniTaskVoid StartLoadingAsync()
        {
            var cancellationToken = _disposeCancellationTokenSource.Token;
            
            try
            {
                var response = await _requestAsync.RequestAsync(RestApiUrl.UserInstallation, cancellationToken);
                
                if (!response.IsFailed)
                {
                    var rawData = response.Result;
                    
                    if (string.IsNullOrEmpty(rawData))
                    {
                        await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                        {
                            await UniTask.SwitchToThreadPool();
                            
                            cancellationToken.ThrowIfCancellationRequested();
                            
                            _parser.Parse(rawData);
                        }
                    }
                }
                else
                {
                    Debug.LogError(response.ErrorMessage);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
            }
            
            MessageDispatcher.SendMessage(MessageType.UserInstallationPreloadComplete);
        }

        private void OnAuthorizationConfirmed(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AuthorizationConfirm, OnAuthorizationConfirmed);
            StartLoadingAsync().Forget();
        }
    }
}