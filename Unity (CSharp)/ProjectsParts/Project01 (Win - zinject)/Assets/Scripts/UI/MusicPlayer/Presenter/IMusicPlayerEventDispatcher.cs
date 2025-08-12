using System;

namespace UI.MusicPlayer.Presenter
{
    public interface IMusicPlayerEventDispatcher
    {
        event Action<bool> PlaybackCompleted;
    }
}