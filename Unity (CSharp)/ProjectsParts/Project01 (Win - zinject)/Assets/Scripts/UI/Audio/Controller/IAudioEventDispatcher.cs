using System;

namespace UI.Audio.Controller
{
    public interface IAudioEventDispatcher
    {
        event Action<float> VolumeChanged;
    }
}