using UI.ScreenBlocker.Presenter.Popups.Base;
using UnityEngine;

namespace UI.ScreenBlocker.Presenter.Popups.Common.Factory
{
    public interface IScreenBlockerPopupFactory
    {
        IScreenBlockerPopup Create<TScreenBlockerPopup>(RectTransform parent)
            where TScreenBlockerPopup : IScreenBlockerPopup;
    }
}