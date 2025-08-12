using Core.UI.Audio.Enums;

namespace UI.Audio.Controller
{
    public interface IAudioController : IAudioEventDispatcher
    {
        bool IsMutedOf(AudioMixerGroupName name);
        float GetVolumeOf(AudioMixerGroupName name);
        void SetVolumeOf(AudioMixerGroupName name, float value);
    }
}