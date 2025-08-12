using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Elements.StatusColumn.Presenter.Components
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class StatusHeaderTab : MonoBehaviour, IStatusHeaderTab
    {
        [SerializeField] private GameObject _background;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private RectTransform _titleRectTransform;
        [SerializeField] private float _maxTitleWidth;
        [SerializeField] private GameObject _separator;
        
        private RectTransform _rectTransform;
        
        public RectTransform RectTransform => 
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public Rect TitleRect { get; private set; }
        
        public bool IsBackgroundActive => _background.activeSelf;
        
        public void SetName(string value) => gameObject.name = value;
        
        public void SetParent(RectTransform parent) => RectTransform.SetParent(parent);
        
        public virtual void AlignToParent()
        {
            var rectTransform = RectTransform;
            
            rectTransform.ResetAnchors(new Vector2(0f, 0.5f));
            rectTransform.Reset();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((RectTransform) rectTransform.parent).rect.height);
        }

        public void SetBackgroundActive(bool value) => _background.SetActive(value);
        
        public void SetIconSprite(Sprite sprite) => _icon.sprite = sprite;
        
        public void SetTitleText(string text)
        {
            _title.text = text;
            _title.ForceMeshUpdate(true, true);
            
            var sizeDelta = _titleRectTransform.sizeDelta;
            var titleWidth = _title.preferredWidth > _maxTitleWidth ? _maxTitleWidth : _title.preferredWidth;
            
            TitleRect = new Rect(-sizeDelta.x, -sizeDelta.y, titleWidth, _titleRectTransform.rect.height);
        }
        
        public void SetSeparatorActive(bool value) => _separator.SetActive(value);
        
        public void Unload()
        {
            try
            {
                Destroy(gameObject);
            }
            catch
            {
                // ignored
            }
        }
    }
}