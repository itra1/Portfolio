using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preview.Stages
{
    public interface IPreviewImageEncoder
    {
        UniTask<byte[]> EncodeAsync(RenderTexture texture, CancellationToken cancellationToken);
    }
}