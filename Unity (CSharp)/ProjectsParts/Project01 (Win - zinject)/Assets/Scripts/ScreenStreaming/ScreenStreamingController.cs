using System;
using System.Collections.Generic;
using System.Threading;
using Base;
using Base.Presenter;
using com.ootii.Messages;
using Core.Configs;
using Core.Messages;
using Core.Options;
using Cysharp.Threading.Tasks;
using ScreenStreaming.Parameters;
using ScreenStreaming.Sender;
using ScreenStreaming.Sender.Factory;
using UI.Canvas.Presenter;
using Unity.RenderStreaming;
using UnityEngine;
using WebSocketSharp;
using Debug = Core.Logging.Debug;

namespace ScreenStreaming
{
    public class ScreenStreamingController : IScreenStreamingController, IDisposable
    {
        private readonly IConfig _config;
        private readonly IApplicationOptions _options;
        private readonly IScreenStreamingParameters _parameters;
        private readonly IApplicationVideoStreamSenderFactory _factory;
        private readonly IRectTransformable _root;
        private readonly Dictionary<ulong, IRenderStreamingCapable> _targets;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        private IApplicationVideoStreamSender _sender;
        
        public ScreenStreamingController(IConfig config,
            IApplicationOptions options,
            IScreenStreamingParameters parameters, 
            IApplicationVideoStreamSenderFactory factory, 
            ICanvasPresenter root)
        {
            _config = config;
            _options = options;
            _parameters = parameters;
            _factory = factory;
            _root = root;
            _targets = new Dictionary<ulong, IRenderStreamingCapable>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            MessageDispatcher.AddListener(MessageType.UserInstallationPreloadComplete, OnUserInstallationPreloadCompleted);
        }
        
        public void Add(ulong areaMaterialId, IRenderStreamingCapable target)
        {
            if (!_parameters.IsEnabled) 
                return;

            if (_targets.ContainsKey(areaMaterialId))
            {
                Debug.LogWarning($"Screen streaming already contains area material with id {areaMaterialId}");
                return;
            }
            
            if (target == null)
            {
                Debug.LogError($"Target is null when trying to add it to screen streaming by area material with id {areaMaterialId}");
                return; 
            }
            
            target.Resized += OnTargetResized;
            _targets.Add(areaMaterialId, target);
        }

        public void Remove(ulong areaMaterialId)
        {
            if (!_parameters.IsEnabled) 
                return;
            
            if (_targets.TryGetValue(areaMaterialId, out var target))
            {
                target.Resized -= OnTargetResized;
                
                _targets.Remove(areaMaterialId);
                
                try
                {
                    _sender?.RemoveStreamRect(target.RectTransform);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                Debug.LogWarning($"Screen streaming does not yet contain area material with id {areaMaterialId}");
            }
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            MessageDispatcher.RemoveListener(MessageType.UserInstallationPreloadComplete, OnUserInstallationPreloadCompleted);
            MessageDispatcher.RemoveListener(MessageType.StreamingAreaSelect, OnStreamingAreaSelected);
            
            if (_disposeCancellationTokenSource is { IsCancellationRequested: false })
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
            
            if (!_parameters.IsEnabled) 
                return;
            
            var signaling = SignalingManager.Instance;
            
            if (signaling != null && signaling.Running)
                signaling.Stop();
            
            try
            {
                _sender.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _sender = null;
            }
        }

        private void AttemptToStart()
        {
            if (!_parameters.IsEnabled)
                return;
            
            var serverUrl = _parameters.ServerUrl;
            
            if (string.IsNullOrEmpty(serverUrl))
            {
                Debug.LogError("Screen streaming cannot start because the server URL is missing");
                return;
            }
            
            _sender ??= _factory.Create(_root.RectTransform);
            
            var signaling = SignalingManager.Instance;

            if (signaling == null)
            {
                Debug.LogError("Signaling manager instance is null");
                return;
            }
            
            if (signaling.Running)
                return;
            
            MessageDispatcher.AddListener(MessageType.StreamingAreaSelect, OnStreamingAreaSelected);
            
            signaling.RunCustomServer(serverUrl, _options.IsRenderStreamingStunUsing);
            
            if (_options.IsManagersLogEnabled)
                Debug.Log($"Signaling manager has been started. URL: {serverUrl}");
        }
        
        private async UniTaskVoid SetStreamRectAsync(RectTransform rectTransform)
        {
            try
            {
                if (!rectTransform.gameObject.activeInHierarchy)
                {
                    await UniTask.WaitUntil(() => rectTransform.gameObject.activeInHierarchy,
                        cancellationToken: _disposeCancellationTokenSource.Token);
                }
                
                _sender.SetStreamRect(rectTransform);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void OnTargetResized(IRectTransformable target)
        {
            if (!_parameters.IsEnabled)
                return;
            
            _sender?.UpdateStreamRect(target.RectTransform);
        }

        private void OnStreamingAreaSelected(IMessage message)
        {
            if (!_parameters.IsEnabled)
                return;
            
            var areaMaterialId = (ulong) message.Data;
            
            var signaling = SignalingManager.Instance;
            
            if (signaling == null || !signaling.Running)
            {
                Debug.LogError($"Signaling manager has not yet been started when trying to select a streaming area with area material id {areaMaterialId}");
                return;
            }
            
            if (_sender == null)
            {
                Debug.LogError($"Stream sender has not yet been created when trying to select a streaming area with area material id {areaMaterialId}");
                return;
            }
            
            if (!_targets.TryGetValue(areaMaterialId, out var target))
            {
                Debug.LogError($"Area material id {areaMaterialId} is missing when trying to select a streaming area");
                return;
            }
            
            SetStreamRectAsync(target.RectTransform).Forget();
        }
        
        private void OnConfigLoaded(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            
            if (!_options.ServerToken.IsNullOrEmpty())
                AttemptToStart();
        }

        private void OnUserInstallationPreloadCompleted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.UserInstallationPreloadComplete, OnUserInstallationPreloadCompleted);
            
            if (_config.IsLoaded)
                AttemptToStart();
            else
                MessageDispatcher.AddListener(MessageType.ConfigLoad, OnConfigLoaded);
        }
    }
}