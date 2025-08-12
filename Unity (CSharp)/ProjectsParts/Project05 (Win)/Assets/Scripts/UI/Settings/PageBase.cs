using System;
using System.Collections.Generic;

using UnityEngine;

using static it.UI.Settings.Settings;

namespace it.UI.Settings
{
  public class PageBase : MonoBehaviourBase
  {
	 private List<Item> _itemList = new List<Item>();
	 [SerializeField]
	 private PageType _page;
	 public PageType Page { get => _page; set => _page = value; }

	 protected virtual void OnEnable()
	 {
		_itemList = GetChildrens<Item>();

		foreach (var elem in _itemList)
		  elem._onFocus = SetFocus;
	 }

	 protected virtual void OnDisable() { }

	 private void SetFocus(Item focus)
	 {

	 }

  }
}