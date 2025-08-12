using UI.ScreenBlocker.Presenter.Popups.Base;
using UI.ScreenBlocker.Presenter.Popups.Dialog.Enums;

namespace UI.ScreenBlocker.Presenter.Popups.Dialog
{
    public interface IDialogStateContext : IScreenBlockerStateContext
    {
        public DialogMessageType CurrentMessageType { get; set; }
        
        void EnableMessage(string text);
        void DisableMessage();
        void EnableStatus(string text);
        void DisableStatus();
        void EnableLogo();
        void DisableLogo();

        void Close();
    }
}