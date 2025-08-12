using DG.Tweening;

using it.Settings;
using it.UI.Elements;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Garilla.Main
{
	public interface TargetCaruselNavigationMenu
	{
		public void SelectFromCaruselMenu(string type);
	}

	public class NavigationsCarusel : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IScrollHandler
	{

		[SerializeField] private GameObject TargetAction;
		[SerializeField] private RectTransform _contentRect;
		//[SerializeField] private RectTransform[] _positions;
		[SerializeField] private GameObject _prefab;
		[SerializeField] private bool _eventOnEnable;
		[SerializeField] private NabigationsUse NavigationFor;
		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI _indexLabel;

		private bool _isPointer;
		private float _elementDistance;
		private RectTransform _rt;
		private TargetCaruselNavigationMenu _target;
		private List<ButtonStuct> _buttins = new List<ButtonStuct>();
		private int _currentIndex = 0;
		private int _count;

		private struct ButtonStuct
		{
			public string Type;
			public RectTransform RT;
			public string Title;
		}
		private void Awake()
		{
			Spawn();
			_target = TargetAction.GetComponent<TargetCaruselNavigationMenu>();
			_elementDistance = GetComponent<RectTransform>().rect.width / 3 * 1.15f;
			_rt = GetComponent<RectTransform>();
			PositionsElements();
			UpdateStruct();
			_buttins[0].RT.GetComponent<GraphicButtonUI>().Click();
		}

		private void OnEnable()
		{
			if (_eventOnEnable)
				_buttins[_currentIndex].RT.GetComponent<GraphicButtonUI>().Click(); 
		}

		private void Spawn()
		{
			_buttins.Clear();
			_prefab.gameObject.SetActive(false);
			for (int i = 0; i < GameSettings.Games.Count; i++)
			{
				if ((GameSettings.Games[i].NavigationFor & NavigationFor) == 0) continue;
				var index = i;
				var go = Instantiate(_prefab, _prefab.transform.parent);
				go.GetComponent<Image>().sprite = GameSettings.Games[i].RectIcone;
				go.gameObject.SetActive(true);
				_buttins.Add(new ButtonStuct()
				{
					RT = go.GetComponent<RectTransform>()
					,
					Type = GameSettings.Games[index].SlugRequest
				,
					Title = GameSettings.Games[index].Name
				});
				int bIndex = _buttins.Count - 1;
				GraphicButtonUI btn = go.GetComponent<GraphicButtonUI>();
				btn.OnClick.RemoveAllListeners();
				btn.OnClick.AddListener(() =>
				{
					_target.SelectFromCaruselMenu(GameSettings.Games[index].SlugRequest);
					MoveTargetIndex(bIndex);
				});
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_isPointer = true;
		}

		private void PositionsElements()
		{

			for (int i = 0; i < _buttins.Count; i++)
			{
				_buttins[i].RT.anchoredPosition = new Vector2(i * _elementDistance, _buttins[i].RT.anchoredPosition.y);
			}
		}

		private void MoveTargetIndex(int target)
		{
			int targetIndexAll = (int)Mathf.Round(_contentRect.anchoredPosition.x / _elementDistance);
			int targetIndex = targetIndexAll % _buttins.Count;
			if (targetIndex < 0)
				targetIndex = _buttins.Count + targetIndex;

			_contentRect.DOAnchorPos(new Vector2(-_buttins[target].RT.anchoredPosition.x, _contentRect.anchoredPosition.y), 0.2f).OnUpdate(() =>
			{
				UpdateStruct();
			}).OnComplete(() =>
			{
				UpdateStruct();
				_currentIndex = target;
				//_taget.SelectFromCaruselMenu(_buttins[targetIndex].Type);
			});
			if (_titleLabel != null)
				_titleLabel.text = _buttins[target].Title;
			if (_indexLabel != null)
				_indexLabel.text = $"{target+1}/{_buttins.Count}";
		}

		public void OnDrag(PointerEventData eventData)
		{
			_contentRect.anchoredPosition += new Vector2(eventData.delta.x, 0);
			UpdateStruct();
		}


		private void UpdateStruct()
		{
			RePositions();
			ResizeElements();
		}

		private void ResizeElements()
		{
			int targetIndexAll = (int)Mathf.Round(_contentRect.anchoredPosition.x / _elementDistance);
			float xCenter = -_contentRect.anchoredPosition.x;

			for (int i = 0; i < _buttins.Count; i++)
			{
				var tElem = _buttins[i];
				float distance = xCenter > tElem.RT.anchoredPosition.x
				? xCenter - tElem.RT.anchoredPosition.x
				: tElem.RT.anchoredPosition.x - xCenter;
				tElem.RT.localScale = (((1 - (Mathf.Abs(distance) / (_elementDistance * 2))) * 0.5f) + 0.5f) * Vector2.one;
			}

		}

		public void SelectElement(string type)
		{

			for (int i = 0; i < _buttins.Count; i++)
			{
				if (_buttins[i].Type == type)
				{
					//_taget.SelectFromCaruselMenu(_buttins[i].Type);
					MoveTargetIndex(i);
				}
			}

		}

		private void RePositions()
		{

			int targetIndexAll = (int)Mathf.Round(-_contentRect.anchoredPosition.x / _elementDistance);
			int targetIndex = targetIndexAll % _buttins.Count;
			if (targetIndex < 0)
				targetIndex = _buttins.Count + targetIndex;
			float xCenter = -_contentRect.anchoredPosition.x;
			float allLenght = (_buttins.Count - 1) * _elementDistance;

			float tXPosition = _buttins[targetIndex].RT.anchoredPosition.x;

			for (int i = 1; i <= _buttins.Count / 2; i++)
			{
				int sIndex = targetIndex + i;
				if (sIndex >= _buttins.Count)
					sIndex = (targetIndex + i) - _buttins.Count;

				if (_buttins.Count > sIndex)
					_buttins[sIndex].RT.anchoredPosition = _buttins[targetIndex].RT.anchoredPosition + Vector2.right * i * _elementDistance;

				sIndex = targetIndex - i;
				if (sIndex < 0)
					sIndex = _buttins.Count + sIndex;

				if (_buttins.Count > sIndex)
					_buttins[sIndex].RT.anchoredPosition = _buttins[targetIndex].RT.anchoredPosition + Vector2.right * -i * _elementDistance;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_isPointer = false;
			int targetIndexAll = (int)Mathf.Round(-_contentRect.anchoredPosition.x / _elementDistance);
			int targetIndex = targetIndexAll % _buttins.Count;
			if (targetIndex < 0)
				targetIndex = _buttins.Count + targetIndex;
			//MoveTargetIndex(targetIndex);
			_buttins[targetIndex].RT.GetComponent<GraphicButtonUI>().Click();
		}

		public void OnScroll(PointerEventData eventData)
		{
			_contentRect.anchoredPosition += new Vector2(MathF.Sign(eventData.scrollDelta.y) * _elementDistance, 0);
			OnEndDrag(null);
		}
	}
}