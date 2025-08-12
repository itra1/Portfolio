using System.Threading;
using Core.FileResources.Customizing.Converting.Serializers.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Core.FileResources.Customizing.Converting.Serializers
{
    public class Texture2DSerializer : IResourceSerializer<Texture2D>
    {
        private readonly GraphicsFormat _graphicsFormat;
        private readonly TextureCreationFlags _flags;
		
        public Texture2DSerializer(GraphicsFormat graphicsFormat, TextureCreationFlags flags)
        {
            _graphicsFormat = graphicsFormat;
            _flags = flags;
        }
		
        public async UniTask<Texture2D> SerializeAsync(string name, byte[] bytes, CancellationToken cancellationToken)
        {
            var texture = new Texture2D(0, 0, _graphicsFormat, 0, _flags)
            {
                name = name,
                wrapMode = TextureWrapMode.Clamp
            };
            
            texture.LoadRawTextureData(bytes);
            
            await UniTask.NextFrame(cancellationToken);
            
            texture.Apply(false, true);
            
            return texture;
        }
    }
}