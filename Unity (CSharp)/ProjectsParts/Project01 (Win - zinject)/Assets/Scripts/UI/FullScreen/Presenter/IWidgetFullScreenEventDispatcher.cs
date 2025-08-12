using System;

namespace UI.FullScreen.Presenter
{
    public interface IWidgetFullScreenEventDispatcher
    {
        event Action Clicked;
    }
}