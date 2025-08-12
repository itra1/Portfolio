using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace it.UI.Elements
{
	public class VerticalAccordionUI : MonoBehaviour
	{
		public event UnityEngine.Events.UnityAction OnSizeChange;
		[SerializeField] private RectTransform _rt;
		[SerializeField] private RectTransform _titleRect;
		[SerializeField] private Color _colorText;
		[SerializeField] private float _animateTime = 0.4f;
		[SerializeField] private TextButtonUI _buttonText;
		[SerializeField] private Image _border;

		private Image _titleImage;
		private TextMeshProUGUI _titleLabel;
		private Vector2 _startDeltaSize;

		private bool _isOpen = true;
		private void Awake()
		{
			_rt = GetComponent<RectTransform>();
			_startDeltaSize = _rt.sizeDelta;
			_titleImage = _titleRect.GetComponent<Image>();
			_titleLabel = _titleRect.GetComponentInChildren<TextMeshProUGUI>();
		}

		public void SetOpen(bool isOpen)
		{
			if (_isOpen == isOpen) return;

			_isOpen = isOpen;

			Color w = Color.white;
			w.a = 0;

			//_titleLabel.DOColor(_isOpen ? _colorText : Color.white, _animateTime);
			_titleImage.DOColor(_isOpen ? w : Color.white, _animateTime);
			_rt.DOSizeDelta(_isOpen ? _startDeltaSize : _titleRect.sizeDelta, _animateTime).OnUpdate(()=> {
				OnSizeChange?.Invoke();
			}).OnComplete(()=> {
				OnSizeChange?.Invoke();
			});
			_buttonText.StartColor = _isOpen ? _colorText : Color.white;
			if (!_isOpen)
				_buttonText.NormalState();

			Color borderColor = _border.color;
			borderColor.a = _isOpen ? 1 : 0;
			_border.DOColor(borderColor, _animateTime);
		}

		public void ToggleButton(){
			SetOpen(!_isOpen);
		}

	}
}