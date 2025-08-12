using Core.App;
using UI.ScreenBlocker.Presenter.Popups.Base;

namespace UI.ScreenBlocker.Presenter.Popups.Dialog.States
{
    public class DialogClosingState : IScreenBlockerState
    {
        private readonly IDialogStateContext _context;
        private readonly IApplicationState _applicationState;
        
        public DialogClosingState(IDialogStateContext context, IApplicationState applicationState)
        {
            _context = context;
            _applicationState = applicationState;
        }
        
        public void Show()
        {
            if (_applicationState.IsScreenLocked)
                _context.ChangeState<LockedState>();
            else
                _context.Close();
        }

        public void Hide() { }
    }
}