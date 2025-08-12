using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Configs;
using Core.Configs.Consts;
using Core.Materials.Data;
using Core.Messages;
using Core.Options;
using Cysharp.Threading.Tasks;
using Preview.Stages;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Preview
{
    public class PreviewProvider : IPreviewProvider, IDisposable
    {
        private readonly IApplicationOptions _options;
        private readonly IConfig _config;
        private readonly IPreviewStages _stages;
        private readonly IPreviewImageEncoder _encoder;
        private readonly ISet<AreaMaterialData> _inProgress;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool Enabled { get; private set; }
        
        public PreviewProvider(IApplicationOptions options,
            IConfig config,
            IPreviewStages stages,
            IPreviewImageEncoder encoder)
        {
            _options = options;
            _config = config;
            _stages = stages;
            _encoder = encoder;
            _inProgress = new HashSet<AreaMaterialData>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            ReadOptions();
        }
        
        public async UniTaskVoid MakePreviewAsync(AreaMaterialData areaMaterial,
            RectTransform rectTransform,
            CancellationToken cancellationToken)
        {
            if (!Enabled)
                return;
            
            if (areaMaterial == null)
            {
                Debug.LogError("Area material is null when trying to make a preview");
                return;
            }
            
            if (rectTransform == null)
            {
                Debug.LogError("Rect transform is null when trying to make a preview");
                return;
            }
            
            if (!_inProgress.Add(areaMaterial))
                return;
            
            CancellationTokenSource mergedCancellationTokenSource = null;
            
            try
            {
                mergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposeCancellationTokenSource.Token,
                    cancellationToken);
                
                cancellationToken = mergedCancellationTokenSource.Token;
                
                var texture = await _stages.SnapshotMaker.MakeAsync(rectTransform, cancellationToken, 500f);
                
                if (texture == null)
                    return;
                
                var bytes = await _encoder.EncodeAsync(texture, cancellationToken);
                
                if (bytes == null || bytes.Length == 0)
                {
                    Debug.LogError("Encoded image bytes are null or empty when trying to make a preview");
                    return;
                }
                
                if (bytes is { Length: > 0 })
                    _stages.Publisher.AttemptToPublishBasedOnQueue(areaMaterial, bytes);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
            finally
            {
                mergedCancellationTokenSource?.Dispose();
                _inProgress.Remove(areaMaterial);
            }
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
            
            if (_stages.Visible)
                _stages.Hide();
            
            _inProgress.Clear();
        }
        
        private void AttemptToParseEnableState()
        {
            if (_config.TryGetValue(ConfigKey.PreviewGenerate, out var rawValue) && int.TryParse(rawValue, out var value))
                Enabled = value > 0;
            
            _stages.Show();
        }
        
        private void ReadOptions()
        {
            if (_options.IsPreviewEnabled != null)
            {
                Enabled = _options.IsPreviewEnabled.Value;
                
                _stages.Show();
            }
            else
            {
                if (!_config.IsLoaded)
                    MessageDispatcher.AddListener(MessageType.ConfigLoad, OnConfigLoaded);
                else
                    AttemptToParseEnableState();
            }
        }
        
        private void OnConfigLoaded(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.ConfigLoad, OnConfigLoaded);
            AttemptToParseEnableState();
        }
    }
}