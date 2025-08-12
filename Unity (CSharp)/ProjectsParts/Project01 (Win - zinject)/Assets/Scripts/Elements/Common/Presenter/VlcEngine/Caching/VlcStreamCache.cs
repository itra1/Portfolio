using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Windows.Camera.Data;
using Core.Elements.Windows.Video.Data.Utils;
using Core.Materials.Storage;
using Core.Messages;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.VlcEngine.Factory;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Elements.Common.Presenter.VlcEngine.Caching
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class VlcStreamCache : MonoBehaviour, IVlcStreamCache, IDisposable
    {
        private IMaterialDataStorage _materials;
        private IVlcPlayerFactory _playerFactory;
        private IDictionary<ulong, IVlcStreamCacheItem> _itemsById;

        private RectTransform _rectTransform;
        private CancellationTokenSourceProxy _disposeCancellationTokenSource;
        
        private RectTransform RectTransform => 
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        [Inject]
        private void Initialize(IMaterialDataStorage materials, IVlcPlayerFactory playerFactory)
        {
            _materials = materials;
            _playerFactory = playerFactory;
            _itemsById = new Dictionary<ulong, IVlcStreamCacheItem>();
            
            MessageDispatcher.AddListener(MessageType.CameraAdd, OnCameraAdded);
            MessageDispatcher.AddListener(MessageType.CameraRemove, OnCameraRemoved);
            MessageDispatcher.AddListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.CameraAdd, OnCameraAdded);
            MessageDispatcher.RemoveListener(MessageType.CameraRemove, OnCameraRemoved);
            MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
            
            if (_disposeCancellationTokenSource != null)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
                _disposeCancellationTokenSource = null;
            }
            
            foreach (var item in _itemsById.Values)
                item.Dispose();
            
            _itemsById.Clear();
            
            _itemsById = null;
            _playerFactory = null;
            _materials = null;
        }
        
        public bool Contains(ulong id) => _itemsById.ContainsKey(id);
        
        public bool TryGet(ulong id, IVlcStreamReceiver receiver, out IVlcStream stream)
        {
            if (!_itemsById.TryGetValue(id, out var item))
            {
                stream = null;
                return false;
            }
            
            if (!item.TryGetStream(out stream))
                return false;
            
            var hasAnyReceiverBefore = item.HasAnyReceiver();
            
            if (item.TryAddReceiver(receiver) && !hasAnyReceiverBefore)
                stream.Show();
            
            return true;
        }
        
        public bool TryRemove(ulong id, IVlcStreamReceiver receiver)
        {
            if (!_itemsById.TryGetValue(id, out var item))
                return false;
            
            if (item.TryGetStream(out var stream) && item.TryRemoveReceiver(receiver))
            {
                if (!item.HasAnyReceiver())
                    stream.Hide();
                
                return true;
            }
            
            Debug.LogError($"Invalid VLC stream instance detected while trying to remove from cache by id {id}");
            return false;
        }
        
        private void TryAdd(CameraMaterialData material)
        {
            var id = material.Id;
            
            if (Contains(id))
                return;
            
            var stream = _playerFactory.CreateStream(RectTransform, material, material.CameraUrl);
            
            if (stream == null)
            {
                Debug.LogError($"An attempt was detected to add a null VLC stream reference to the cache by id {id}");
                return;
            }
            
            _itemsById.Add(id, new VlcStreamCacheItem(stream));
            
            stream.SetParent(RectTransform);
            stream.AlignToParent();
            stream.Play();
        }

        private void TryRemove(ulong id)
        {
            if (!_itemsById.TryGetValue(id, out var item))
                return;
            
            item.Dispose();
            
            _itemsById.Remove(id);
        }
        
        private void OnCameraAdded(IMessage message)
        {
            if (message.Data is not CameraMaterialData material)
                return;
            
            if (!material.IsMultiple())
                return;
            
            TryAdd(material);
        }

        private void OnCameraRemoved(IMessage message)
        {
            if (message.Data is not CameraMaterialData material)
                return;
            
            if (!material.IsMultiple())
                return;
            
            TryRemove(material.Id);
        }
        
        private async UniTaskVoid PreloadMaterialsAsync()
        {
            _disposeCancellationTokenSource = new CancellationTokenSourceProxy();
            
            try
            {
                var cancellationToken = _disposeCancellationTokenSource.Token;
                
                IEnumerable<CameraMaterialData> materials;
                
                await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                {
                    await UniTask.SwitchToThreadPool();
                    cancellationToken.ThrowIfCancellationRequested();
                    materials = _materials.GetList<CameraMaterialData>();
                }
                
                foreach (var material in materials)
                {
                    if (!material.IsMultiple())
                        continue;
                    
                    TryAdd(material);
                    
                    await UniTask.NextFrame(cancellationToken);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
                
                if (_disposeCancellationTokenSource is { IsDisposed: false })
                {
                    _disposeCancellationTokenSource.Dispose();
                    _disposeCancellationTokenSource = null;
                }
            }
        }

        private void OnApplicationLoadingCompleted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
            PreloadMaterialsAsync().Forget();
        }
    }
}