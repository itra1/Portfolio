using Base;
using UnityEngine;

namespace UI.Canvas.Presenter
{
    public interface ICanvasRectTransform : IRectTransformable
    {
        Vector2 ReferenceResolution { get; }
    }
}