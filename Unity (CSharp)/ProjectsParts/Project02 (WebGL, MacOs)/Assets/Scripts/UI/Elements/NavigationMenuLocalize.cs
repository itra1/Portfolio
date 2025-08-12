using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace it.UI.Elements
{
	public class NavigationMenuLocalize : MonoBehaviour
	{
		[SerializeField] private ScrollRect _scroll;
		[SerializeField] private float _borderXsize = 10;
		[SerializeField] private float _spaceBetween = 1;
		[SerializeField] private NavigationItem[] _navigations;

		[System.Serializable]
		public struct NavigationItem
		{
			public TextMeshProUGUI Label;
		}

		private void Awake()
		{
			ConfirmPositions();

			var buttons = GetComponentsInChildren<FilterSwitchButtonUI>();
			for(int i = 0; i < buttons.Length;i++){
				int index = i;
				buttons[index].OnSelectAction = () =>
				{
					MoveScroll(buttons[index].GetComponent<RectTransform>());
				};
			}

		}

		private void MoveScroll(RectTransform buttonRect)
		{
			if (_scroll == null) return;
			
			float leftXDelta = _scroll.content.anchoredPosition.x + buttonRect.anchoredPosition.x;
			float rightXDelta = _scroll.content.anchoredPosition.x + buttonRect.anchoredPosition.x + buttonRect.rect.width;
			if (leftXDelta < 0){
				_scroll.content.DOAnchorPos(new Vector2(_scroll.content.anchoredPosition.x - leftXDelta, _scroll.content.anchoredPosition.y), 0.2f);
			}
			if (rightXDelta > _scroll.viewport.rect.width)
			{
				_scroll.content.DOAnchorPos(new Vector2(_scroll.content.anchoredPosition.x - (rightXDelta - _scroll.viewport.rect.width), _scroll.content.anchoredPosition.y), 0.2f);
			}
		}

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizationChange);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.LocalizationChange, LocalizationChange);
			ConfirmPositions();
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizationChange);
		}
		private void LocalizationChange(com.ootii.Messages.IMessage handler)
		{
			ConfirmPositions();
		}

		private void ConfirmPositions()
		{
			StartCoroutine(Positing());
		}
		IEnumerator Positing()
		{
			yield return null;
			SetPositions();

		}

		[ContextMenu("Set positions")]
		public void SetPositions()
		{

			RectTransform _oldRect = null;

			for (int i = 0; i < _navigations.Length; i++)
			{
				RectTransform bRect = _navigations[i].Label.transform.parent.GetComponent<RectTransform>();

				if (_oldRect != null)
				{
					bRect.anchoredPosition = new Vector2(_oldRect.anchoredPosition.x + _oldRect.rect.width + _spaceBetween, bRect.anchoredPosition.y);
				}

				bRect.sizeDelta = new Vector2(_navigations[i].Label.preferredWidth + (2 * _borderXsize), bRect.sizeDelta.y);

				_oldRect = bRect;
			}

			if (_scroll != null)
				_scroll.content.sizeDelta = new Vector2((_oldRect.anchoredPosition.x + _oldRect.rect.width) - _scroll.viewport.rect.width, _scroll.content.sizeDelta.y);
		}

	}
}