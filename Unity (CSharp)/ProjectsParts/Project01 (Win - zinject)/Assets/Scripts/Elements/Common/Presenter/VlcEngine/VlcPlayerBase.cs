using System;
using Core.Elements.Windows.Camera.Data;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Elements.Common.Presenter.VlcEngine
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public abstract class VlcPlayerBase : MonoBehaviour
    {
        [SerializeField] private RawImage _image;
        
        private RectTransform _rectTransform;
        
        protected RawImage Image => _image;
        
        public event Action Disposed;
        
        public bool IsInitialized { get; private set; }
        public bool IsDisposed { get; private set; }

        public virtual bool IsDisplayed { get; protected set; }

        public CameraMaterialData Material { get; private set; }
        
        public RectTransform RectTransform => 
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        protected void Initialize() => IsInitialized = true;

        public void SetMaterial(CameraMaterialData material) => Material = material;
        
        public void SetName(string value) => gameObject.name = value;
        
        public void SetParent(RectTransform parent) => RectTransform.SetParent(parent);
        
        public void AlignToParent()
        {
            var rectTransform = RectTransform;
            
            rectTransform.ResetAnchors(Vector2.one * 0.5f);
            rectTransform.Reset();
        }
        
        public virtual bool Show()
        {
            if (gameObject.activeSelf)
                return false;
            
            gameObject.SetActive(true);
            return true;
        }
        
        public virtual bool Hide()
        {
            try
            {
                if (!gameObject.activeSelf)
                    return false;
                
                gameObject.SetActive(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual void Dispose()
        {
            try
            {
                Destroy(gameObject);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Disposed?.Invoke();
                IsDisposed = true;
            }
        }
    }
}