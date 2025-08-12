using System.Collections.Generic;
using Core.UI.Audio.Enums;
using UnityEngine.Audio;

namespace UI.Audio
{
    public interface IAudioMixerGroupProvider
    {
        ICollection<AudioMixerGroupName> GetAvailableNames();
        bool TryGetGroup(AudioMixerGroupName name, out AudioMixerGroup group);
    }
}