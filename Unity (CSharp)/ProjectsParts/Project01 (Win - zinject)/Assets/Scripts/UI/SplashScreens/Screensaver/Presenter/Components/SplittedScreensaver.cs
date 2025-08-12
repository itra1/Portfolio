using System;
using Core.Elements.Windows.VideoSplit.Data;
using Core.FileResources;
using Core.FileResources.Customizing.Category;
using Core.FileResources.Info;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.Windows.Video.Presenter.VideoPlayer;
using Elements.Windows.VideoSplit.Presenter.VideoPlayer.Factory;
using UnityEngine;
using Utils;
using Debug = Core.Logging.Debug;

namespace UI.SplashScreens.Screensaver.Presenter.Components
{
    public class SplittedScreensaver : ScreensaverBase, IScreensaver
    {
        private IResourceProvider _resources;
        private IMaterialDataLoader _materialLoader;
        private IMaterialDataStorage _materials;
        private IVideoSplitPlayerFactory _playerFactory;
        
        private IVideoPlayer[] _players;
        private RectTransform _rectTransform;
        private CancellationTokenSourceProxy _cancellationTokenSource;
        private float? _progress;
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public void Initialize(IResourceProvider resources,
            IMaterialDataLoader materialLoader,
            IMaterialDataStorage materials,
            IVideoSplitPlayerFactory playerFactory)
        {
            _resources = resources;
            _materialLoader = materialLoader;
            _materials = materials;
            _playerFactory = playerFactory;
            
            SetInitialOpacity(0f);
        }
        
        public async UniTask<bool> StartPlayingAsync(ulong materialId)
        {
            StopIfPlaying();
            
            var material = _materials.Get<VideoSplitMaterialData>(materialId) ?? await TryLoadMaterialAsync(materialId);

            if (material == null)
            {
                StopIfPlaying();
                return false;
            }

            var files = material.Files;
            var playersCount = files.Count;
            
            if (_players == null || _players.Length == 0)
                CreatePlayers(playersCount);
            
            if (_players == null || _players.Length == 0)
            {
                StopIfPlaying();
                return false;
            }
            
            for (var i = 0; i < playersCount; i++)
            {
                var url = files[i].Url;
                var materialName = material.Name;
                
                var filePath = await DownloadFileAsync(url, materialName);
                
                if (string.IsNullOrEmpty(filePath))
                {
                    Debug.LogError($"Cached video file path is null or empty when trying to preload video resource by URL: {url}");
                    StopIfPlaying();
                    return false;
                }
                
                var player = _players[i];
                
                player.SetMode(ScaleMode.ScaleToFit);
                player.Show();
                
                if (!player.Visible)
                    await ValidateAsync(() => player.Visible);
                
                if (!player.Open(filePath))
                {
                    Debug.LogError($"Failed attempt detected to open video by file path: {filePath}");
                    StopIfPlaying();
                    return false;
                }
                
                if (!player.IsInitialized)
                    await ValidateAsync(() => player.IsInitialized);
            }
            
            var size = RectTransform.rect.size;
            var widthParameters = _players.CalculateWidthParameters(size, true);
            var totalVideoWidth = widthParameters.TotalVideoWidth;
            var scale = widthParameters.Scale;
            
            var totalPlayerWidth = 0f;
            
            for (var i = 0; i < playersCount; i++)
            {
                var player = _players[i];
                var playerRectTransform = player.RectTransform;
                
                var playerWidth = 0f;
                var playerHeight = 0f;
				
                if (player.IsInitialized)
                {
                    var videoSize = player.OriginalVideoSize;
					
                    if (videoSize is { x: > 0, y: > 0 })
                    {
                        playerWidth = videoSize.x * scale;
                        playerHeight = videoSize.y * scale;
                    }
                }
                
                var anchoredPlayerPositionX = totalPlayerWidth + (playerWidth - totalVideoWidth) * 0.5f;
                var anchoredPlayerPositionY = playerRectTransform.anchoredPosition.y;
				
                playerRectTransform.anchoredPosition = new Vector2(anchoredPlayerPositionX, anchoredPlayerPositionY);
                playerRectTransform.sizeDelta = new Vector2(playerWidth - size.x, playerHeight - size.y);
                
                totalPlayerWidth += playerWidth;
                
                player.Play();
                
                if (i > 0)
                    ValidatePlayerAsync(player).Forget();
            }

            return true;
        }
        
        public void TogglePlayPause()
        {
            if (_players == null || _players.Length == 0)
                return;
            
            foreach (var player in _players)
            {
                if (_progress != null)
                {
                    player.SetProgress(_progress.Value);
                    player.Play();
                }
                else
                {
                    player.Pause();
                }
            }
            
            if (_progress != null)
            {
                _progress = null;
            }
            else
            {
                var player = _players[0];
                _progress = (float) (player.CurrentTime / player.Duration);
            }
        }
        
        public void StopIfPlaying()
        {
            _progress = null;
            
            if (_cancellationTokenSource != null)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();
                
                if (!_cancellationTokenSource.IsDisposed)
                    _cancellationTokenSource.Dispose();
                
                _cancellationTokenSource = null;
            }
            
            if (_players is { Length: > 0 })
                RemovePlayers();
        }
        
        public override void Unload()
        {
            base.Unload();
            
            StopIfPlaying();
            
            _resources = null;
            _materialLoader = null;
            _materials = null;
            _playerFactory = null;
        }
        
        private void CreatePlayers(int playersCount) => 
            _players = _playerFactory.Create(RectTransform, playersCount, true);
        
        private void RemovePlayers() =>
            _playerFactory.Destroy(ref _players);
        
        private async UniTask<VideoSplitMaterialData> TryLoadMaterialAsync(ulong materialId)
        {
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            var info = new MaterialDataLoadingInfo(typeof(VideoSplitMaterialData), materialId);
            var result = await _materialLoader.LoadAsync(info);
            
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                return null;
            
            if (!_cancellationTokenSource.IsDisposed)
                _cancellationTokenSource.Dispose();
            
            _cancellationTokenSource = null;
            
            if (!result.Success)
            {
                Debug.LogError($"Failed to load video split material by id {materialId}");
                return null;
            }
			
            if (!result.TryGetFirstMaterial<VideoSplitMaterialData>(out var material))
            {
                Debug.LogError($"No video split material was found in the loaded list of materials by requested material id {materialId}");
                return null;
            }
			
            return material;
        }
        
        private async UniTask<string> DownloadFileAsync(string url, string materialName)
        {
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            var info = new ResourceInfo(url, ResourceCategory.Video, materialName);
            var result = await _resources.RequestAsync<byte[]>(info, _cancellationTokenSource.Original);
			
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                return null;
            
            if (!_cancellationTokenSource.IsDisposed)
                _cancellationTokenSource.Dispose();
            
            _cancellationTokenSource = null;
			
            return result.FilePath;
        }
        
        private async UniTask<bool> ValidateAsync(Func<bool> predicate)
        {
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            try
            {
                await UniTask.WaitUntil(predicate, cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
            finally
            {
                if (_cancellationTokenSource is { IsDisposed: false })
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }

            return true;
        }
        
        private async UniTaskVoid ValidatePlayerAsync(IVideoPlayer player)
        {
            if (player.IsInitialized || await ValidateAsync(() => player.IsInitialized))
                player.Mute();
        }
    }
}