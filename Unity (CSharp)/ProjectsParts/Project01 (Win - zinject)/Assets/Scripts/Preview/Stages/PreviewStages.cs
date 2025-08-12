using System;
using Core.Network.Http;
using UnityEngine;
using Zenject;

namespace Preview.Stages
{
    [DisallowMultipleComponent]
    public class PreviewStages : MonoBehaviour, IPreviewStages, IDisposable
    {
        [SerializeField] private PreviewSnapshotMaker _snapshotMaker;
        [SerializeField] private PreviewPublisher _publisher;
        
        public IPreviewSnapshotMaker SnapshotMaker => _snapshotMaker;
        public IPreviewPublisher Publisher => _publisher;
        
        public bool Visible => gameObject.activeSelf;
        
        [Inject]
        private void Initialize(IHttpRequestAsync requestAsync) => _publisher.Initialize(requestAsync);
        
        public bool Show()
        {
            gameObject.SetActive(true);
            return true;
        }

        public bool Hide()
        {
            gameObject.SetActive(false);
            return true;
        }

        public void Dispose() => _publisher.Dispose();
    }
}