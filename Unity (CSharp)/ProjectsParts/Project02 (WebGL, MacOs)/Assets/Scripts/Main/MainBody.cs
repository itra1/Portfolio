using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace it.Main
{
	public class MainBody : MonoBehaviour
	{
		[SerializeField] private List<MainContentPage> _pages;

		public void SetPage(MainPagesType game)
		{
			for (int i = 0; i < _pages.Count; i++)
			{
				_pages[i].gameObject.SetActive(game == _pages[i].Page);
			}
		}

	}
}