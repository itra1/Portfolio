using UnityEngine;

namespace UI.Switches.Triggers
{
    public class KeyboardTrigger : TriggerBase, ITrigger
    {
        private readonly KeyCode _keyCode;
        
        public override bool Charged => Input.GetKeyDown(_keyCode);
        
        public KeyboardTrigger(KeyCode keyCode, bool enabled) : base(enabled)
        {
            _keyCode = keyCode;
        }
    }
}