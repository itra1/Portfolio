using System.Threading;
using Core.Utils;
using Core.Utils.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preview.Stages
{
    public class PreviewImageEncoder : IPreviewImageEncoder
    {
        public async UniTask<byte[]> EncodeAsync(RenderTexture texture, CancellationToken cancellationToken)
        {
            try
            {
                return await texture.ToBytesAsync(cancellationToken, FileEncodingFormat.Png);
            }
            finally
            {
                try
                {
                    texture.Release();
                    texture.DiscardContents();

                    Object.Destroy(texture);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}