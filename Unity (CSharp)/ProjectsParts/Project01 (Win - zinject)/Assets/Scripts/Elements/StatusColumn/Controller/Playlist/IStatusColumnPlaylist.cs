using System;

namespace Elements.StatusColumn.Controller.Playlist
{
    public interface IStatusColumnPlaylist : IDisposable
    {
        bool IsPlaying { get; }
        
        void Play();
        void Stop();
    }
}