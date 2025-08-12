using Core.UI.Timers.Data;
using TMPro;
using UI.Timers.Controller.Items.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = Core.Logging.Debug;

namespace UI.Timers.Presenter.Widgets
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public abstract class TimerWidgetBase : MonoBehaviour, ITimerWidget, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        protected const string DefaultLabelText = "00";
        
        [SerializeField] private Image[] _images;
        [SerializeField] private TextMeshProUGUI[] _colorizedTexts;
        
        private RectTransform _rectTransform;
        private Camera _mainCamera;
        private float _distanceToScreen;
        private Vector3 _offset;
        private bool _isDragging;
        
        private RectTransform RectTransform =>
            _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        private RectTransform Parent => RectTransform.parent as RectTransform;
        
        private Vector2 NormalizePosition
        {
            get
            {
                var parent = Parent;

                if (parent == null)
                {
                    Debug.LogError("Parent is not found when trying to get relative position of timer widget");
                    return default;
                }
                
                var parentRect = parent.rect;
                
                var rectTransform = RectTransform;
                var rect = rectTransform.rect;
                
                var areaWidth = parentRect.width - rect.width;
                var areaHeight = parentRect.height - rect.height;
                
                var anchoredPosition = rectTransform.anchoredPosition;
                
                return new Vector2((anchoredPosition.x + areaWidth * 0.5f) / areaWidth,
                    (anchoredPosition.y + areaHeight * 0.5f) / areaHeight);
            }
            set
            {
                var parent = Parent;
                
                if (parent == null)
                {
                    Debug.LogError("Parent is not found when trying to set relative position of timer widget");
                    return;
                }
                
                var parentRect = parent.rect;
                
                var rectTransform = RectTransform;
                var rect = rectTransform.rect;
                
                var areaWidth = parentRect.width - rect.width;
                var areaHeight = parentRect.height - rect.height;
                
                rectTransform.anchoredPosition = new Vector2(value.x * areaWidth - areaWidth * 0.5f, 
                    value.y * areaHeight - areaHeight * 0.5f);
            }
        }

        public abstract TimerType Type { get; }
        
        public ITimerInfo Info { get; private set; }
        
        public bool Visible => Info is { Visible: true } && gameObject.activeSelf;
        
        public void Initialize(ITimerInfo info)
        {
            if (info == null)
            {
                Debug.LogError($"An attempt was detected to assign null info when trying to initialize {GetType().Name}");
                return;
            }
            
            if (info.Type != Type)
            {
                Debug.LogError($"Timer type mismatch detected when trying to initialize {GetType().Name}. Expected timer type: {Type}. Assigned timer type: {info.Type}");
                return;
            }
            
            Info = info;
            
            _mainCamera = Camera.main;
            
            Reset();
            
            UpdatePosition();
            UpdateColor();
        }
        
        public virtual bool Show()
        {
            if (Visible)
                return false;
            
            Refresh();
            
            gameObject.SetActive(true);
            Info.Visible = true;
            
            return true;
        }

        public abstract void Refresh();
        
        public bool Hide()
        {
            if (!Visible)
                return false;
            
            gameObject.SetActive(false);
            Info.Visible = false;
            
            Reset();
            
            return true;
        }

        protected abstract void Reset();
        
        protected void SetColor(Color color)
        {
            for (var i = _images.Length - 1; i >= 0; i--)
            {
                var image = _images[i];
                var c = color;
                c.a = image.color.a;
                _images[i].color = c;
            }
            
            for (var i = _colorizedTexts.Length - 1; i >= 0; i--)
            {
                var text = _colorizedTexts[i];
                var c = color;
                c.a = text.color.a;
                _colorizedTexts[i].color = c;
            }
        }
        
        protected void UpdatePosition()
        {
            if (!_isDragging)
                NormalizePosition = new Vector2(Info.X, Info.Y);
        }

        protected void UpdateColor()
        {
            var color = Info.Color;
            
            if (!string.IsNullOrEmpty(color))
            {
                //TODO: Need to add color changes to the timer widget later
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            
            var mousePosition = Input.mousePosition;
            var position = RectTransform.position;
            
            _distanceToScreen = _mainCamera.WorldToScreenPoint(position).z;
            _offset = position - _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _distanceToScreen));
        }

        public void OnDrag(PointerEventData eventData)
        {
            var mousePosition = Input.mousePosition;
            var position = _mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _distanceToScreen)) + _offset;
            
            RectTransform.position = position;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            var normalizePosition = NormalizePosition;
            
            Info.SetPosition(normalizePosition.x, normalizePosition.y);
            
            _distanceToScreen = 0f;
            _offset = default;
            
            _isDragging = false;
        }
    }
}