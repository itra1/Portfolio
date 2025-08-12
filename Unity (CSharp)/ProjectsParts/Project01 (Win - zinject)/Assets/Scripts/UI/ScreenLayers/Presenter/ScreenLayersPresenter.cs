using com.ootii.Messages;
using Core.Messages;
using Core.Options.Offsets;
using UI.Base.Presenter;
using UnityEngine;
using Utils;

namespace UI.ScreenLayers.Presenter
{
    [RequireComponent(typeof(RectTransform))]
    public class ScreenLayersPresenter : HiddenPresenterBase, IScreenLayersPresenter
    {
        private IScreenOffsets _screenOffsets;
        
        private RectTransform _rectTransform;
        
        public RectTransform RectTransform => 
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public void Initialize(IScreenOffsets screenOffsets)
        {
            _screenOffsets = screenOffsets;
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }

        public override void Unload()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            base.Unload();
        }
        
        private void AlignToParent()
        {
            var rectTransform = RectTransform;
			
            rectTransform.ResetAnchors(Vector2.one * 0.5f);
            rectTransform.Reset();
            
            var sizeDeltaX = -(_screenOffsets.Left + _screenOffsets.Right);
            var sizeDeltaY = -(_screenOffsets.Bottom + _screenOffsets.Top);
            var anchoredPositionX = sizeDeltaX * 0.5f;
            var anchoredPositionY = -sizeDeltaY * 0.5f;
			
            rectTransform.anchoredPosition = new Vector2(anchoredPositionX, anchoredPositionY);
            rectTransform.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
        }
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            AlignToParent();
            
            if (!gameObject.activeSelf) 
                gameObject.SetActive(true);
        }
    }
}