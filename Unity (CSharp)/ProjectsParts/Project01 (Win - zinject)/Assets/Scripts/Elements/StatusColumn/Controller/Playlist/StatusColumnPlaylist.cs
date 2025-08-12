using System;
using System.Linq;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.StatusColumn.Data;
using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Video.Data;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.StatusTabs.Controller;
using UnityEngine;

namespace Elements.StatusColumn.Controller.Playlist
{
    public class StatusColumnPlaylist : IStatusColumnPlaylist
    {
        private readonly StatusContentMaterialData _statusContent;
        private readonly IApplicationOptions _options;
        private readonly IHttpRequestAsync _requestAsync;
        private readonly IMaterialDataStorage _materials;
        private readonly IOutgoingStateController _outgoingState;
        private readonly IStatusTabsActionPerformer _actionPerformer;
        
        private CancellationTokenSourceProxy _cancellationTokenSource;
        
        private int _currentAreaMaterialIndex;
        private int _currentTrackIndex;
        
        public bool IsPlaying { get; private set; }
        
        public StatusColumnPlaylist(StatusContentMaterialData statusContent,
            IApplicationOptions options,
            IHttpRequestAsync requestAsync,
            IMaterialDataStorage materials,
            IOutgoingStateController outgoingState,
            IStatusTabsActionPerformer actionPerformer)
        {
            _statusContent = statusContent;
            _options = options;
            _requestAsync = requestAsync;
            _materials = materials;
            _outgoingState = outgoingState;
            _actionPerformer = actionPerformer;
        }

        public void Play()
        {
            if (IsPlaying)
            {
                Debug.LogWarning("An attempt was detected to start playing a playlist which had already been playing");
                return;
            }
            
            if (!AttemptToActualizeCurrentIndices())
                return;
            
            var areaMaterial = _actionPerformer.AreaMaterials[_currentAreaMaterialIndex];
            var materialId = areaMaterial.MaterialId;
            
            if (materialId == null)
                return;
            
            IsPlaying = true;
            
            _outgoingState.Context.AddStatusPlaylist(_statusContent.Column, _statusContent.StatusId);
            _outgoingState.PrepareToSendIfAllowed();
            
            MessageDispatcher.AddListener(MessageType.StatusTabActive, OnStatusTabActive);
            
            PlayCurrentTrackAsync().Forget();
        }
        
        public void Stop()
        {
            if (!IsPlaying)
            {
                Debug.LogWarning("An attempt was detected to stop playing a playlist which had not yet been playing");
                return;
            }
            
            MessageDispatcher.RemoveListener(MessageType.StatusTabActive, OnStatusTabActive);
            
            CancelIfCurrentTrackPlaying();
            
            _outgoingState.Context.RemoveStatusPlaylist(_statusContent.Column, _statusContent.StatusId);
            _outgoingState.PrepareToSendIfAllowed();
            
            _currentAreaMaterialIndex = 0;
            _currentTrackIndex = 0;
            
            IsPlaying = false;
        }
        
        public void Dispose() => Stop();
        
        private void CancelIfCurrentTrackPlaying()
        {
            if (_cancellationTokenSource == null)
                return;
            
            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();
            
            if (!_cancellationTokenSource.IsDisposed)
                _cancellationTokenSource.Dispose();
            
            _cancellationTokenSource = null;
        }
        
        private bool AttemptToActualizeCurrentIndices()
        {
            _currentAreaMaterialIndex = 0;
            _currentTrackIndex = 0;
            
            var contentOrder = _statusContent.ContentOrder;
            var areaMaterials = _actionPerformer.AreaMaterials;
            
            if (contentOrder == null || areaMaterials == null)
                return false;
            
            var contentOrderLength = contentOrder.Count;
            var areaMaterialsCount = areaMaterials.Count;
            
            if (contentOrderLength == 0 || areaMaterialsCount == 0)
                return false;
            
            var activeAreaMaterial = _actionPerformer.ActiveAreaMaterial;
            
            ulong activeAreaMaterialId;
            
            if (activeAreaMaterial == null)
            {
                _currentTrackIndex = 0;
                
                activeAreaMaterialId = contentOrder[_currentTrackIndex];
                
                for (var i = 0; i < areaMaterialsCount; i++)
                {
                    if (areaMaterials[i].Id != activeAreaMaterialId)
                        continue;
                    
                    _currentAreaMaterialIndex = i;
                    return true;
                }

                return false;
            }
            
            activeAreaMaterialId = activeAreaMaterial.Id;
            
            for (var i = 0; i < contentOrderLength; i++)
            {
                if (contentOrder[i] != activeAreaMaterialId)
                    continue;
                
                _currentTrackIndex = i;
                
                for (var j = 0; j < areaMaterialsCount; j++)
                {
                    if (areaMaterials[j].Id != activeAreaMaterialId)
                        continue;
                    
                    _currentAreaMaterialIndex = j;
                    return true;
                }
            }
            
            _currentAreaMaterialIndex = 0;
            _currentTrackIndex = 0;
            return false;
        }

        private bool IsLoopingAt(ulong areaMaterialId, ulong materialId)
        {
            var states = _materials.Get<WindowMaterialData>(materialId)?.States;
            
            if (states is { Length: > 0 })
            {
                var state = states.FirstOrDefault(state =>
                    state.StateContentId == _statusContent.Id && state.AreaId == areaMaterialId);
                
                return state is VideoState { Loop: true };
            }
            
            return false;
        }
        
        private async UniTask<bool> AttemptToPlayCurrentTrackAsync(CancellationToken cancellationToken)
        {
            if (!AttemptToActualizeCurrentIndices())
                return false;
            
            var areaMaterials = _actionPerformer.AreaMaterials;
            
            if (areaMaterials == null || areaMaterials.Count == 0)
                return false;
            
            var areaMaterial = areaMaterials[_currentAreaMaterialIndex];
            var materialId = areaMaterial.MaterialId;
            
            if (materialId == null)
                return true;
            
            var startTime = DateTime.Now;
            
            var response = await _requestAsync.RequestAsync(string.Format(RestApiUrl.ActiveStatusContentMaterialFormat, _statusContent.Id),
                HttpMethodType.Patch,
                new (string, object)[] { ("activeMaterialId", materialId.Value) },
                cancellationToken);
            
            if (response.IsFailed)
                return true;
            
            if (areaMaterial.AutoPlay is true)
            {
                _actionPerformer.PerformAction(WindowMaterialActionAlias.Reset, materialId.Value);
                _actionPerformer.PerformAction(WindowMaterialActionAlias.Play, materialId.Value);
            }
            
            var timeOut = areaMaterial.TimeOut;
            var delayTimeSpan = TimeSpan.FromSeconds(1.0);
            
            while (timeOut is > 0 && (DateTime.Now - startTime).TotalSeconds < timeOut.Value)
            {
                await UniTask.Delay(delayTimeSpan, cancellationToken: cancellationToken);
                timeOut = areaMaterial.TimeOut;
            }
            
            return !IsLoopingAt(areaMaterial.Id, areaMaterial.Id);
        }
        
        private bool AttemptToActivateNextTab()
        {
            if (!AttemptToActualizeCurrentIndices())
                return false;
            
            var contentOrder = _statusContent.ContentOrder;
            var areaMaterials = _actionPerformer.AreaMaterials;

            if (contentOrder == null || areaMaterials == null)
                return false;
            
            var contentOrderLength = contentOrder.Count;
            var areaMaterialsCount = areaMaterials.Count;
            
            if (contentOrderLength == 0 || areaMaterialsCount == 0)
                return false;
            
            if (_currentTrackIndex < contentOrderLength - 1)
                _currentTrackIndex++;
            else
                _currentTrackIndex = 0;
            
            var activeAreaMaterialId = contentOrder[_currentTrackIndex];
            var isCurrentAreaMaterialIndexFound = false;
            
            for (var i = 0; i < areaMaterialsCount; i++)
            {
                if (areaMaterials[i].Id != activeAreaMaterialId)
                    continue;
                
                _currentAreaMaterialIndex = i;
                isCurrentAreaMaterialIndexFound = true;
                break;
            }

            if (!isCurrentAreaMaterialIndexFound)
                return false;
            
            var areaMaterial = areaMaterials[_currentAreaMaterialIndex];
            var materialId = areaMaterial.MaterialId;
            
            if (materialId == null) 
                return false;
            
            var data = new StatusTabData
            {
                AreaId = areaMaterial.Id,
                MaterialId = materialId.Value,
                ColumnIndex = _statusContent.Column - 1,
                ContentId = _statusContent.Id
            };
            
            MessageDispatcher.SendMessage(this, MessageType.StatusTabActive, data, EnumMessageDelay.IMMEDIATE);
            return true;
        }
        
        private async UniTaskVoid PlayCurrentTrackAsync()
        {
            CancelIfCurrentTrackPlaying();
            
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            try
            {
                var cancellationToken = _cancellationTokenSource.Token;
                
                if (!await AttemptToPlayCurrentTrackAsync(cancellationToken) || !AttemptToActivateNextTab())
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    PlayCurrentTrackAsync().Forget();
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private async UniTaskVoid OnNextTabActivatedAsync()
        {
            try
            {
                await UniTask.NextFrame(_cancellationTokenSource.Token);
                
                CancelIfCurrentTrackPlaying();
                
                if (AttemptToActualizeCurrentIndices())
                    PlayCurrentTrackAsync().Forget();
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void OnStatusTabActive(IMessage message)
        {
            var column = _statusContent.Column;
            
            if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != column)
                return;
            
            var data = (StatusTabData) message.Data;
            
            if (_statusContent.Id != data.ContentId || column != data.ColumnIndex + 1)
                return;
            
            OnNextTabActivatedAsync().Forget();
        }
    }
}