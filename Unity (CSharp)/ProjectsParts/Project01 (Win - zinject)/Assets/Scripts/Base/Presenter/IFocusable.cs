namespace Base.Presenter
{
    public interface IFocusable : IFocusState
    {
        void Focus();
        void Unfocus();
    }
}