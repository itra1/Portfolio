using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine.Textures
{
    public interface ITextureValidator
    {
        bool Validate(RenderTexture renderTexture);
    }
}