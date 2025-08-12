using System;
using UnityEngine;
using UnityEngine.UI;

namespace it.UI.Custom.Scroll
{
    public class RadialScroll : MonoBehaviour
    {
        [SerializeField] private GameObject[] _elements;
        
        [Range(-500, 500), SerializeField] private int _elementOffset;
        [Range(0f, 20f), SerializeField] private float _snapSpeed;
        [Range(0f, 10f), SerializeField] private float _scaleOffset;
        [Range(1f, 20f), SerializeField] private float _scaleSpeed;
        [Range(0f, 10f), SerializeField] private float _alphaOffset;
        [Range(1f, 20f), SerializeField] private float _alphaSpeed;
        
        [SerializeField] private bool _isHorizontal;
        [SerializeField] private bool _isScaleChange;
        [SerializeField] private bool _isAlphaChange;
        
        [SerializeField] private ScrollRect _scrollRect;
        
        private Vector2[] elementPositions;
        private Vector3[] elementScales;
        private CanvasGroup[] elementCanvasGroups;

        private RectTransform contentRect;
        private Vector2 contentVector;

        private int selectedElementID;
        private bool isScrolling;

        public event Action<int> OnSelectIndexChanged;

        public int countElements => _elements.Length;
        public int currentIndex => selectedElementID;

        private void Start()
        {
            contentRect = GetComponent<RectTransform>();
            elementPositions = new Vector2[_elements.Length];
            elementScales = new Vector3[_elements.Length];
            elementCanvasGroups = new CanvasGroup[_elements.Length];
            for (int i = 0; i < _elements.Length; i++)
            {
                if (_elements[i].GetComponent<CanvasGroup>() == null)
                {
                    elementCanvasGroups[i] = _elements[i].AddComponent<CanvasGroup>();
                }
                else
                {
                    elementCanvasGroups[i] = _elements[i].GetComponent<CanvasGroup>();
                }

                if (i == 0) continue;
                
                if (_isHorizontal)
                {
                    _elements[i].transform.localPosition = new Vector2(
                        _elements[i - 1].transform.localPosition.x +
                        _elements[i].GetComponent<RectTransform>().sizeDelta.x + _elementOffset,
                        _elements[i].transform.localPosition.y);
                }
                else
                {
                    _elements[i].transform.localPosition = new Vector2(
                        _elements[i].transform.localPosition.x,
                        _elements[i - 1].transform.localPosition.y +
                        _elements[i].GetComponent<RectTransform>().sizeDelta.y + _elementOffset);
                }
                
                elementPositions[i] = -_elements[i].transform.localPosition;
            }
        }

        private void FixedUpdate()
        {
            if (_isHorizontal)
            {
                if (contentRect.anchoredPosition.x >= elementPositions[0].x && !isScrolling ||
                    contentRect.anchoredPosition.x <= elementPositions[elementPositions.Length - 1].x && !isScrolling)
                    _scrollRect.inertia = false;
            }
            else
            {
                if (contentRect.anchoredPosition.y >= elementPositions[0].y && !isScrolling ||
                    contentRect.anchoredPosition.y <= elementPositions[elementPositions.Length - 1].y && !isScrolling)
                    _scrollRect.inertia = false;
            }

            float nearestPos = float.MaxValue;
            int id = selectedElementID;

            for (int i = 0; i < _elements.Length; i++)
            {
                float distance = _isHorizontal
                    ? Mathf.Abs(contentRect.anchoredPosition.x - elementPositions[i].x)
                    : Mathf.Abs(contentRect.anchoredPosition.y - elementPositions[i].y);

                if (distance < nearestPos)
                {
                    nearestPos = distance;
                    selectedElementID = i;
                }

                if (_isScaleChange)
                {
                    float scale = Mathf.Clamp(1 / (distance / Mathf.Abs(_elementOffset)) * _scaleOffset, 0.01f, 1f);
                    elementScales[i].x = Mathf.SmoothStep(_elements[i].transform.localScale.x, scale,
                        _scaleSpeed * Time.fixedDeltaTime);
                    elementScales[i].y = Mathf.SmoothStep(_elements[i].transform.localScale.y, scale,
                        _scaleSpeed * Time.fixedDeltaTime);
                    elementScales[i].z = 1;
                    _elements[i].transform.localScale = elementScales[i];
                }

                if (_isAlphaChange)
                {
                    float alpha = Mathf.Clamp(1 / (distance / Math.Abs(_elementOffset)) * _alphaOffset, 0.5f, 1f);
                    elementCanvasGroups[i].alpha =
                        Mathf.SmoothStep(elementCanvasGroups[i].alpha, alpha, _alphaSpeed * Time.fixedDeltaTime);
                }
            }

            float scrollVelocity = _isHorizontal ? Mathf.Abs(_scrollRect.velocity.x) : Mathf.Abs(_scrollRect.velocity.y);

            if (scrollVelocity < 400 && !isScrolling) _scrollRect.inertia = false;
            
            if (id != selectedElementID) OnSelectIndexChanged?.Invoke(selectedElementID);

            if (isScrolling || scrollVelocity > 400) return;

            if (_isHorizontal)
                contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, elementPositions[selectedElementID].x,
                    _snapSpeed * Time.fixedDeltaTime);
            else
                contentVector.y = Mathf.SmoothStep(contentRect.anchoredPosition.y, elementPositions[selectedElementID].y,
                    _snapSpeed * Time.fixedDeltaTime);

            contentRect.anchoredPosition = contentVector;
        }

        public GameObject GetSelectedObject()
        {
            return _elements.Length >= selectedElementID ? _elements[selectedElementID].gameObject : null;
        }

        public GameObject[] GetScrollObjects()
        {
            return _elements;
        }

        public void SetIndexSelectObject(int index)
        {
            if (index <= _elements.Length)
            {
                selectedElementID = index;
                OnSelectIndexChanged?.Invoke(selectedElementID);

                if (_isHorizontal)
                {
                    if (contentRect.anchoredPosition.x >= elementPositions[0].x && !isScrolling ||
                        contentRect.anchoredPosition.x <= elementPositions[elementPositions.Length - 1].x && !isScrolling)
                        _scrollRect.inertia = false;
                }
                else
                {
                    if (contentRect.anchoredPosition.y >= elementPositions[0].y && !isScrolling ||
                        contentRect.anchoredPosition.y <= elementPositions[elementPositions.Length - 1].y && !isScrolling)
                        _scrollRect.inertia = false;
                }

                float scrollVelocity =
                    _isHorizontal ? Mathf.Abs(_scrollRect.velocity.x) : Mathf.Abs(_scrollRect.velocity.y);

                if (scrollVelocity < 400 && !isScrolling) _scrollRect.inertia = false;

                if (isScrolling || scrollVelocity > 400) return;

                if (_isHorizontal)
                    contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, elementPositions[selectedElementID].x,
                        _snapSpeed * Time.fixedDeltaTime);
                else
                    contentVector.y = Mathf.SmoothStep(contentRect.anchoredPosition.y, elementPositions[selectedElementID].y,
                        _snapSpeed * Time.fixedDeltaTime);

                for (int i = 0; i < _elements.Length; i++)
                {
                    float distance = _isHorizontal
                        ? Mathf.Abs(contentRect.anchoredPosition.x - elementPositions[i].x)
                        : Mathf.Abs(contentRect.anchoredPosition.y - elementPositions[i].y);

                    if (_isScaleChange)
                    {
                        float scale = Mathf.Clamp(1 / (distance / Math.Abs(_elementOffset)) * _scaleOffset, 0.5f, 1f);
                        elementScales[i].x = Mathf.SmoothStep(_elements[i].transform.localScale.x, scale,
                            _scaleSpeed * Time.fixedDeltaTime);
                        elementScales[i].y = Mathf.SmoothStep(_elements[i].transform.localScale.y, scale,
                            _scaleSpeed * Time.fixedDeltaTime);
                        elementScales[i].z = 1;
                        _elements[i].transform.localScale = elementScales[i];
                    }

                    if (_isAlphaChange)
                    {
                        float alpha = Mathf.Clamp(1 / (distance / Math.Abs(_elementOffset)) * _alphaOffset, 0.5f, 1f);
                        elementCanvasGroups[i].alpha =
                            Mathf.SmoothStep(elementCanvasGroups[i].alpha, alpha, _alphaSpeed * Time.fixedDeltaTime);
                    }
                }

                contentRect.anchoredPosition = contentVector;
            }
        }

        public void Scrolling(bool scroll)
        {
            isScrolling = scroll;
            if (scroll) _scrollRect.inertia = true;
        }
    }
}