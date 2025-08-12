using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine.Textures
{
    public class TexturesBuffer : ITexturesBuffer
    {
        public Texture2D Texture { get; set; }
        public RenderTexture[] RenderTextures { get; set; }
        public Texture2D ExperimentalTexture { get; set; }
        public RenderTexture ExperimentalRenderTexture { get; set; }
        
        public int CurrentRenderTextureIndex { get; set; }
        public int NextRenderTextureIndex => (CurrentRenderTextureIndex + 1) % 2;
        
        public void Flush()
        {
            if (Texture != null)
                Object.Destroy(Texture);
            
            var renderTextures = RenderTextures;
            
            if (renderTextures != null)
            {
                for (var i = renderTextures.Length - 1; i >= 0; i--)
                {
                    var renderTexture = renderTextures[i];
                    
                    renderTexture.Release();
                    renderTexture.DiscardContents();
                    
                    Object.Destroy(renderTexture);
                }
            }
            
            if (ExperimentalTexture != null)
                Object.Destroy(ExperimentalTexture);

            if (ExperimentalRenderTexture != null)
            {
                ExperimentalRenderTexture.Release();
                ExperimentalRenderTexture.DiscardContents();
                
                Object.Destroy(ExperimentalRenderTexture);
            }

            CurrentRenderTextureIndex = 0;
        }
    }
}