using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.UI.Elements;
using System.Linq;

namespace it.UI
{
	public class SettingsLeftPanel : MonoBehaviour
	{
		[SerializeField] private List<Pages> _pages;
		[SerializeField] private List<SettingsNavigationButton> _navButtons;

		private RectTransform _rt;

		private void Awake()
		{
			_rt = GetComponent<RectTransform>();
			for (int i = 0; i < _pages.Count; i++)
			{
				_pages[i].Block.GetComponent<VerticalAccordionUI>().OnSizeChange -= UpdateSize;
				_pages[i].Block.GetComponent<VerticalAccordionUI>().OnSizeChange += UpdateSize;
			}
		}

		

		//public void SetPage(SettingsPageType page)
		//{
		//	for (int i = 0; i < _navButtons.Count; i++)
		//	{
		//		_navButtons[i].SetFocus(_navButtons[i].Page == page);
		//	}

		//	for (int i = 0; i < _pages.Count; i++)
		//	{
		//		_pages[i].Block.GetComponent<VerticalAccordionUI>().SetOpen(_pages[i].Page.Contains(page));
		//	}
		//}

		private void UpdateSize()
		{
			float startHeight = 0;
			for (int i = 0; i < _pages.Count; i++)
			{
				_pages[i].Block.anchoredPosition = new Vector2(_pages[i].Block.anchoredPosition.x, -startHeight);
				startHeight += _pages[i].Block.rect.height;
			}
			//_rt.sizeDelta = new Vector2(_rt.sizeDelta.x, startHeight);
		}

		[System.Serializable]
		public struct Pages
		{
			public RectTransform Block;
			//public List<SettingsPageType> Page;
		}

		public enum AccordiontPageType
		{
			MyPage,
			Carrier,
			Settings
		}

	}
}