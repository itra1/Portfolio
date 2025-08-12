using System;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.ScreenModes;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Network.Socket.Packets.Incoming.States.Data;
using Core.User.Installation;
using Cysharp.Threading.Tasks;
using Debug = Core.Logging.Debug;

namespace Core.Recovery
{
    public class PreviousSessionRecovery : IPreviousSessionRecovery, IDisposable
    {
        private readonly IUserInstallation _installation;
        private readonly IMaterialDataStorage _materials;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        private bool? _isStatusSet;
        
        public PreviousSessionRecovery(IUserInstallation installation, IMaterialDataStorage materials)
        {
	        _installation = installation;
            _materials = materials;
            _disposeCancellationTokenSource = new CancellationTokenSource();
        }
        
        public void HandleStateData(IncomingStateData data)
        {
            switch (data.CurrentScreen)
            {
                case ScreenModeName.Desktop:
                {
                    RestoreDesktop(data.Desktop);
                    break;
                }
                case ScreenModeName.Presentation:
                {
                    RestorePresentation(data.Presentation);
                    break;
                }
                case ScreenModeName.Status:
                {
	                RestoreStatusAsync(data.Status).Forget();
	                break;
                }
	            default:
                {
	                MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.None, EnumMessageDelay.NEXT_UPDATE);
	                break;
                }
            }
        }
        
        public void SetDefaultDesktop()
        {
	        var defaultDesktopId = _installation.DefaultDesktopId;
	        
	        if (defaultDesktopId != null)
		        MessageDispatcher.SendMessage(this, MessageType.DesktopSet, defaultDesktopId.Value, EnumMessageDelay.NEXT_UPDATE);
	        else
		        MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.None, EnumMessageDelay.NEXT_UPDATE);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.StatusSetCompleted, OnStateSetCompleted);
            
            if (_disposeCancellationTokenSource is { IsCancellationRequested: false })
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
        }
        
        private void RestoreDesktop(DesktopState state)
        {
            var desktopId = state.DesktopId;
            
            if (desktopId != null) 
                MessageDispatcher.SendMessage(this, MessageType.DesktopSet, desktopId, EnumMessageDelay.NEXT_UPDATE);
        }
        
        private void RestorePresentation(PresentationState state)
        {
            if (state.EpisodeId == 0)
                MessageDispatcher.SendMessage(this, MessageType.PresentationSet, state.PresentationId, EnumMessageDelay.NEXT_UPDATE);
            else
                MessageDispatcher.SendMessage(this, MessageType.PresentationEpisodeSet, new PresentationData(state), EnumMessageDelay.NEXT_UPDATE);
        }
        
        private async UniTaskVoid RestoreStatusAsync(StatusState state)
        {
            _isStatusSet = null;
            
            MessageDispatcher.AddListener(MessageType.StatusSetCompleted, OnStateSetCompleted);
            
            MessageDispatcher.SendMessage(this, MessageType.StatusSet, state.ActiveStatusId, EnumMessageDelay.NEXT_UPDATE);
            
            if (_isStatusSet == null)
            {
                try
                {
                    await UniTask.WaitUntil(() => _isStatusSet != null, cancellationToken: _disposeCancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    return;
                }
            }
            
            if (_isStatusSet == null)
                return;
            
            if (!_isStatusSet.Value)
            {
                _isStatusSet = null;
                return;
            }
            
            _isStatusSet = null;
            
            var activeMaterials = state.ActiveMaterials;
            
            if (activeMaterials is { Length: > 0 })
            {
                foreach (var activeMaterialId in activeMaterials)
                {
                    var areaMaterial = _materials.Get<StatusContentAreaMaterialData>(activeMaterialId);
                    
                    if (areaMaterial == null)
                        continue;
                    
                    var data = new StatusTabData
                    {
                        AreaId = areaMaterial.Id,
                        MaterialId = areaMaterial.StatusContent.ActiveMaterialId,
                        ColumnIndex = areaMaterial.Column - 1,
                        ContentId = areaMaterial.StatusContent.Id,
                    };
                    
                    MessageDispatcher.SendMessage(this, MessageType.StatusTabActive, data, EnumMessageDelay.NEXT_UPDATE);
                }
            }
        }
        
        private void OnStateSetCompleted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.StatusSetCompleted, OnStateSetCompleted);
            _isStatusSet = (bool) message.Data;
        }
    }
}