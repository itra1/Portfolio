using it.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace it.Mobile.Main
{
	public class NewsPage : MonoBehaviour, MobilePageBase
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] protected RectTransform _itemPrefab;
		[SerializeField] private List<GraphicButtonUI> _banners;
		private string _currentLoc;

		private void Awake()
		{
			for (int i = 0; i < _banners.Count; i++)
			{
				int index = i;
				_banners[index].OnClick.RemoveAllListeners();
				_banners[index].OnClick.AddListener(() =>
				{

					RectTransform rt = _banners[index].GetComponent<RectTransform>();

					float upDelta = _scrollRect.content.anchoredPosition.y + rt.anchoredPosition.y;
					float downDelta = _scrollRect.content.anchoredPosition.y + _scrollRect.viewport.rect.height + rt.anchoredPosition.y - rt.rect.height;
					if (upDelta > 0)
					{
						_scrollRect.content.DOAnchorPos(new Vector2(_scrollRect.content.anchoredPosition.x, _scrollRect.content.anchoredPosition.y - upDelta), 0.2f);
					}
					if (downDelta < 0)
					{
						_scrollRect.content.DOAnchorPos(new Vector2(_scrollRect.content.anchoredPosition.x, _scrollRect.content.anchoredPosition.y- downDelta), 0.2f);
					}


				});
			}
		}

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.LocalizationChange, LocalizeChangeHadler);
			LoadBannersToCurrentLoc();
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizeChangeHadler);
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, LocalizeChangeHadler);
		}

		protected virtual void LocalizeChangeHadler(com.ootii.Messages.IMessage handle)
		{
			OnLocalizeChange();
		}

		protected virtual void OnLocalizeChange()
		{
			LoadBannersToCurrentLoc();
		}

		private void LoadBannersToCurrentLoc()
		{
			if (I2.Loc.LocalizationManager.CurrentLanguageCode == _currentLoc) return;

			_currentLoc = I2.Loc.LocalizationManager.CurrentLanguageCode;

			_banners.ForEach(x => Destroy(x.gameObject));
			_banners.Clear();

			Texture2D[] textures = Garilla.ResourceManager.GetResourceAll<Texture2D>($"Textures/Banners/News/{_currentLoc}");

			for (int i = 0; i < textures.Length; i++)
			{

				GameObject inst = Instantiate(_itemPrefab.gameObject, _itemPrefab.parent);
				inst.gameObject.SetActive(true);
				RectTransform iRect = inst.GetComponent<RectTransform>();
				RawImage rawI = inst.GetComponent<RawImage>();
				GraphicButtonUI bI = inst.GetComponentInChildren<GraphicButtonUI>();
				rawI.texture = textures[i];
				_banners.Add(bI);
				bI.OnClick.RemoveAllListeners();
				bI.OnClick.AddListener(() =>
				{

					RectTransform rt = iRect.GetComponent<RectTransform>();

					float upDelta = _scrollRect.content.anchoredPosition.y + rt.anchoredPosition.y;
					float downDelta = _scrollRect.content.anchoredPosition.y + _scrollRect.viewport.rect.height + rt.anchoredPosition.y - rt.rect.height;
					if (upDelta > 0)
					{
						_scrollRect.content.DOAnchorPos(new Vector2(_scrollRect.content.anchoredPosition.x, _scrollRect.content.anchoredPosition.y - upDelta), 0.2f);
					}
					if (downDelta < 0)
					{
						_scrollRect.content.DOAnchorPos(new Vector2(_scrollRect.content.anchoredPosition.x, _scrollRect.content.anchoredPosition.y - downDelta), 0.2f);
					}


				});

			}

		}

	}
}