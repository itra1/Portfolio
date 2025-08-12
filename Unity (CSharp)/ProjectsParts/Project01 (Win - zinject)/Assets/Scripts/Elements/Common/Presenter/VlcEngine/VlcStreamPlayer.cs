using System;
using Core.Elements.Windows.Video.Data.Utils;
using Elements.Common.Presenter.VlcEngine.Utils;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine
{
    public class VlcStreamPlayer : VlcPlayerBase, IVlcStreamPlayer
    {
#if UNITY_EDITOR
        [SerializeField] private VlcStream _streamSource;
#endif
        
        private IVlcStream _stream;
        
        public event Action Playing;
        public event Action Stopped;
        public event Action Displayed;
        
        public bool IsPlaying => _stream is { IsPlaying: true };
        
        public override bool IsDisplayed => _stream is { IsDisplayed: true };
        
        public void Initialize(IVlcStream stream)
        {
#if UNITY_EDITOR
            _streamSource = stream as VlcStream;
#endif
            _stream = stream;
            
            _stream.Playing += OnStreamPlaying;
            _stream.Stopped += OnStreamStopped;
            _stream.Displayed += OnStreamDisplayed;
            _stream.ImageUpdated += OnImageUpdated;
            _stream.SizeAdjusted += OnSizeAdjusted;
            _stream.Disposed += OnStreamDisposed;
            
            if (!_stream.IsInitialized) 
                return;
            
            UpdateImage(_stream.ImageTexture, _stream.ImageMaterial);
            
            base.Initialize();
            
            AdjustSize();
        }
        
        public void Play() => _stream.Play();
        public void Stop() => _stream.Stop();
        public void Refresh() => _stream.Refresh();
        
        public void AdjustSize()
        {
            var texture = _stream.CurrentTexture;
            
            if (texture != null)
                AdjustSize(texture);
        }

        public override void Dispose()
        {
            _stream.Playing -= OnStreamPlaying;
            _stream.Stopped -= OnStreamStopped;
            _stream.Displayed -= OnStreamDisplayed;
            _stream.ImageUpdated -= OnImageUpdated;
            _stream.SizeAdjusted -= OnSizeAdjusted;
            _stream.Disposed -= OnStreamDisposed;
            
            base.Dispose();
        }
        
        private void AdjustSize(Texture texture)
        {
            var sizeDeltaX = Material.GetSizeDeltaX();
            var sizeDeltaY = Material.GetSizeDeltaY();
            
            if (Material.IsFillParentRequired())
                this.ScaleAndCrop(texture);
            else if (Material.IsFillInParentRequired())
                this.ScaleToFit(texture);
            else
                this.StretchToFill();
            
            if (sizeDeltaX != null || sizeDeltaY != null)
                this.ApplySizeDelta(new Vector2(sizeDeltaX ?? 0f, sizeDeltaY ?? 0f));
        }
        
        private void UpdateImage(Texture texture, Material newMaterial)
        {
            if (newMaterial != null)
                Image.material = newMaterial;
            
            if (texture != null)
                Image.texture = texture;
        }

        private void OnStreamPlaying()
        {
            if (!IsDisposed)
                Playing?.Invoke();
        }

        private void OnStreamStopped()
        {
            if (!IsDisposed)
                Stopped?.Invoke();
        }

        private void OnStreamDisplayed()
        {
            if (!IsDisposed)
                Displayed?.Invoke();
        }

        private void OnImageUpdated(Texture texture, Material newMaterial)
        {
            if (!IsDisposed)
                UpdateImage(texture, newMaterial);
        }

        private void OnSizeAdjusted(Texture texture) => AdjustSize(texture);

        private void OnStreamDisposed()
        {
            if (!IsDisposed)
                Dispose();
        }
    }
}