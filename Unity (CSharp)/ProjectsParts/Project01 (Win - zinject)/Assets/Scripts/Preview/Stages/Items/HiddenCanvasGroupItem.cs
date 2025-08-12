using System;
using UnityEngine;

namespace Preview.Stages.Items
{
    [Serializable]
    public class HiddenCanvasGroupItem : CanvasGroupItem
    {
        [SerializeField] private bool _isShownIfDescendant;
        
        private RectTransform _rectTransform;
        
        public bool IsShownIfDescendant => _isShownIfDescendant;
        
        public RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = Context.GetComponent<RectTransform>() : _rectTransform;
        
        public HiddenCanvasGroupItem(CanvasGroup context) : base(context) { }
    }
}