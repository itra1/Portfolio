using com.ootii.Messages;
using Core.App;
using Core.Materials.Loading.AutoPreloader;
using Core.Messages;
using UI.ScreenBlocker.Presenter.Popups.Dialog.Enums;
using UI.ScreenBlocker.Presenter.Popups.Dialog.States;
using Zenject;

namespace UI.ScreenBlocker.Presenter.Popups.Dialog
{
    public class DialogPopup : DialogStateContext
    {
        private IApplicationState _applicationState;

        [Inject]
        private void Initialize(IApplicationState applicationState, IAutoMaterialDataPreloader preloader)
        {
            _applicationState = applicationState;

            ConfigureStates(applicationState);

            MessageDispatcher.AddListener(MessageType.AppStart, OnApplicationStarted);
            MessageDispatcher.AddListener(MessageType.AppLoadStart, OnApplicationLoadingStarted);
            MessageDispatcher.AddListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
            MessageDispatcher.AddListener(MessageType.ScreenLock, OnScreenLocked);
        }

        public override void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);
            MessageDispatcher.RemoveListener(MessageType.AppLoadStart, OnApplicationLoadingStarted);
            MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);
            MessageDispatcher.RemoveListener(MessageType.ScreenLock, OnScreenLocked);

            _applicationState = null;

            base.Dispose();
        }

        private void OnApplicationStarted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);

            if (!Visible)
                Show();

            CurrentMessageType = DialogMessageType.ConnectionWaiting;
            ChangeState<NotificationState>();
        }

        private void OnApplicationLoadingStarted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppLoadStart, OnApplicationLoadingStarted);

            if (!Visible)
                Show();

            CurrentMessageType = DialogMessageType.Initialization;
            ChangeState<NotificationState>();
        }

        private void OnApplicationLoadingCompleted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationLoadingCompleted);

            if (_applicationState.IsScreenLocked)
                ChangeState<LockedState>();
            else
                Close();
        }

        private void OnScreenLocked(IMessage message)
        {
            if (!_applicationState.IsAppLoadingCompleted)
                return;

            if (_applicationState.IsScreenLocked)
            {
                if (!Visible)
                    Show();

                ChangeState<LockedState>();
            }
            else
            {
                Close();
            }
        }
    }
}