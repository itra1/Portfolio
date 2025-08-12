using System.Collections.Generic;
using System.Linq;
using Game.Scripts.UI.Presenters.Interfaces;
using UnityEngine;

namespace Game.Scripts.UI.Components
{
	public class CarouselIndicatorsPanel : MonoBehaviour, IUiVisibleHandler
	{
		[SerializeField] private Carousel _carousel;
		[SerializeField] private CarouselIndicator _indicatorPrefab;
		[SerializeField] private RectTransform _content;
		[SerializeField] private float _forwardDistance = 59;
		[SerializeField] private float _itemDistance = 40;

		private List<CarouselIndicator> _indicatorsList = new();

		private float DistanceDelta => _forwardDistance - _itemDistance;

		public void Show()
		{
			MakeInstances();
			_carousel.OnDragDeltaEvent.AddListener(OnCarouselRotate);
		}

		public void Hide()
		{
			_carousel.OnDragDeltaEvent.RemoveListener(OnCarouselRotate);
		}

		private void OnCarouselRotate()
		{
			var carouselProportion = _carousel.Content.anchoredPosition.x / _carousel.ItemDistance;
			var indicatorsProportion = carouselProportion * _itemDistance;
			_content.anchoredPosition = new(indicatorsProportion, _content.anchoredPosition.y);
			ConfirmItems();
		}

		private void ConfirmItems()
		{
			for (int i = 0; i < _indicatorsList.Count; i++)
			{
				var currentItem = _indicatorsList[i];
				var currentAnchorDelta = _content.anchoredPosition.x + currentItem.RectTransform.anchoredPosition.x;

				var absDeltaItem = Mathf.Abs(currentAnchorDelta);

				if (absDeltaItem < _itemDistance)
				{
					var prop = currentAnchorDelta / _itemDistance;

					_ = _content.anchoredPosition.x + currentItem.RectTransform.anchoredPosition.x;
					currentItem.SetLight(Mathf.Abs(prop));
					currentItem.SetSize(1);
				}
				else
				{
					currentItem.SetLight(1);
					currentItem.SetSize(1);
				}
			}
			Reposition();
		}

		private void Reposition()
		{
			bool existsReposition = false;

			var minPositionElement = _indicatorsList[0];
			var maxPositionElement = _indicatorsList[_indicatorsList.Count - 1];


			if (minPositionElement.RectTransform.anchoredPosition.x + _content.anchoredPosition.x > -_itemDistance * 6)
			{
				maxPositionElement.RectTransform.anchoredPosition = new Vector2(
				minPositionElement.RectTransform.anchoredPosition.x - _itemDistance,
				maxPositionElement.RectTransform.anchoredPosition.y
				);
				existsReposition = true;
			}

			if (_content.anchoredPosition.x + maxPositionElement.RectTransform.anchoredPosition.x < _itemDistance * 9)
			{
				minPositionElement.RectTransform.anchoredPosition = new Vector2(
				maxPositionElement.RectTransform.anchoredPosition.x + _itemDistance,
				minPositionElement.RectTransform.anchoredPosition.y
				);
				existsReposition = true;
			}

			if (existsReposition)
				OrderPosition();
		}

		private void MakeInstances()
		{
			_indicatorPrefab.gameObject.SetActive(false);
			_indicatorsList.ForEach(x => x.gameObject.SetActive(false));

			var center = MakeIndicator(0, 0);
			center.SetLight(0);
			center.SetSize(1);

			for (int i = 0; i < 5; i++)
			{
				_ = MakeIndicator(i, 1);
			}
			for (int i = 0; i < 5; i++)
			{
				_ = MakeIndicator(i, -1);
			}
			OrderPosition();
		}

		private CarouselIndicator MakeIndicator(int index, int incrementSign)
		{
			CarouselIndicator inst = _indicatorsList.Find(x => !x.gameObject.activeSelf);
			inst ??= Instantiate(_indicatorPrefab, _content);
			inst.gameObject.SetActive(true);
			var rectTransform = inst.transform as RectTransform;
			rectTransform.anchoredPosition = new((_itemDistance * incrementSign) + _itemDistance * index * incrementSign, 0);
			inst.SetSizeOwerPosition(1);
			_indicatorsList.Add(inst);
			return inst;
		}

		private void OrderPosition()
		{
			_indicatorsList = _indicatorsList.OrderBy(x => x.RectTransform.anchoredPosition.x).ToList();
		}
	}
}
