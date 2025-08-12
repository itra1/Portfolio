using System;
using UnityEngine;
using System.Collections.Generic;
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
	
	private List<PanelUiBase> _guiList = new List<PanelUiBase>();

	public UiElementsLibrary library;

	public static T ShowUi<T>() where T : PanelUiBase {
		return Instance.ShowUI<T>();
	}
	private T ShowUI<T>() where T : PanelUiBase {

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

	public AudioClip clickClip;                                         // Звук клика

	public AudioClip[] dialogClip;
	
	public static void ClickButtonAudio() {
		AudioManager.PlayEffect(Instance.clickClip, AudioMixerTypes.runnerUi);
	}

	#region Диалоги

	public static void ShowPushDialog(Action OnClose) {
		if (UserManager.pushRegister) {
			if (OnClose != null) OnClose();
			return;
		}
		AudioManager.PlayEffect(Instance.dialogClip[Random.Range(0, Instance.dialogClip.Length)], AudioMixerTypes.runnerUi);
		PushDialog instPush = ShowUi<PushDialog>();
		instPush.gameObject.SetActive(true);
		instPush.OnClose = OnClose;
	}

	public static void ShareDialogShow(Action OnClose) {
		AudioManager.PlayEffect(Instance.dialogClip[Random.Range(0, Instance.dialogClip.Length)], AudioMixerTypes.runnerUi);
		ShareDialog instPush = ShowUi<ShareDialog>();
		instPush.gameObject.SetActive(true);
		instPush.OnClose = OnClose;
	}

	public static void RateDialogShow(Action OnClose) {
		AudioManager.PlayEffect(Instance.dialogClip[Random.Range(0, Instance.dialogClip.Length)], AudioMixerTypes.runnerUi);
		RateDialog instPush = ShowUi<RateDialog>();
		instPush.gameObject.SetActive(true);
		instPush.OnClose = OnClose;
	}

	#endregion

}
