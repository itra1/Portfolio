using it.Popups;
using UnityEngine;
using it.UI.Elements;
using Garilla;
using UnityEngine.Events;
using System;

public class TopMobilePanel : MonoBehaviour, ISwipe
{
	public UnityEngine.Events.UnityEvent OnBackAction;
	public UnityEngine.Events.UnityEvent OnSorterAction;

	[SerializeField] private BalanceWidget _balanceWidget;
	[SerializeField] private RectTransform _balancePanel;
	[SerializeField] private RectTransform _backPanel;
	[SerializeField] private RectTransform _sorterPanel;
	[SerializeField] private RectTransform _singleTitlePanel;
	[SerializeField] private RectTransform _appTitlePanel;
	[SerializeField] private bool _swipeListenerEnable = true;
	[SerializeField] private Garilla.SwipeToClose _swipeToClose = Garilla.SwipeToClose.None;

	private VisibleElements _startPanelVisible;
	private UnityEngine.Events.UnityEvent OnBackActionStart;

	[Flags]
	public enum VisibleElements
	{
		None = 0,
		Balance = 1,
		BackButton = 2,
		SorterButton = 4,
		SingleTitle = 8,
		AppTitle = 16
	}

	public SwipeToClose SwipeType => _swipeToClose;

	public UnityAction<SwipeToClose> OnSwipeEvent => (swipe) =>
	{
		if ((_swipeToClose & swipe) != 0)
		{
			BackButtonTouch();
		}

	};

	private void OnEnable()
	{
		RectTransform bRt = _balancePanel;
		RectTransform sRt = _backPanel;
		if (_sorterPanel.gameObject.activeSelf)
			bRt.anchoredPosition = new Vector2(-30 - _backPanel.rect.width, bRt.anchoredPosition.y);
		else
			bRt.anchoredPosition = new Vector2(-15, bRt.anchoredPosition.y);
		if (_swipeListenerEnable && _swipeToClose != SwipeToClose.None)
			SwipeListenerAdd();
		StartVisiblePanels();
	}

	private void StartVisiblePanels(){
		if (_startPanelVisible == VisibleElements.None) return;
		VisiblePanels(_startPanelVisible);
	}
	public void SetVisiblePanels(VisibleElements elements)
	{
		if (_startPanelVisible == VisibleElements.None){
			if (_balancePanel.gameObject.activeSelf) _startPanelVisible |= VisibleElements.Balance;
			if (_backPanel.gameObject.activeSelf) _startPanelVisible |= VisibleElements.BackButton;
			if (_sorterPanel.gameObject.activeSelf) _startPanelVisible |= VisibleElements.SorterButton;
			if (_singleTitlePanel.gameObject.activeSelf) _startPanelVisible |= VisibleElements.SingleTitle;
			if (_appTitlePanel.gameObject.activeSelf) _startPanelVisible |= VisibleElements.AppTitle;
		}
		VisiblePanels(elements);
	}
	private void VisiblePanels(VisibleElements panels){
		_balancePanel.gameObject.SetActive((panels & VisibleElements.Balance) != 0);
		_backPanel.gameObject.SetActive((panels & VisibleElements.BackButton) != 0);
		_sorterPanel.gameObject.SetActive((panels & VisibleElements.SorterButton) != 0);
		_singleTitlePanel.gameObject.SetActive((panels & VisibleElements.SingleTitle) != 0);
		_appTitlePanel.gameObject.SetActive((panels & VisibleElements.AppTitle) != 0);
	}

	public void SwipeListenerAdd()
	{
		SwipeManager.AddListener(this);
	}
	public void SwipeListenerRemove()
	{
		SwipeManager.RemoveListener(this);
	}

	private void OnDisable()
	{
		if (_swipeListenerEnable && _swipeToClose != SwipeToClose.None)
			SwipeListenerRemove();
	}

	public void SorterButtonTouch()
	{
		OnSorterAction?.Invoke();
	}

	public void BackButtonTouch()
	{
		OnBackAction?.Invoke();
	}

}
