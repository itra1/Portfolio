using Core.Elements.Windows.Camera.Data;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine
{
    /// <summary>
    /// Устаревшее название - "VlcPlayer"
    /// </summary>
    public interface IVlcStream : IVlcPlayer, IVlcStreamEventDispatcher
    {
        Texture CurrentTexture { get; }
        
        CameraMaterialData Material { get; }
        
        Texture ImageTexture { get; }
        Material ImageMaterial { get; }
    }
}