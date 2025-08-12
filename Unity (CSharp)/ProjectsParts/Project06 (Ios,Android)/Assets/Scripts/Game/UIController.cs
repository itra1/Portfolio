using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum UITypes {
	timer = 0,
	battleResult = 1,
	battleGamePlay = 2,
	menuGamePlay = 3,
	battlePause = 4,
	battleWeaponBye = 5,
	userProfile = 6,
	option = 7,
	errorDialog = 8,
	briefing = 9,
	shop = 10,
	messageDialog = 11,
	mapGamePlay = 12,
	fillBlack = 13,
	catScene = 14
}

/// <summary>
/// Общий контроллер интерфейсов
/// </summary>
public class UIController : Singleton<UIController> {

  private List<UiDialog> _guiList = new List<UiDialog>();

  public UiLibrary library;

	public static T ShowUi<T>() where T : UiDialog {
		return Instance.ShowUI<T>();
	}
  private T ShowUI<T>() where T : UiDialog {

    T inst = (T)_guiList.Find(x => x.GetType() == typeof(T));

    if (inst == null) {

      T lib = (T)library.DialogLibrary.Find(x => x.GetType() == typeof(T));

      if (lib == null) {
        Debug.LogError("Не найдена панель типа " + typeof(T));
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
    GameObject ins = Instantiate(prefabUi,transform);
    //ins.transform.SetParent(transform);
    //RectTransform rect = ins.GetComponent<RectTransform>();
    //rect.localScale = Vector2.one;
    //rect.localPosition = Vector2.zero;
    //rect.anchoredPosition = Vector2.zero;
    //rect.anchorMin = prefabUi.GetComponent<RectTransform>().anchorMin;
    //rect.anchorMax = prefabUi.GetComponent<RectTransform>().anchorMax;
    //rect.sizeDelta = prefabUi.GetComponent<RectTransform>().sizeDelta;
    return ins;
  }
  	
	public AudioBlock clickAudio;
	public AudioBlock rejectAudio;
	public AudioBlock parametrUpAudio;

	public static void ClickPlay() {
		Instance.clickAudio.PlayRandom(Instance);
		//AudioManager.PlayEffects(instance.clickAudio, AudioMixerTypes.effectPlay);
	}

	public static void RejectPlay() {
		Instance.rejectAudio.PlayRandom(Instance);
		//AudioManager.PlayEffects(instance.rejectAudio[Random.Range(0, instance.rejectAudio.Count)], AudioMixerTypes.effectPlay);
	}

	public static void ParametrUpPlay() {
		Instance.parametrUpAudio.PlayRandom(Instance);
		//AudioManager.PlayEffects(instance.parametrUpAudio[Random.Range(0, instance.parametrUpAudio.Count)], AudioMixerTypes.effectPlay);
	}


	public ErrorDialog ErrorDialog(string testMessage) {
    ErrorDialog gui = UIController.ShowUi<ErrorDialog>();
		gui.gameObject.SetActive(true);
		gui.errorText.text = testMessage;
		return gui;
	}

	public MessageDialog MessageDialog(string testMessage) {
    MessageDialog gui = UIController.ShowUi<MessageDialog>();
		gui.gameObject.SetActive(true);
		gui.messageText.text = testMessage;
		return gui;
	}
}