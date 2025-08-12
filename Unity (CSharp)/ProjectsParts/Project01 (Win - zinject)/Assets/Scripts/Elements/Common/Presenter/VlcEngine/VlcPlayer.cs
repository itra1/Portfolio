using System;
using System.Threading;
using Core.Elements.Windows.Video.Data.Utils;
using Core.UI.Audio.Enums;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.VlcEngine.Factory;
using Elements.Common.Presenter.VlcEngine.Textures;
using Elements.Common.Presenter.VlcEngine.Utils;
using LibVLCSharp;
using UI.Audio.Controller;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Common.Presenter.VlcEngine
{
    public class VlcPlayer : VlcPlayerBase, IVlcPlayer
    {
        [SerializeField] private string _url;
        [SerializeField] private Material _videoMaterialBGR;
        
        private IVlcLibraryFactory _libraryFactory;
        private IVlcMediaPlayerFactory _mediaPlayerFactory;
        private IVlcMediaFactory _mediaFactory;
        private IVlcPlayerTexturesFactory _texturesFactory;
        private IAudioController _audio;
        
        private CancellationTokenSource _lateUpdateCancellationTokenSource;
        
        private LibVLC _library;
        private Media _media;
        private MediaPlayer _mediaPlayer;
        private ITexturesBuffer _texturesBuffer;
        private ITextureValidator _textureValidator;
        
        private Material _initialMaterial;
        private Material _newVideoMaterialBGR;
        
        public event Action Playing;
        public event Action Stopped;
        public event Action Displayed;
        
        public bool IsPlaying => _mediaPlayer is { IsPlaying: true };
        
        public Texture CurrentTexture => _texturesBuffer?.RenderTextures[_texturesBuffer.CurrentRenderTextureIndex];

        public void SetUrl(string url) => _url = url;
        
        public void Initialize(IVlcLibraryFactory libraryFactory, 
            IVlcMediaPlayerFactory mediaPlayerFactory, 
            IVlcMediaFactory mediaFactory, 
            IVlcPlayerTexturesFactory texturesFactory,
            IAudioController audio)
        {
            _libraryFactory = libraryFactory;
            _mediaPlayerFactory = mediaPlayerFactory;
            _mediaFactory = mediaFactory;
            _texturesFactory = texturesFactory;
            
            CreateLibrary();
            CreateMedia();
            CreateMediaPlayer();
            
            if (_library == null || _media == null || _mediaPlayer == null) 
                return;
            
            _audio = audio;
            _audio.VolumeChanged += OnVolumeChanged;
            
            _mediaPlayer.SetVolume(Mathf.RoundToInt(_audio.GetVolumeOf(AudioMixerGroupName.Common) * 100));
            
            base.Initialize();
        }
        
        public override bool Show()
        {
            if (!base.Show())
                return false;
            
            LateUpdateAsync().Forget();
            return true;
        }
        
        public void Play() => _mediaPlayer?.Play(_media);
        public void Stop() => _mediaPlayer?.Stop();
        
        public void Refresh()
        {
            if (IsPlaying && IsDisplayed)
                DestroyTextures();
            else
                AdjustSize();
        }
        
        public void AdjustSize()
        {
            if (_texturesBuffer != null)
                AdjustSize(_texturesBuffer.RenderTextures[_texturesBuffer.CurrentRenderTextureIndex]);
        }
        
        public override bool Hide()
        {
            if (!base.Hide())
                return false;
            
            _lateUpdateCancellationTokenSource?.Cancel();
            
            return true;
        }

        public override void Dispose()
        {
            _lateUpdateCancellationTokenSource?.Cancel();
            
            if (_audio != null)
            {
                _audio.VolumeChanged -= OnVolumeChanged;
                _audio = null;
            }
            
            DestroyLibrary();
            DestroyMediaPlayer();
            DestroyMedia();
            DestroyTextures();
            
            _textureValidator = null;

            base.Dispose();
        }
        
        protected virtual void AdjustSize(Texture texture)
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

        private void CreateLibrary() => _library = _libraryFactory.Create();
        private void DestroyLibrary() => _libraryFactory.Destroy(ref _library);
        private void CreateMediaPlayer() => _mediaPlayer = _mediaPlayerFactory.Create(_library, OnMediaPlayerPaying, OnMediaPlayerStopped);
        private void DestroyMediaPlayer() => _mediaPlayerFactory.Destroy(ref _mediaPlayer, OnMediaPlayerPaying, OnMediaPlayerStopped);
        private void CreateMedia() => _media = _mediaFactory.Create(_library, _url);
        private void DestroyMedia() => _mediaFactory.Destroy(ref _media);
        private bool TryCreateTextures(out ITexturesBuffer buffer) => _texturesFactory.TryCreate(_mediaPlayer, out buffer);
        
        private void DestroyTextures()
        {
            IsDisplayed = false;
            
            try
            {
                Image.texture = null;
                
                if (_initialMaterial != null)
                {
                    var currentMaterial = Image.material;
                    
                    Image.material = _initialMaterial;
                    
                    _initialMaterial = null;
                    
                    if (currentMaterial != null)
                        Destroy(currentMaterial);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                if (_newVideoMaterialBGR != null)
                    Destroy(_newVideoMaterialBGR);
            }
            catch (Exception)
            {
                // ignored
            }
            
            _texturesFactory.Destroy(ref _texturesBuffer);
        }
        
        private bool ValidateMediaPlayer() => IsPlaying && Math.Abs(_mediaPlayer.Position + 1.0) > 0.1;

        private async UniTask<bool> ValidateTexturesAsync(CancellationToken cancellationToken)
        {
            if (_texturesBuffer != null) 
                return true;
            
            if (!TryCreateTextures(out _texturesBuffer))
                return false;
            
            _textureValidator = new TextureValidator(_texturesBuffer);
            
            Material material = null;
            
            if (Material.IsBgr())
            {
                if (_newVideoMaterialBGR == null)
                    _newVideoMaterialBGR = Instantiate(_videoMaterialBGR);
                
                material = _newVideoMaterialBGR;
            }
            
            var texture = _texturesBuffer.RenderTextures[_texturesBuffer.CurrentRenderTextureIndex];
            
            UpdateImage(texture, material);
            
            AdjustSize(texture);
            
            await UniTask.WaitUntil(ValidateMediaPlayer, cancellationToken: cancellationToken);
            
            if (_textureValidator == null)
                return false;
            
            IsDisplayed = true;
            
            if (!IsDisposed)
                Displayed?.Invoke();
            
            return true;
        }
        
        private void UpdateTextures()
        {
            if (Material.IsStreamValidationRequired()) 
                return;
            
            if (!ValidateMediaPlayer())
                return;
            
            var texture = _texturesBuffer.Texture;
            
            if (texture == null)
                return;
            
            var texturePtr = _mediaPlayer.GetTexture((uint) texture.width, (uint) texture.height, out var updated);
            
            if (!updated)
                return;
            
            if (texturePtr == IntPtr.Zero)
                return;
            
            texture.UpdateExternalTexture(texturePtr);
            
            var nextRenderTexturesIndex = _texturesBuffer.NextRenderTextureIndex;
            
            var renderTexture = _texturesBuffer.RenderTextures[nextRenderTexturesIndex];
            
            Graphics.Blit(texture, renderTexture, new Vector2(-1.0f, -1.0f), Vector2.zero);
            
            if (!_textureValidator.Validate(renderTexture))
                return;
            
            _texturesBuffer.CurrentRenderTextureIndex = nextRenderTexturesIndex;
            
            UpdateImage(renderTexture);
        }
        
        protected virtual void UpdateImage(Texture texture, Material newMaterial = null)
        {
            if (newMaterial != null)
            {
                var currentMaterial = Image.materialForRendering;
                
                if (currentMaterial != newMaterial)
                {
                    if (_initialMaterial == null)
                    {
                        _initialMaterial = currentMaterial;
                        Image.material = newMaterial;
                    }
                    else
                    {
                        Image.material = newMaterial;
                        Destroy(currentMaterial);
                    }
                }
            }
            
            Image.texture = texture;
        }

        private async UniTaskVoid LateUpdateAsync()
        {
            _lateUpdateCancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                var cancellationToken = _lateUpdateCancellationTokenSource.Token;
                
                while (true)
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
                    
                    if (await ValidateTexturesAsync(cancellationToken))
                        UpdateTextures();
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
            finally
            {
                _lateUpdateCancellationTokenSource.Dispose();
                _lateUpdateCancellationTokenSource = null;
            }
        }
        
        private void OnMediaPlayerPaying(object sender, EventArgs args) =>
            OnMediaPlayerPayingAsync(sender, args).Forget();
        
        private async UniTaskVoid OnMediaPlayerPayingAsync(object sender, EventArgs args)
        {
            try
            {
                if (Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
                
                if (!IsDisposed)
                    Playing?.Invoke();
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void OnMediaPlayerStopped(object sender, EventArgs args) => 
            OnMediaPlayerStoppedAsync(sender, args).Forget();
        
        private async UniTaskVoid OnMediaPlayerStoppedAsync(object sender, EventArgs args)
        {
            try
            {
                if (Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
                
                if (!IsDisposed)
                    Stopped?.Invoke();
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void OnVolumeChanged(float volume) => 
            _mediaPlayer?.SetVolume(Mathf.RoundToInt(_audio.GetVolumeOf(AudioMixerGroupName.Common) * 100));
    }
}