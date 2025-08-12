using UnityEngine;

namespace Elements.StatusColumn.Presenter.Components.Factory
{
    public interface IStatusHeaderTabFactory
    {
        IStatusHeaderTab Create(RectTransform parent, ulong materialId);
    }
}