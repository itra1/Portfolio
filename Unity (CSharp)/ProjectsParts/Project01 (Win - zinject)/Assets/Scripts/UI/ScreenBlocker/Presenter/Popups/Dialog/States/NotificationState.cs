using UI.ScreenBlocker.Presenter.Popups.Base;
using UI.ScreenBlocker.Presenter.Popups.Dialog.Consts;
using UI.ScreenBlocker.Presenter.Popups.Dialog.Enums;

namespace UI.ScreenBlocker.Presenter.Popups.Dialog.States
{
    public class NotificationState : IScreenBlockerState
    {
        private readonly IDialogStateContext _context;
        
        public NotificationState(IDialogStateContext context)
        {
            _context = context;
        }
        
        public void Show()
        {
            var messageType = _context.CurrentMessageType;
            
            var messageText = string.Empty;
            
            switch (messageType)
            {
                case DialogMessageType.ConnectionWaiting:
                    messageText = DialogComponentText.ConnectionWaiting;
                    break;
                case DialogMessageType.Initialization:
                    messageText = DialogComponentText.Initialization;
                    break;
            }
            
            if (!string.IsNullOrEmpty(messageText))
                _context.EnableMessage(messageText);
            
            _context.CurrentMessageType = DialogMessageType.None;
        }
        
        public void Hide() => _context.DisableMessage();
    }
}