using System.Threading;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using UI.MusicPlayer.Controller.Converter.Data;

namespace UI.MusicPlayer.Controller.Converter
{
    public interface IAudioClipInfoResolver
    {
        UniTask<AudioClipInfo> ResolveAsync(AudioMaterialData material, byte[] bytes, CancellationToken cancellationToken);
    }
}