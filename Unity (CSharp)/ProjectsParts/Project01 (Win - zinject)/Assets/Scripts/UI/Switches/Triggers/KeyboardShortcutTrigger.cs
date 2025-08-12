using UnityEngine;

namespace UI.Switches.Triggers
{
    public class KeyboardShortcutTrigger : TriggerBase, ITrigger
    {
        private readonly KeyCode[] _keyCodes;
        
        public override bool Charged
        {
            get
            {
                var keysCount = _keyCodes.Length;
                
                if (keysCount == 0)
                    return false;
                
                var isAnyKeyDown = false;
                
                for (var i = 0; i < keysCount; i++)
                {
                    var keyCode = _keyCodes[i];
                    
                    if (Input.GetKeyDown(keyCode))
                    {
                        isAnyKeyDown = true;
                        continue;
                    }
                    
                    if (Input.GetKey(keyCode)) 
                        continue;

                    return false;
                }
                
                return isAnyKeyDown;
            }
        }

        public KeyboardShortcutTrigger(KeyCode first, KeyCode second, bool enabled) : base(enabled)
        {
            _keyCodes = new [] { first, second };
        }
        
        public KeyboardShortcutTrigger(KeyCode first, KeyCode second, KeyCode third, bool enabled) : base(enabled)
        {
            _keyCodes = new [] { first, second, third };
        }
        
        public KeyboardShortcutTrigger(KeyCode first, KeyCode second, KeyCode third, KeyCode forth, bool enabled) : base(enabled)
        {
            _keyCodes = new [] { first, second, third, forth };
        }
    }
}