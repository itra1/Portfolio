using ScreenStreaming.Sender.Source;
using ScreenStreaming.Sender.Source.Factory;
using Unity.RenderStreaming;
using UnityEngine;

namespace ScreenStreaming.Sender
{
    public class ApplicationVideoStreamSender : VideoStreamSender, IApplicationVideoStreamSender
    {
        private IApplicationVideoStreamSourceFactory _factory;
        private IApplicationVideoStreamSource _source;
        private WaitForCreateTrack _instruction;
        
        public void Initialize(IApplicationVideoStreamSourceFactory factory)
        {
            _factory = factory;
            OnStoppedStream += OnStreamStopped;
        }
        
        public void SetStreamRect(RectTransform rectTransform)
        {
            if (_source == null)
                _instruction = CreateTrack();
            
            _source?.SetStreamRect(rectTransform);
        }

        public void UpdateStreamRect(RectTransform rectTransform)
        {
            if (_source == null)
                _instruction = CreateTrack();
            
            _source?.UpdateStreamRect(rectTransform);
        }
        
        public void RemoveStreamRect(RectTransform rectTransform) => _source?.RemoveStreamRect(rectTransform);
        
        public void Dispose()
        {
            OnStoppedStream -= OnStreamStopped;

            _instruction = null;
            
            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
            
            _factory = null;
            
            Destroy(gameObject);
        }
        
        public override WaitForCreateTrack CreateTrack()
        {
            _instruction = null;
            
            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
            
            _source = _factory.Create(this);
            
            return _source.CreateTrack();
        }

        private void OnStreamStopped(string id)
        {
            _instruction = null;
            
            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
        }
    }
}