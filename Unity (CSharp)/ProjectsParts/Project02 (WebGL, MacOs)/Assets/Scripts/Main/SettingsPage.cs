using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using it.Main.SinglePages;
using System;

namespace it.UI
{
	public class SettingsPage : SinglePage
	{
		[SerializeField] private NavigationButton[] _buttons;
		[SerializeField] private RectTransform _contentParent;

		[System.Serializable]
		public struct NavigationButton
		{
			public string Type;
			public string Title;
			public it.UI.Elements.FilterSwitchButtonUI Button;
			public GameObject Page;
		}

		private void OnDisable()
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				if (_buttons[i].Page != null)
				{
					Destroy(_buttons[i].Page);
				}
			}
		}

		protected override void EnableInit()
		{
			base.EnableInit();

			for (int i = 0; i < _buttons.Length; i++)
			{
				int index = i;
				_buttons[index].Button.OnClick.RemoveAllListeners();
				_buttons[index].Button.OnClick.AddListener(() =>
				{
					SelectPage(index);
				});
			}
			SelectPage(0);
		}

		public void SelectPage(int index)
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].Button.IsSelect = i == index;

				if (i == index)
				{
					if (_buttons[i].Page == null)
					{
						GameObject go = Garilla.ResourceManager.GetResource<GameObject>($"Prefabs/UI/Settings/{_buttons[i].Title}");
						GameObject inst = Instantiate(go, _contentParent);
						RectTransform rtInst = inst.GetComponent<RectTransform>();
						rtInst.anchoredPosition = Vector2.zero;
						rtInst.sizeDelta = Vector2.zero;
						inst.gameObject.SetActive(true);
						_buttons[i].Page = inst;
					}
				}

				if (_buttons[i].Page != null && i != index)
				{
					Destroy(_buttons[i].Page);
				}
			}
		}
		public void SelectPageByType(string type)
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				if (_buttons[i].Type == type)
				{
					SelectPage(i);
					return;
				}
			}
		}

	}
}