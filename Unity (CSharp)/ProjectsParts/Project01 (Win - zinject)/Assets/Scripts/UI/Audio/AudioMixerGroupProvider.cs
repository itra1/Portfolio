using System;
using System.Collections.Generic;
using System.Threading;
using Core.Base;
using Core.UI.Audio.Enums;
using Cysharp.Threading.Tasks;
using Settings;
using UnityEngine;
using UnityEngine.Audio;
using Debug = Core.Logging.Debug;

namespace UI.Audio
{
    public class AudioMixerGroupProvider : IAudioMixerGroupProvider, ILateInitialized, IDisposable
    {
        private readonly IUISettings _settings;
        private readonly IDictionary<AudioMixerGroupName, AudioMixerGroup> _groupsByName;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool IsInitialized { get; private set; }
        
        public ICollection<AudioMixerGroupName> GetAvailableNames() => _groupsByName.Keys;
        
        public AudioMixerGroupProvider(IUISettings settings)
        {
            _settings = settings;
            _groupsByName = new Dictionary<AudioMixerGroupName, AudioMixerGroup>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            StartDownload();
        }
        
        public bool TryGetGroup(AudioMixerGroupName name, out AudioMixerGroup group) =>
            _groupsByName.TryGetValue(name, out group);
        
        public void Dispose()
        {
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
			
            _groupsByName.Clear();
        }
        
        private void StartDownload() => 
            DownloadResourcesAsync(_settings.AudioMixersFolder).Forget();
        
        private async UniTaskVoid DownloadResourcesAsync(string directoryPath)
        {
            try
            {
                var cancellationToken = _disposeCancellationTokenSource.Token;

                var type = typeof(AudioMixerGroupName);
                var values = Enum.GetValues(type);

                foreach (AudioMixerGroupName value in values)
                {
                    var name = Enum.GetName(type, value);
                    var path = $"{directoryPath}/{name}";
                    var request = Resources.LoadAsync<AudioMixerGroup>(path);
                    
                    await UniTask.WaitUntil(() => request.isDone, cancellationToken: cancellationToken);
                    
                    var group = request.asset as AudioMixerGroup;
                    
                    if (group == null)
                    {
                        Debug.LogError($"An error occurred while trying to download a group of audio mixers with name \"{name}\" from resources along by path: {path}");
                        continue;
                    }
                    
                    _groupsByName.Add(value, group);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
            finally
            {
                IsInitialized = true;
            }
        }
    }
}