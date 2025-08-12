using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace it.UI.Elements
{
	public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[HideInInspector] public UnityEngine.Events.UnityEvent PointerEnterEvent = new UnityEngine.Events.UnityEvent();
		[HideInInspector] public UnityEngine.Events.UnityEvent PointerExitEvent = new UnityEngine.Events.UnityEvent();
		[SerializeField] private RectTransform _target;
		[SerializeField] private Vector3 _scale = new Vector3(1.05f, 1.05f, 1.05f);
		[SerializeField] private float _time = 0.3f;

		private Vector3 _startScale;

		private void Awake()
		{
			_startScale = _target.localScale;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_target.DOScale(_scale, _time);
			PointerEnterEvent?.Invoke();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_target.DOScale(_startScale, _time);
			PointerExitEvent?.Invoke();
		}

	}
}