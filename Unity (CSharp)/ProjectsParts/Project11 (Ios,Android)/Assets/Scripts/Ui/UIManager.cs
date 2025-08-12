using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

  public UiLibrary library;

  private List<UiPanel> instanceList = new List<UiPanel>();

	private UiPanel actualPanel;

	public T GetPanel<T>() where T : UiPanel {

    UiPanel panel = instanceList.Find(x => x.GetType() == typeof(T));

    if(panel == null) {

      UiPanel pref = (T)library.uiList.Find(x => x.GetType() == typeof(T));

      GameObject obj = Instantiate(pref.gameObject);
      obj.transform.SetParent(transform);
      obj.transform.localScale = Vector3.one;
      obj.GetComponent<RectTransform>().sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;
      RectTransform rect = obj.GetComponent<RectTransform>();
      rect.localScale = Vector2.one;
      rect.localPosition = Vector2.zero;
      rect.anchoredPosition = Vector2.zero;
      rect.anchorMin = pref.GetComponent<RectTransform>().anchorMin;
      rect.anchorMax = pref.GetComponent<RectTransform>().anchorMax;
      rect.sizeDelta = pref.GetComponent<RectTransform>().sizeDelta;
      panel = obj.GetComponent<UiPanel>();
      instanceList.Add(panel);
    }
    
    return (T)panel;
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
	public void Show<T>(Action OnShowCompleted = null) where T: UiPanel {

		actualPanel = library.uiList.Find(x => x.GetType() == typeof(T));
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