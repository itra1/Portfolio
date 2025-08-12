namespace UI.MouseCursor.Presenter.Components
{
    public interface IMouseCursorUpdater
    {
        string Name { get; }
        int Priority { get; }
    }
}