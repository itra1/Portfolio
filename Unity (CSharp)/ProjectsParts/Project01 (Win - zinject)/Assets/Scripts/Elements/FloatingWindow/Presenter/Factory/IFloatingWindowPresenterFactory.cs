using Core.Materials.Data;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.Factory
{
    public interface IFloatingWindowPresenterFactory
    {
        IFloatingWindowPresenter Create(MaterialData material, RectTransform parent, bool isAdaptiveSizeRequired);
    }
}