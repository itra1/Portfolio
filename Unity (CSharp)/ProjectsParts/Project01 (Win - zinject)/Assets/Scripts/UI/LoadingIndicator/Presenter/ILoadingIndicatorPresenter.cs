using Base;

namespace UI.LoadingIndicator.Presenter
{
    public interface ILoadingIndicatorPresenter : IUnloadable
    {
        bool Active { get; }

        void Initialize();
        
        void Activate();
        void Deactivate();
    }
}