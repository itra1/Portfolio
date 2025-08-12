namespace UI.MouseCursor.Presenter.Components
{
    public class CustomMouseCursorUpdater : IMouseCursorUpdater
    {
        public string Name { get; }
        public int Priority { get; }

        public CustomMouseCursorUpdater(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
    }
}