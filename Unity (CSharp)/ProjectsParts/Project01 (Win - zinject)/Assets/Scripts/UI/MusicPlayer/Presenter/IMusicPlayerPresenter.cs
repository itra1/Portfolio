using Base;
using Core.Materials.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace UI.MusicPlayer.Presenter
{
    public interface IMusicPlayerPresenter : IMusicPlayer, IMusicPlayerEventDispatcher, IUnloadable
    {
        void Initialize();
        
        void SetGroup(AudioMixerGroup group);
        
        void AddTrack(AudioMaterialData material, AudioClip audioClip);
        void UpdateTrack(AudioMaterialData material, AudioClip audioClip);
        void RemoveTrack(AudioMaterialData material);
    }
}