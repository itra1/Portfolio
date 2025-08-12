namespace UI.Profiling.Presenter.Base
{
    public interface IProfilerItemPresenter
    {
        bool Active { get; }
        
        void Activate();
        void Deactivate();
    }
}