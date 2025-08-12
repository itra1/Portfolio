using System;
using Core.Materials.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.MusicPlayer.Presenter.Tracks
{
    public class MusicTrack : IMusicTrack, IDisposable
    {
        public AudioMaterialData Material { get; private set; }
        public AudioClip AudioClip { get; private set; }
        
        public ulong Id => Material.Id;
        
        public MusicTrack(AudioMaterialData material, AudioClip audioClip)
        {
            Material = material;
            AudioClip = audioClip;
        }
        
        public void Update(AudioClip audioClip)
        {
            try
            {
                AudioClip.UnloadAudioData();
                Object.Destroy(AudioClip);
            }
            catch
            {
                // ignored
            }

            AudioClip = audioClip;
        }

        public void Dispose()
        {
            try
            {
                AudioClip.UnloadAudioData();
                Object.Destroy(AudioClip);
            }
            catch
            {
                // ignored
            }
            
            Material = null;
        }
    }
}