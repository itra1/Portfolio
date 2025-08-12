using Base.Presenter;
using com.ootii.Messages;
using Core.Messages;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.ShadedElements.Presenter.Base
{
    public abstract class ShadedElementsPresenterBase : PresenterBase
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image _image;
        [SerializeField] private float _animationDuration;
        [SerializeField, Range(0f, 1f)] private float _maxOpacity;
        
        public float AnimationDuration => _animationDuration;
        
        protected Image Image => _image;
        protected float MaxOpacity => _maxOpacity;
        
        [Inject]
        private void Initialize()
        {
            var typeName = GetType().Name;
			
            SetName(typeName.EndsWith(TypeNamePrefix)
                ? typeName[..^TypeNamePrefix.Length]
                : typeName);
            
            ResetImageToOriginalState();
        }
        
        private void Awake() => 
            MessageDispatcher.SendMessage(this, MessageType.HiddenCanvasGroupItemAdd, _group, EnumMessageDelay.IMMEDIATE);
        
        public override void Unload()
        {
            ResetImageToOriginalState();
            base.Unload();
        }

        protected void ResetImageToOriginalState()
        {
            var color = _image.color;
            color.a = 0f;
            _image.color = color;
            
            _image.enabled = false;
        }
    }
}