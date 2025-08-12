using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

	public List<UiPanel> uiList;

	private UiPanel actualPanel;

	public UiPanel GetPanel(UiType getType) {
		return uiList.Find(x => x.type == getType);
	}

	// Изменение панель
	public void SetPanel(UiType getType, Action OnHideCompleted = null, Action OnShowCompleted = null) {

		if (actualPanel == null) {
			Show(getType, OnShowCompleted);
			return;
		}

		Hide(() => {
			if (OnHideCompleted != null) OnHideCompleted();
			Show(getType);
		});
	}

	/// <summary>
	/// Сокрытие панели
	/// </summary>
	/// <param name="Onconplited"></param>
	public void Hide(Action OnСompleted = null) {
		actualPanel.Hide(() => {
			actualPanel.gameObject.SetActive(false);
			actualPanel = null;
			if (OnСompleted != null) OnСompleted();
		});
	}

	/// <summary>
	/// Открытие панели
	/// </summary>
	/// <param name="getType">Какой тип запустить</param>
	public void Show(UiType getType, Action OnShowCompleted = null) {

		actualPanel = uiList.Find(x => x.type == getType);
		actualPanel.gameObject.SetActive(true);
		actualPanel.Show(() => {
			if (OnShowCompleted != null) OnShowCompleted();
		});
	}

	private Stack<UiPanel> _stack = new Stack<UiPanel>();

	public void VisibleDialog(UiPanel panel) {

		_stack.Push(panel);
	}

	public void HiddenDialog(UiPanel panel) {

		_stack.Pop();
	}

	public void Escape() {
		if (_stack.Count <= 0) return;

		_stack.Peek().ManagerClose();
	}

}

public enum UiType {
	none = 0,
	splash = 1,
	loader = 2,
	menu = 3,
	locations = 4,
	levels = 5,
	game = 6,
	dailyBonusConfirm = 7,
	shop = 8,
	conch = 9,
	setting = 10,
	gameResult = 11,
	blackRound = 12,
	bonusLevelStart = 13,
	dailyBonusFailed = 14,
	decor = 15,
	tutorial = 16,
	tutorConfirm = 17,
	settingTutor = 18,
	dailyBonusCancel = 19,
	exitGameQuestion = 20,
	needInternet = 21,
	rewardVideoGet = 22,
	language = 23,
	revardVideoGift = 24
}