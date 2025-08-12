using Core.App;
using TMPro;
using UI.ScreenBlocker.Presenter.Popups.Base;
using UI.ScreenBlocker.Presenter.Popups.Dialog.Enums;
using UI.ScreenBlocker.Presenter.Popups.Dialog.States;
using UnityEngine;

namespace UI.ScreenBlocker.Presenter.Popups.Dialog
{
    public class DialogStateContext : ScreenBlockerStateContextBase, IDialogStateContext
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private GameObject _logo;
        
        public DialogMessageType CurrentMessageType { get; set; }
        
        protected void ConfigureStates(IApplicationState applicationState)
        {
            base.ConfigureStates();
            
            RegisterState(new NotificationState(this));
            RegisterState(new LockedState(this));
            RegisterState(new DialogClosingState(this, applicationState));
        }

        public void EnableMessage(string text)
        {
            _message.gameObject.SetActive(true);
            _message.text = text;
        }
        
        public void DisableMessage()
        {
            _message.gameObject.SetActive(false);
            _message.text = string.Empty;
        }
        
        public void EnableStatus(string text)
        {
            _status.gameObject.SetActive(true);
            _status.text = text;
        }
        
        public void DisableStatus()
        {
            _status.gameObject.SetActive(false);
            _status.text = string.Empty;
        }
        
        public void EnableLogo() => _logo.SetActive(true);
        public void DisableLogo() => _logo.SetActive(false);
        
        public void Close()
        {
            if (Visible)
                Hide();
        }
    }
}