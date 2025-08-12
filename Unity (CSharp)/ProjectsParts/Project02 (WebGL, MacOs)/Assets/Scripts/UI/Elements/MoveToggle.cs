using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class MoveToggle : MonoBehaviour, IPointerClickHandler
	{
		public UnityEngine.Events.UnityEvent<bool> OnChangeValue;

		public bool IsOn
		{
			get => _isOn; set
			{

				_isOn = value;
				OnChangeValue?.Invoke(_isOn);

				if (_moveParentRect == null)
					_moveParentRect = _mover.parent.GetComponent<RectTransform>();
				if (_moveRect == null)
					_moveRect = _mover.GetComponent<RectTransform>();

				_moveRect.DOAnchorPos(new Vector2(_isOn ? _moveParentRect.rect.width / 2 : -_moveParentRect.rect.width / 2, 0), 0.1f);

				Color targetColor = Color.white;
				targetColor.a = _isOn ? 1 : 0;
				_activeBack.DOColor(targetColor, 0.1f);

			}
		}

		[SerializeField] private RectTransform _mover;
		[SerializeField] private Image _activeBack;

		private bool _isOn;
		private RectTransform _moveParentRect;
		private RectTransform _moveRect;

		public void OnPointerClick(PointerEventData eventData)
		{
			IsOn = !IsOn;

		}
	}
}