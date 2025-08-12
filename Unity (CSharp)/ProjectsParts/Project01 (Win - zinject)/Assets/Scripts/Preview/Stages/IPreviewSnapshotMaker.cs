using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preview.Stages
{
    public interface IPreviewSnapshotMaker
    {
        UniTask<RenderTexture> MakeAsync(RectTransform rectTransform,
            CancellationToken cancellationToken,
            float? maxHeight = null);
    }
}