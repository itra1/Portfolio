using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UiController))]
public class UiControllerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
	}
}

#endif

/// <summary>
/// Общий контроллер интерфейса
/// </summary>
public class UiController : Singleton<UiController> {

	private List<UiPanel> _guiList = new List<UiPanel>();

	public UiElementsLibrary library;

	public static T GetUi<T>() where T : UiPanel {
		return Instance.GetUI<T>();
	}
	private T GetUI<T>() where T : UiPanel {

		T inst = (T)_guiList.Find(x => x.GetType() == typeof(T));
		
		if (inst == null) {

			T lib = (T)library.guiList.Find(x => x.GetType() == typeof(T));

			if (lib == null) {
				Debug.LogError("Не найдена панель с типом " + typeof(T));
				return null;
			}
			lib.gameObject.SetActive(false);
			GameObject panel = InstanceUi(lib.gameObject);

			if (panel == null) {
				Debug.LogError("Не удалось сгенерировать панель с типом " + typeof(T));
				return null;
			}

			inst = panel.GetComponent<T>();

			_guiList.Add(inst);

		}

		inst.transform.SetAsLastSibling();
		return inst;
	}
	GameObject InstanceUi(GameObject prefabUi) {
		prefabUi.gameObject.SetActive(false);
		GameObject ins = Instantiate(prefabUi);
		ins.transform.SetParent(transform);
		RectTransform rect = ins.GetComponent<RectTransform>();
		rect.localScale = Vector2.one;
		rect.localPosition = Vector2.zero;
		rect.anchoredPosition = Vector2.zero;
		rect.anchorMin = prefabUi.GetComponent<RectTransform>().anchorMin;
		rect.anchorMax = prefabUi.GetComponent<RectTransform>().anchorMax;
		rect.sizeDelta = prefabUi.GetComponent<RectTransform>().sizeDelta;
		return ins;
	}

  public void ClickSound() {
    DarkTonic.MasterAudio.MasterAudio.PlaySound("Click");
  }

}
