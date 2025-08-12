using UI.ScreenBlocker.Presenter.Popups.Base;
using UI.ScreenBlocker.Presenter.Popups.Dialog.Consts;

namespace UI.ScreenBlocker.Presenter.Popups.Dialog.States
{
    public class LockedState : IScreenBlockerState
    {
        private readonly IDialogStateContext _context;
        
        public LockedState(IDialogStateContext context)
        {
            _context = context;
        }
        
        public void Show()
        {
            _context.EnableLogo();
            _context.EnableStatus(DialogComponentText.ScreenLocked);
        }

        public void Hide()
        {
            _context.DisableLogo();
            _context.DisableStatus();
        }
    }
}