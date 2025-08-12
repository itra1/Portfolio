using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Options;
using Core.UI.Notifications.Consts;
using Core.UI.Notifications.Data;
using Environment.Microsoft.Windows.Api;
using UI.Switches;
using UI.Switches.Triggers.Data;
using UI.Switches.Triggers.Data.Enums;

namespace App
{
    /// <summary>
    /// Устаревшее название - "GameManager"
    /// </summary>
    public class ApplicationEnvironmentController : IApplicationEnvironmentController, IDisposable
    {
        private readonly IApplicationOptions _options;
        private readonly ITriggerSwitch _triggerSwitch;
        
        private readonly TriggerKey _triggerShortcutKeys1;
        private readonly TriggerKey _triggerShortcutKeys2;
        
        public bool IsOnTop { get; private set; }
        
        public ApplicationEnvironmentController(IApplicationOptions options, ITriggerSwitch triggerSwitch)
        {
            _options = options;
            _triggerSwitch = triggerSwitch;
            
            _triggerShortcutKeys1 = new TriggerKey(TriggerType.WindowsOnTop1);
            _triggerShortcutKeys2 = new TriggerKey(TriggerType.WindowsOnTop2);
            
            MessageDispatcher.AddListener(MessageType.AppStart, OnApplicationStarted);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);
            
            _triggerSwitch.RemoveListener(_triggerShortcutKeys1, OnTopModeStateToggled);
            _triggerSwitch.RemoveListener(_triggerShortcutKeys2, OnTopModeStateToggled);
            
#if UNITY_EDITOR
            ConfirmOnTopMode(false);
#endif
        }
        
        private void ConfirmOnTopMode(bool value)
        {
            IsOnTop = value;
            
            var window = WindowApi.GetActiveWindow();
            
            var isOnTopPtr = value
                ? (IntPtr) WindowApi.SpecialWindowHandles.HWND_TOPMOST
                : (IntPtr) WindowApi.SpecialWindowHandles.HWND_NOTOPMOST;
            
            WindowApi.SetWindowPos(window,
                isOnTopPtr,
                0, 0, 0, 0,
                WindowApi.SetWindowPosFlags.SWP_NOMOVE | WindowApi.SetWindowPosFlags.SWP_NOSIZE);
        }
        
        private void NotifyAboutStateOfOnTopMode()
        {
            var notificationText = IsOnTop
                ? NotificationTexts.OnTopModeEnabled
                : NotificationTexts.OnTopModeDisabled;
            
            MessageDispatcher.SendMessageData(MessageType.NotificationDisplay,
                new NotificationInfo(NotificationType.Log, notificationText),
                EnumMessageDelay.IMMEDIATE);
        }
        
        private void OnTopModeStateToggled(bool value)
        {
            ConfirmOnTopMode(!IsOnTop);
            NotifyAboutStateOfOnTopMode();
        }

        private void OnApplicationStarted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);
            
            if (_options.IsOnTopByDefault)
                ConfirmOnTopMode(true);
            
            _triggerSwitch.AddListener(_triggerShortcutKeys1, OnTopModeStateToggled);
            _triggerSwitch.AddListener(_triggerShortcutKeys2, OnTopModeStateToggled);
        }
    }
}