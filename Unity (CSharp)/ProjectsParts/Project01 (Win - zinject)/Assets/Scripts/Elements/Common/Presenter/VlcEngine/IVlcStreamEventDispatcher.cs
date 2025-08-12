using System;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine
{
    public interface IVlcStreamEventDispatcher
    {
        event Action<Texture, Material> ImageUpdated;
        event Action<Texture> SizeAdjusted;
    }
}