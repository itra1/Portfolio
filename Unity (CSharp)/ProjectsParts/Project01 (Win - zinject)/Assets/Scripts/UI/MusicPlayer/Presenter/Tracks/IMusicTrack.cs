using Core.Materials.Data;
using UnityEngine;

namespace UI.MusicPlayer.Presenter.Tracks
{
    public interface IMusicTrack
    {
        AudioMaterialData Material { get; }
        AudioClip AudioClip { get; }

        ulong Id { get; }

        void Update(AudioClip audioClip);
    }
}