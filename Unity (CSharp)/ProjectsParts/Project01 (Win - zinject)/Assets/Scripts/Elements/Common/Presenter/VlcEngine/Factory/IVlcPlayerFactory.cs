using Core.Elements.Windows.Camera.Data;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public interface IVlcPlayerFactory
    {
        IVlcPlayer CreatePlayer(RectTransform parent, CameraMaterialData material, string url);
        IVlcStream CreateStream(RectTransform parent, CameraMaterialData material, string url);
        IVlcStreamPlayer CreateStreamPlayer(IVlcStream stream, RectTransform parent);
    }
}