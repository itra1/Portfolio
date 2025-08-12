using it.UI;
using it.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Main
{
	public class HomePage : MainContentPage
	{
	[SerializeField] private RectTransform _rectParent;
		[SerializeField] protected RectTransform _itemPrefab;
		[SerializeField] private List<RectTransform> _banners;

		private string _currentLoc;
		public void ClickTouch()
		{

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
				rawI.texture = textures[i];
				_banners.Add(iRect);
				

			}

		}

	}
}