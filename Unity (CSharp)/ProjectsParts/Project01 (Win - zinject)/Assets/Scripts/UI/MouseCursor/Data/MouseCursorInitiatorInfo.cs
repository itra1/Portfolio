using System;
using Settings.Data;
using UI.MouseCursor.Presenter.Components;

namespace UI.MouseCursor.Data
{
#if UNITY_EDITOR
    [Serializable]
#endif
    public struct MouseCursorInitiatorInfo
    {
        public IMouseCursorUpdater Initiator;
        public MouseCursorState State;

#if UNITY_EDITOR
        public override string ToString() => $"{{priority: {Initiator.Priority}, name: {Initiator.Name}, state: {State}}}";
#endif
    }
}