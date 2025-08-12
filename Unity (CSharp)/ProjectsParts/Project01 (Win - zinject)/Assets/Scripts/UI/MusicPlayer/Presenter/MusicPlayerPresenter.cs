using System;
using System.Collections.Generic;
using Core.Materials.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UI.MusicPlayer.Presenter.Tracks;
using UnityEngine;
using UnityEngine.Audio;
using Debug = Core.Logging.Debug;

namespace UI.MusicPlayer.Presenter
{
    public class MusicPlayerPresenter : MonoBehaviour, IMusicPlayerPresenter
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _isLooping;
        
        private IOutgoingStateController _outgoingState;
        private IList<MusicTrack> _orderedTracks;
        private IDictionary<ulong, int> _orderedIndicesByTrackId;
        private int _time;
        private CancellationTokenSourceProxy _cancellationTokenSource;
        
        public event Action<bool> PlaybackCompleted;
        
        public bool IsPlaying { get; private set; }
        public bool IsLooping => _isLooping;
        
        public ulong CurrentTrackId { get; private set; }
        
        private int RealTime => Mathf.FloorToInt(_audioSource.time);
        
        public int Time
        {
            get => _time;
            private set
            {
                _time = value;
                TimeChanged++;
            }
        }
        
        public int TimeChanged { get; private set; }
        
        private int CurrentTrackIndex
        {
            get
            {
                var trackId = CurrentTrackId;
                
                if (_orderedIndicesByTrackId.TryGetValue(trackId, out var index)) 
                    return index;
                
                if (trackId == 0 && _orderedTracks.Count > 0)
                    return 0;
                
                Debug.LogError($"The current track id with value {trackId} was not found as a key in the track index ordered dictionary");
                return -1;
            }
        }
        
        public void Initialize()
        {
            _audioSource.playOnAwake = false;
            _orderedTracks = new List<MusicTrack>();
            _orderedIndicesByTrackId = new Dictionary<ulong, int>();
        }
        
        public void SetGroup(AudioMixerGroup group) => _audioSource.outputAudioMixerGroup = group;
        
        public void AddTrack(AudioMaterialData material, AudioClip audioClip)
        {
            var track = new MusicTrack(material, audioClip);
            _orderedTracks.Add(track);
            _orderedIndicesByTrackId.Add(track.Id, _orderedTracks.Count - 1);
        }

        public void UpdateTrack(AudioMaterialData material, AudioClip audioClip)
        {
            for (var i = _orderedTracks.Count - 1; i >= 0; i--)
            {
                var track = _orderedTracks[i];
                
                if (material != track.Material) 
                    continue;
                
                if (CurrentTrackId == track.Id)
                {
                    var isPlaying = IsPlaying;
                    
                    if (isPlaying)
                        StopAudio();
                    
                    _audioSource.clip = audioClip;
                    _audioSource.time = 0f;
                    
                    if (isPlaying)
                        PlayAudio();
                }
                
                track.Update(audioClip);
                return;
            }
            
            AddTrack(material, audioClip);
        }
        
        public void RemoveTrack(AudioMaterialData material)
        {
            for (var i = _orderedTracks.Count - 1; i >= 0; i--)
            {
                var track = _orderedTracks[i];
                
                if (material != track.Material) 
                    continue;
                
                var trackId = track.Id;
                
                if (CurrentTrackId == trackId)
                {
                    if (IsPlaying)
                        StopAudio();
                    
                    _audioSource.clip = null;
                    
                    CurrentTrackId = 0;
                }
                
                _orderedTracks.RemoveAt(i);
                _orderedIndicesByTrackId.Remove(trackId);
                
                track.Dispose();
                return;
            }
        }
        
        public void ToggleLoop() => _isLooping = !_isLooping;
        
        public void ResetTrack()
        {
            ValidateAudioClip();
            
            var isPlaying = IsPlaying;
            
            if (isPlaying)
                StopAudio();
            
            _audioSource.time = 0f;
            
            if (isPlaying)
                PlayAudio();
            
            Time = RealTime;
        }
        
        public void TogglePlay()
        {
            ValidateAudioClip();
            
            if (IsPlaying)
                PauseAudio();
            else
                PlayAudio();
            
            Time = RealTime;
        }
        
        public void SetTime(int time)
        {
            _audioSource.time = Mathf.Clamp(time, 0f, _audioSource.clip.length);
            Time = RealTime;
        }
        
        public void SetNextTrack()
        {
            var index = CurrentTrackIndex;
            
            if (index < 0)
                return;
            
            var tracksCount = _orderedTracks.Count;
            
            if (tracksCount == 0)
                return;
            
            if (index >= tracksCount - 1)
                index = 0;
            else
                index++;
            
            SwitchToTrack(index);
        }
        
        public void SetPreviousTrack()
        {
            var index = CurrentTrackIndex;
            
            if (index < 0)
                return;
            
            var tracksCount = _orderedTracks.Count;
            
            if (tracksCount == 0)
                return;
            
            if (index <= 0)
                index = tracksCount - 1;
            else
                index--;
            
            SwitchToTrack(index);
        }
        
        public void SetTrack(ulong trackId)
        {
            var tracksCount = _orderedTracks.Count;
            
            if (tracksCount == 0)
                return;
            
            if (!_orderedIndicesByTrackId.TryGetValue(trackId, out var index))
            {
                Debug.LogError($"The current track id with value {trackId} was not found as a key in the track index ordered dictionary");
                return;
            }
            
            if (index < 0)
                return;
            
            SwitchToTrack(index);
        }
        
        public void Unload()
        {
            try
            {
                if (IsPlaying)
                    StopAudio();

                _audioSource.clip = null;
            }
            catch
            {
                // ignored
            }
            finally
            {
                StopCheckingIfRequired();
            }
            
            CurrentTrackId = 0;
            TimeChanged = 0;
            
            _time = 0;
            
            for (var i = _orderedTracks.Count - 1; i >= 0; i--)
                _orderedTracks[i].Dispose();
            
            _orderedTracks.Clear();
            _orderedIndicesByTrackId.Clear();
        }

        private void SwitchToTrack(int index)
        {
            var isPlaying = IsPlaying;
            
            if (isPlaying)
                StopAudio();
            
            var track = _orderedTracks[index];
            _audioSource.clip = track.AudioClip;
            CurrentTrackId = track.Id;

            if (isPlaying)
                PlayAudio();
            else
                Time = 0;
        }

        private void ValidateAudioClip()
        {
            if (_audioSource.clip != null) 
                return;
            
            var track = _orderedTracks[CurrentTrackIndex];
            _audioSource.clip = track.AudioClip;
            CurrentTrackId = track.Id;
        }

        private void PlayAudio()
        {
            _audioSource.Play();
            IsPlaying = _audioSource.isPlaying;
            CheckForPlaybackFinished().Forget();
        }

        private void StopAudio()
        {
            StopCheckingIfRequired();
            _audioSource.Stop();
            IsPlaying = _audioSource.isPlaying;
        }
        
        private void PauseAudio()
        {
            StopCheckingIfRequired();
            _audioSource.Pause();
            IsPlaying = _audioSource.isPlaying;
        }
        
        private async UniTaskVoid CheckForPlaybackFinished()
        {
            StopCheckingIfRequired();
            
            _cancellationTokenSource = new CancellationTokenSourceProxy();
            
            try
            {
                await UniTask.WaitWhile(() => _audioSource.clip != null && _audioSource.time < _audioSource.clip.length,
                    cancellationToken: _cancellationTokenSource.Token);
                
                if (_audioSource.clip == null)
                {
                    StopCheckingIfRequired();
                    return;
                }
                
                PlaybackCompleted?.Invoke(_isLooping);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void StopCheckingIfRequired()
        {
            if (_cancellationTokenSource != null)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();
                
                if (!_cancellationTokenSource.IsDisposed)
                    _cancellationTokenSource.Dispose();
                
                _cancellationTokenSource = null;
            }
        }
    }
}