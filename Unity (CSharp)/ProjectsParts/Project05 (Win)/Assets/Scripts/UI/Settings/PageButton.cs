using System;

using UnityEngine;

using UnityEngine.Events;

namespace it.UI.Settings
{
  public class PageButton : MonoBehaviourBase
  {
	 public UnityAction<PageButton> OnClickEvent;

	 [SerializeField] private Settings.PageType _page;
	 [SerializeField] private Settings.PageType _focusPage;

	 public Settings.PageType Page { get => _page; set => _page = value; }
	 public Settings.PageType FocusPage { get => _focusPage; set => _focusPage = value; }

	 public void Click()
	 {
		OnClickEvent?.Invoke(this);
	 }

  }
}