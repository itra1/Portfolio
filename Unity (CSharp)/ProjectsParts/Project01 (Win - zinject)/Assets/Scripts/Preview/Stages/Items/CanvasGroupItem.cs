using System;
using Base;
using UnityEngine;

namespace Preview.Stages.Items
{
    [Serializable]
    public class CanvasGroupItem : IAdaptiveForPreview
    {
        [SerializeField] private CanvasGroup _context;
        
        private float? _alpha;

        protected CanvasGroup Context => _context;
        
        public CanvasGroupItem(CanvasGroup context) => _context = context;
        
        public bool IsContextEqualTo(CanvasGroup context) => _context == context;
        
        public bool AttemptToSetAlpha(float value)
        {
            if (_alpha != null)
                return false;
            
            _alpha = _context.alpha;
            _context.alpha = value;
            
            return true;
        }
        
        public bool AttemptToRestoreAlpha()
        {
            if (_alpha == null)
                return false;
            
            _context.alpha = _alpha.Value;
            _alpha = null;
            
            return true;
        }
    }
}