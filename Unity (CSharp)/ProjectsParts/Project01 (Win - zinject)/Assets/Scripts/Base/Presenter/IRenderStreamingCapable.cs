using System;

namespace Base.Presenter
{
    public interface IRenderStreamingCapable : IRectTransformable
    {
        event Action<IRectTransformable> Resized;
    }
}