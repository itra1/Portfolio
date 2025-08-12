using System;
using System.Collections.Generic;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.States.Data;
using Core.UI.Audio.Enums;
using UI.Audio.Controller.Consts;
using UI.Audio.Controller.Data;
using UI.Audio.Controller.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace UI.Audio.Controller
{
    public class AudioController : IAudioController, IDisposable
    {
        private readonly IAudioMixerGroupProvider _provider;
        private readonly IDictionary<AudioMixerGroupName, IVolume> _volumesByName;
        
        public event Action<float> VolumeChanged;
        
        public AudioController(IAudioMixerGroupProvider provider)
        {
            _provider = provider;
            _volumesByName = new Dictionary<AudioMixerGroupName, IVolume>();
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
            MessageDispatcher.AddListener(MessageType.AudioVolumeUp, OnAudioVolumeUp);
            MessageDispatcher.AddListener(MessageType.AudioVolumeDown, OnAudioVolumeDown);
            MessageDispatcher.AddListener(MessageType.AudioMuteToggle, OnAudioMuteToggled);
            MessageDispatcher.AddListener(MessageType.AudioSet, OnAudioSet);
        }
        
        public bool IsMutedOf(AudioMixerGroupName name)
        {
            if (_volumesByName.TryGetValue(name, out var volume))
                return volume.IsMuted;
            
            if (!TryGetThresholdValue(name, out var thresholdValue))
                return false;
            
            volume = new Volume(VolumeConverter.ConvertToValue(thresholdValue));
            _volumesByName.Add(name, volume);
            
            return volume.IsMuted;
        }
        
        public float GetVolumeOf(AudioMixerGroupName name)
        {
            if (_volumesByName.TryGetValue(name, out var volume))
                return volume.Value;
            
            if (!TryGetThresholdValue(name, out var thresholdValue))
                return 0f;
            
            volume = new Volume(VolumeConverter.ConvertToValue(thresholdValue));
            _volumesByName.Add(name, volume);
            
            return volume.Value;
        }
        
        public void SetVolumeOf(AudioMixerGroupName name, float value)
        {
            if (!_volumesByName.TryGetValue(name, out var volume))
            {
                volume = new Volume(value);
                _volumesByName.Add(name, volume);
            }
            else
            {
                volume.Value = value;
            }
            
            if (IsMutedOf(name))
                return;
            
            if (_provider.TryGetGroup(name, out var group))
                SetVolumeAt(group, value);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            MessageDispatcher.RemoveListener(MessageType.AudioVolumeUp, OnAudioVolumeUp);
            MessageDispatcher.RemoveListener(MessageType.AudioVolumeDown, OnAudioVolumeDown);
            MessageDispatcher.RemoveListener(MessageType.AudioMuteToggle, OnAudioMuteToggled);
            MessageDispatcher.RemoveListener(MessageType.AudioSet, OnAudioSet);
            
            _volumesByName.Clear();
        }
        
        private void MuteOf(AudioMixerGroupName name)
        {
            if (!TryGetThresholdValue(name, out var group, out var thresholdValue))
                return;
            
            if (!_volumesByName.TryGetValue(name, out var volume))
            {
                volume = new Volume(VolumeConverter.ConvertToValue(thresholdValue), true);
                _volumesByName.Add(name, volume);
            }
            else
            {
                volume.IsMuted = true;
            }
            
            SetVolumeAt(group, VolumeConverter.ConvertToValue(VolumeConverter.MinThresholdValue));
        }
        
        private void UnmuteOf(AudioMixerGroupName name)
        {
            if (!TryGetThresholdValue(name, out var group, out var thresholdValue))
                return;
            
            if (!_volumesByName.TryGetValue(name, out var volume))
            {
                volume = new Volume(VolumeConverter.ConvertToValue(thresholdValue));
                _volumesByName.Add(name, volume);
            }
            else
            {
                volume.IsMuted = false;
                SetVolumeAt(group, volume.Value);
            }
        }
        
        private void TurnVolumeUpOf(AudioMixerGroupName name) => 
            SetVolumeOf(name, Mathf.Clamp01(GetVolumeOf(name) + 0.1f));
        
        private void TurnVolumeDownOf(AudioMixerGroupName name) =>
            SetVolumeOf(name, Mathf.Clamp01(GetVolumeOf(name) - 0.1f));
        
        private bool TryGetThresholdValue(AudioMixerGroupName name, out AudioMixerGroup group, out float thresholdValue)
        {
            thresholdValue = 0;
            
            return _provider.TryGetGroup(name, out group)
                   && group.audioMixer.GetFloat(AudioMixerParameter.BaseVolume, out thresholdValue);
        }
        
        private bool TryGetThresholdValue(AudioMixerGroupName name, out float thresholdValue) =>
            TryGetThresholdValue(name, out _, out thresholdValue);
        
        private void SetVolumeAt(AudioMixerGroup group, float value)
        {
            if (group.audioMixer.SetFloat(AudioMixerParameter.BaseVolume, VolumeConverter.ConvertToThresholdValue(value)))
                VolumeChanged?.Invoke(value);
        }
        
        private void OnAudioVolumeUp(IMessage message)
        {
            foreach (var name in _provider.GetAvailableNames())
                TurnVolumeUpOf(name);
        }
        
        private void OnAudioVolumeDown(IMessage message)
        {
            foreach (var name in _provider.GetAvailableNames())
                TurnVolumeDownOf(name);
        }
        
        private void OnAudioMuteToggled(IMessage message)
        {
            foreach (var name in _provider.GetAvailableNames())
            {
                if (IsMutedOf(name))
                    UnmuteOf(name);
                else
                    MuteOf(name);
            }
        }
        
        private void OnAudioSet(IMessage message)
        {
            var state = (SoundVolumeState) message.Data;
            
            SetVolumeOf(AudioMixerGroupName.Common, Mathf.Clamp01(state.Level / 100f));
            
            if (state.Mute)
                MuteOf(AudioMixerGroupName.Common);
            else
                UnmuteOf(AudioMixerGroupName.Common);
        }

        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            foreach (var name in _provider.GetAvailableNames())
            {
                if (_provider.TryGetGroup(name, out var group))
                    group.audioMixer.SetFloat(AudioMixerParameter.BaseVolume, VolumeConverter.ConvertToThresholdValue(1f));
            }
        }
    }
}