namespace UI.ScreenBlocker.Presenter.Popups.Base
{
    public interface IScreenBlockerStateContext
    {
        void ChangeState<TScreenBlockerState>() where TScreenBlockerState : IScreenBlockerState;
    }
}