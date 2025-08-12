using Core.Materials.Data;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Common.Factory
{
    public interface IWindowPresenterAdapterFactory
    {
        IWindowPresenterAdapter Create(MaterialData material, RectTransform parent);
    }
}