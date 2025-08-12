using System;
using UI.Switches.Triggers.Data.Enums.Attributes;
using UnityEngine;

namespace UI.Switches.Triggers.Data.Enums
{
    [Serializable]
    public enum TriggerType
    {
        Default,
        [Trigger(false, typeof(CustomTrigger))]
        CustomWindowPanels,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.F8, false)]
        WindowPanels,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha1, true)]
        StatusColumn1,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha2, false)]
        StatusColumn2,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha3, false)]
        StatusColumn3,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha4, false)]
        StatusColumn4,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha5, false)]
        StatusColumn5,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha6, false)]
        StatusColumn6,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha7, false)]
        StatusColumn7,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha8, false)]
        StatusColumn8,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha9, false)]
        StatusColumn9,
        [Trigger(true, typeof(KeyboardTrigger), KeyCode.Alpha0, false)]
        StatusColumn10,
        [Trigger(true, typeof(KeyboardShortcutTrigger), KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.W, true)]
        WindowsOnTop1,
        [Trigger(true, typeof(KeyboardShortcutTrigger), KeyCode.RightControl, KeyCode.RightShift, KeyCode.W, true)]
        WindowsOnTop2
    }
}