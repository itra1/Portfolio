using System;
using System.Threading;
using Base;
using com.ootii.Messages;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Preview.Stages.Items;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Utils;
using Debug = Core.Logging.Debug;

namespace Preview.Stages
{
    [DisallowMultipleComponent]
    public class PreviewSnapshotMaker : MonoBehaviour, IPreviewSnapshotMaker
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private HiddenCanvasGroupItem[] _itemsToHide;
        [SerializeField] private CanvasGroupItem[] _itemsToShow;
        
        private Rect _screenRect;
        
        private void Awake() => _screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
        
        private void OnEnable() => 
            MessageDispatcher.AddListener(MessageType.HiddenCanvasGroupItemAdd, OnHiddenCanvasGroupItemAdded);

        private void OnDisable() => 
            MessageDispatcher.RemoveListener(MessageType.HiddenCanvasGroupItemAdd, OnHiddenCanvasGroupItemAdded);

        public async UniTask<RenderTexture> MakeAsync(RectTransform rectTransform,
            CancellationToken cancellationToken,
            float? maxHeight = null)
        {
            if (_camera == null)
            {
                Debug.LogError("Camera is missing when trying to make a snapshot");
                return null;
            }
            
            if (rectTransform == null)
            {
                Debug.LogError("Rect transform is null when trying to make a snapshot");
                return null;
            }
            
            if (!rectTransform.gameObject.activeInHierarchy) 
                return null;
            
            var rect = CalculateRect(rectTransform);
            
            await UniTask.WaitForEndOfFrame(this, cancellationToken);
            
            var components = rectTransform.GetComponentsInChildren<IAdaptiveForPreview>();
            
            PrepareBefore(components, rectTransform);
            
            try
            {
                return GenerateSnapshotTexture(rect, maxHeight);
            }
            finally
            {
                RestoreAfter(components);
            }
        }
        
        private RenderTexture GenerateSnapshotTexture(in Rect rect, in float? maxHeight = null)
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            
            var temporaryTexture = RenderTexture.GetTemporary(screenWidth, 
                screenHeight, 
                24, 
                GraphicsFormat.R8G8B8A8_SRGB);
            
            var currentTexture = _camera.targetTexture;
            
            _camera.targetTexture = temporaryTexture;
            
            _camera.Render();
            
            RenderTexture.active = temporaryTexture;
            
            var position = rect.position;
            var size = rect.size;
            var width = size.x;
            var height = size.y;
            
            var multiplier = height > maxHeight ? maxHeight / height : 1.0f;
            
            var texture = new RenderTexture((int) (width * multiplier),
                (int) (height * multiplier), 
                24, 
                GraphicsFormat.R8G8B8A8_SRGB, 
                0);
            
            var scale = new Vector2(width / screenWidth, height / screenHeight);
            var offset = new Vector2(position.x / screenWidth, position.y / screenHeight);
            
            Graphics.Blit(temporaryTexture, texture, scale, offset);
            
            RenderTexture.active = null;
            
            _camera.targetTexture = currentTexture;
            
            RenderTexture.ReleaseTemporary(temporaryTexture);
            
            return texture;
        }
        
        private Rect CalculateRect(RectTransform rectTransform)
        {
            var pivot = rectTransform.pivot;
            var rect = rectTransform.rect;
            
            var pivotX = pivot.x;
            var pivotY = pivot.y;
            var width = rect.width;
            var height = rect.height;
            
            var bottomLeftPoint = new Vector3(-width * pivotX, -height * pivotY);
            var topLeftPoint = new Vector3(-width * pivotX, height * (1f - pivotY));
            var topRightPoint = new Vector3(width * (1f - pivotX), height * (1f - pivotY));
            var bottomRightPoint = new Vector3(width * (1f - pivotX), -height * pivotY);
            
            unsafe
            {
                Span<Vector3> points = stackalloc[]
                {
                    bottomLeftPoint,
                    topLeftPoint,
                    topRightPoint,
                    bottomRightPoint
                };
                
                rectTransform.TransformPoints(points);
                
                bottomLeftPoint = _camera.WorldToScreenPoint(points[0]);
                topLeftPoint = _camera.WorldToScreenPoint(points[1]);
                //topRightPoint = _camera.WorldToScreenPoint(points[2]);
                bottomRightPoint = _camera.WorldToScreenPoint(points[3]);
            }
            
            return _screenRect.Intersect(new Rect(bottomLeftPoint.x, bottomLeftPoint.y, 
                bottomRightPoint.x - bottomLeftPoint.x, topLeftPoint.y - bottomLeftPoint.y));
        }
        
        private void AttemptToSetAlphaIfFound(CanvasGroup group, float alpha)
        {
            for (var i = _itemsToHide.Length - 1; i >= 0; i--)
            {
                var item = _itemsToHide[i];
                
                if (item.IsContextEqualTo(group))
                    item.AttemptToSetAlpha(alpha);
            }
        }
        
        private void PrepareBefore(IAdaptiveForPreview[] components, RectTransform rectTransform)
        {
            if (rectTransform.TryGetComponent<CanvasGroup>(out var group))
                AttemptToSetAlphaIfFound(group, 1f);
            
            foreach (var target in rectTransform.GetComponentsInParent<CanvasGroup>())
                AttemptToSetAlphaIfFound(target, 1f);
            
            for (var i = _itemsToHide.Length - 1; i >= 0; i--)
            {
                var item = _itemsToHide[i];
                
                if (!item.IsShownIfDescendant || !rectTransform.IsAncestorFor(item.RectTransform))
                    item.AttemptToSetAlpha(0f);
            }
            
            for (var i = _itemsToShow.Length - 1; i >= 0; i--)
                _itemsToShow[i].AttemptToSetAlpha(1f);
            
            for (var i = components.Length - 1; i >= 0; i--)
                components[i].AttemptToSetAlpha(0f);
        }
        
        private void RestoreAfter(IAdaptiveForPreview[] components)
        {
            for (var i = components.Length - 1; i >= 0; i--)
                components[i].AttemptToRestoreAlpha();
            
            for (var i = _itemsToHide.Length - 1; i >= 0; i--)
                _itemsToHide[i].AttemptToRestoreAlpha();
            
            for (var i = _itemsToShow.Length - 1; i >= 0; i--)
                _itemsToShow[i].AttemptToRestoreAlpha();
        }
        
        private void OnHiddenCanvasGroupItemAdded(IMessage message)
        {
            var target = (CanvasGroup) message.Data;
            
            for (var i = _itemsToHide.Length - 1; i >= 0; i--)
            {
                if (_itemsToHide[i].IsContextEqualTo(target))
                    return;
            }
            
            Array.Resize(ref _itemsToHide, _itemsToHide.Length + 1);
            
            _itemsToHide[^1] = new HiddenCanvasGroupItem(target);
        }
    }
}