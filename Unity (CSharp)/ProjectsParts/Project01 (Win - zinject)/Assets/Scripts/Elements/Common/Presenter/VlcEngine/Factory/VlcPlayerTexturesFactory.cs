using System;
using Elements.Common.Presenter.VlcEngine.Textures;
using LibVLCSharp;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Debug = Core.Logging.Debug;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public class VlcPlayerTexturesFactory : IVlcPlayerTexturesFactory
    {
        public bool TryCreate(MediaPlayer mediaPlayer, out ITexturesBuffer buffer)
        {
            buffer = null;

            if (mediaPlayer == null)
                return false;
            
            try
            {
                uint width = 0;
                uint height = 0;
                
                mediaPlayer.Size(0, ref width, ref height);
                
                if (width == 0 || height == 0)
                    return false;

                var texturePtr = mediaPlayer.GetTexture(width, height, out _);
                
                if (texturePtr == IntPtr.Zero)
                    return false;
                
                buffer = new TexturesBuffer();
                
                var texture = Texture2D.CreateExternalTexture((int) width, 
                    (int) height, 
                    TextureFormat.RGBA32, 
                    false, 
                    true, 
                    texturePtr);
                
                buffer.Texture = texture;
                
                var graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(TextureFormat.ARGB32, true);
                
                var renderTextures = new []
                {
                    new RenderTexture(texture.width, texture.height, 0, graphicsFormat),
                    new RenderTexture(texture.width, texture.height, 0, graphicsFormat)
                };
                
                buffer.RenderTextures = renderTextures;
                
                buffer.ExperimentalTexture = new Texture2D(4, 1, texture.format, false, true);
                
                buffer.ExperimentalRenderTexture = new RenderTexture((int) Mathf.Floor(texture.width),
                    (int) Mathf.Floor(texture.height), 
                    0, 
                    graphicsFormat);
                
                return true;
            }
            catch (Exception exception)
            {
                Destroy(ref buffer);
                Debug.LogException(exception);
            }
            
            return false;
        }

        public void Destroy(ref ITexturesBuffer buffer)
        {
            if (buffer == null) 
                return;
            
            buffer.Flush();
            buffer = null;
        }
    }
}