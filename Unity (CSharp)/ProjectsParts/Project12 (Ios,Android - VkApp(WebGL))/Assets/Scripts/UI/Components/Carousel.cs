using System.Collections.Generic;
using DG.Tweening;
using Engine.Scripts.Base;
using Engine.Scripts.Settings;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Songs.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class Carousel : MonoBehaviour, IInjection, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerExitHandler
	{
		[HideInInspector] public UnityEvent<bool> OnDragEvent = new();
		[HideInInspector] public UnityEvent OnDragDeltaEvent = new();
		[HideInInspector] public UnityEvent<RhythmTimelineAsset> OnSelectItem = new();

		[SerializeField] private CarouselIndicatorsPanel _indicatorsPanel;
		[HideInInspector][SerializeField] private List<SongBigUiElement> _elements = new();
		[SerializeField] private CarouselSideButton _leftButton;
		[SerializeField] private CarouselSideButton _rightButton;
		[SerializeField] private RectTransform _content;
		[SerializeField] private float _propertyOffset = 300;
		[SerializeField] private float _itemDistance = 230;
		[SerializeField] private float _timeMove = 0.4f;
		[SerializeField] private float _dragSpeed = 0.4f;

		private bool _isDrag;
		private Vector2 _pointDown;
		private Vector2 _dragContantPointDown;
		private DiContainer _container;
		private ISongsHelper _songHelper;
		private PrefabsLibrary _prefabsLibrary;

		public RectTransform Content => _content;
		public float ItemDistance => _itemDistance;

		[Inject]
		private void Constructor(DiContainer container, ISongsHelper songHelper, PrefabsLibrary prefabsLibrary)
		{
			_container = container;
			_songHelper = songHelper;
			_prefabsLibrary = prefabsLibrary;

			_leftButton.OnPointDown.AddListener(() => MoveTargetOffset(_content.anchoredPosition.x + _itemDistance));
			_rightButton.OnPointDown.AddListener(() => MoveTargetOffset(_content.anchoredPosition.x - _itemDistance));
		}

		public void Open()
		{
			ConfirmPropertyElements();
		}

		public void SpawnItems()
		{
			foreach (var item in _elements)
			{
				if (item != null)
					Destroy(item.gameObject);
			}

			_elements.Clear();

			var songsList = _songHelper.GetReadySongs();

			for (int i = 0; i < songsList.Count; i++)
			{
				var instant = Instantiate(_prefabsLibrary.SongBigUiElement, _content);

				instant.GetComponent<Image>().enabled = false;

				RectTransform instantRect = instant.transform as RectTransform;
				instantRect.localScale = Vector3.one;
				instantRect.anchorMin = Vector2.zero;
				instantRect.anchorMax = Vector2.one;
				instantRect.sizeDelta = Vector2.zero;
				instantRect.localPosition = Vector3.zero;
				instantRect.anchoredPosition = new Vector2(i * _itemDistance, instantRect.anchoredPosition.y);

				var component = instant.GetComponent<SongBigUiElement>();
				_container.Inject(component);

				component.SetData(songsList[i], _songHelper.GetCover(songsList[i].Uuid), _songHelper.GetScore(songsList[i].Uuid).Stars);

				component.SetCoverRaycastListener(false);

				_elements.Add(component);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!_isDrag)
				DeSelectAllEvements();

			_isDrag = true;

			_pointDown = eventData.position;
			_dragContantPointDown = _content.anchoredPosition;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			DragStop(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_isDrag)
				return;

			_content.anchoredPosition = new Vector2(
			_dragContantPointDown.x + (eventData.position.x - _pointDown.x) * _dragSpeed,
			_content.anchoredPosition.y
			);
			ConfirmPropertyElements();
			OnDragEvent?.Invoke(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			DragStop(eventData);
		}

		private void DragStop(PointerEventData eventData)
		{
			_isDrag = false;
			var targetOffset = Mathf.Round(_content.anchoredPosition.x / _itemDistance) * _itemDistance;
			MoveTargetOffset(targetOffset);
		}

		private void MoveTargetOffset(float offset)
		{
			OnDragEvent?.Invoke(true);

			var timeOffset = Mathf.Abs(offset - _content.anchoredPosition.x) / _itemDistance * 0.5f * _timeMove;

			_ = DOTween.To(() => _content.anchoredPosition, (x) => _content.anchoredPosition = x, new Vector2(offset, _content.anchoredPosition.y), timeOffset).OnUpdate(() =>
			{
				ConfirmPropertyElements();
			}).OnComplete(() =>
			{
				ConfirmPropertyElements();
				OnDragEvent?.Invoke(false);
				FindSelected();
			});
		}

		private void FindSelected()
		{
			foreach (var item in _elements)
			{
				var positionX = item.RectTransform.anchoredPosition.x + _content.anchoredPosition.x;
				if (Mathf.Abs(positionX) < 5)
					SelectElement(item.Song);
			}
		}

		public void SelectAndMoveElement(RhythmTimelineAsset song)
		{
			var targetElement = _elements.Find(x => x.Song == song);

			_content.anchoredPosition = new Vector2(-targetElement.RectTransform.anchoredPosition.x, _content.anchoredPosition.y);

			ConfirmPropertyElements();
			SelectElement(song);
		}

		private void SelectElement(RhythmTimelineAsset song)
		{
			OnSelectItem?.Invoke(song);
			foreach (var item in _elements)
				item.IsSelected = item.Song == song;
		}

		private void DeSelectAllEvements()
		{
			foreach (var item in _elements)
				item.IsSelected = false;
		}

		private void ConfirmPropertyElements()
		{
			foreach (var item in _elements)
			{
				var currentPosition = _content.anchoredPosition.x + item.RectTransform.anchoredPosition.x;
				float distance = Mathf.Clamp(Mathf.Abs(currentPosition / _propertyOffset), 0, 1);

				item.RectTransform.localScale = Vector3.one * (1 - distance);
				item.CanvasGroup.alpha = (1 - distance * 2f);
			}
			RePositions();
			OnDragDeltaEvent?.Invoke();
		}

		private void RePositions()
		{
			var minPositionElement = _elements[0];
			var maxPositionElement = _elements[0];

			foreach (var item in _elements)
			{
				if (item.RectTransform.anchoredPosition.x < minPositionElement.RectTransform.anchoredPosition.x)
				{
					minPositionElement = item;
				}

				if (item.RectTransform.anchoredPosition.x > maxPositionElement.RectTransform.anchoredPosition.x)
				{
					maxPositionElement = item;
				}
			}

			if (minPositionElement.RectTransform.anchoredPosition.x + _content.anchoredPosition.x > -_itemDistance * 0.7)
			{
				maxPositionElement.RectTransform.anchoredPosition = new Vector2(
				minPositionElement.RectTransform.anchoredPosition.x - _itemDistance,
				maxPositionElement.RectTransform.anchoredPosition.y
				);
			}

			if (_content.anchoredPosition.x + maxPositionElement.RectTransform.anchoredPosition.x < _itemDistance * 0.7)
			{
				minPositionElement.RectTransform.anchoredPosition = new Vector2(
				maxPositionElement.RectTransform.anchoredPosition.x + _itemDistance,
				minPositionElement.RectTransform.anchoredPosition.y
				);
			}
		}
	}
}
