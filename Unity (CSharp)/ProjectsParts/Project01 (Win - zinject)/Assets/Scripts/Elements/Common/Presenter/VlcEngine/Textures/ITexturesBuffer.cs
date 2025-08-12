using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine.Textures
{
    public interface ITexturesBuffer
    {
        Texture2D Texture { get; set; }
        RenderTexture[] RenderTextures { get; set; }
        
        Texture2D ExperimentalTexture { get; set; }
        RenderTexture ExperimentalRenderTexture { get; set; }
        
        int CurrentRenderTextureIndex { get; set; }
        int NextRenderTextureIndex { get; }

        void Flush();
    }
}