using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.UI;
using UnityEngine.UI;
using DG.Tweening;

namespace it.Game.Managers
{
  /// <summary>
  /// Менеджер UI
  /// </summary>
  public class UiManager : Singleton<UiManager>
  {

	 private List<UIDialog> _guiList = new List<UIDialog>();
	 private UILibrary _library;

	 [SerializeField]
	 private Image _fullcolor = null;

	 [SerializeField]
	 private RectTransform _parentPanel;

	 private void OnEnable()
	 {
		_fullcolor.gameObject.SetActive(false);
	 }

	 public UILibrary Library
	 {
		get
		{
		  if (_library == null)
			 _library = Resources.Load<UILibrary>(ProjectSettings.GuiLibrary);
		  return _library;
		}
	 }

	 public void RemoveInstance(UIDialog panel)
	 {
		_guiList.Remove(panel);
		Destroy(panel.gameObject);
	 }

	 public static T GetPanel<T>() where T : UIDialog
	 {
		return Instance.GetPanel_<T>();
	 }

	 public static void CloseAllPanels()
	 {

		for (int i = 0; i < Instance._guiList.Count; i++)
		{
		  Instance._guiList[i].gameObject.SetActive(false);
		}

	 }

	 private T GetPanel_<T>() where T : UIDialog
	 {

		T inst = (T)_guiList.Find(x => x.GetType() == typeof(T));

		if (inst == null)
		{

		  T lib = (T)Library.DialogLibrary.Find(x => x.GetType() == typeof(T));

		  if (lib == null)
		  {
			 Debug.LogError("Не найдена панель типа " + typeof(T));
			 return null;
		  }
		  lib.gameObject.SetActive(false);
		  GameObject panel = InstantiatePrefab(lib.gameObject);

		  if (panel == null)
		  {
			 Debug.LogError("Не удалось сгенерировать панель с типом " + typeof(T));
			 return null;
		  }

		  inst = panel.GetComponent<T>();

		  inst.transform.hierarchyCapacity = inst.LastHierarchy ? 0 : 1;

		  _guiList.Add(inst);

		}

		inst.transform.SetAsLastSibling();
		return inst;
	 }

	 GameObject InstantiatePrefab(GameObject prefabUi)
	 {
		prefabUi.gameObject.SetActive(false);
		GameObject inst = Instantiate(prefabUi, _parentPanel);
		return inst;
	 }
	 public void FullColor(Color32 targetColor, float duration = 0.5f,
	 bool disableAfter = false,
		UnityEngine.Events.UnityAction onStart = null,
		UnityEngine.Events.UnityAction onComplete = null)
	 {
		FullColor(targetColor, new Color32(0, 0, 0, 0), duration, disableAfter, onStart, onComplete);
	 }
	 public void FullColor(float duration = 0.5f,
	 bool disableAfter = false,
		UnityEngine.Events.UnityAction onStart = null,
		UnityEngine.Events.UnityAction onComplete = null)
	 {
		FullColor(Color.black, new Color32(0, 0, 0, 0), duration, disableAfter, onStart, onComplete);
	 }
	 public void FillAndRepeatColor(Color32 targetColor, float duration,
	 UnityEngine.Events.UnityAction onStart = null,
	 UnityEngine.Events.UnityAction onFullFill = null,
	 UnityEngine.Events.UnityAction onComplete = null,
	 float waitMiddle = 1)
	 {
		FillAndRepeatColor(new Color32(0, 0, 0, 0), targetColor, duration, onStart, onFullFill, onComplete, waitMiddle);
	 }
	 /// <summary>
	 /// Заполнение цветом
	 /// </summary>
	 /// <param name="color"></param>
	 /// <param name="speed"></param>
	 /// <param name="onStart"></param>
	 /// <param name="onFullBlack"></param>
	 /// <param name="onComplete"></param>
	 public void FullColor(Color32 targetColor, Color32 startColor,
	 float duration = 0.5f,
	 bool disableAfter = false,
	 UnityEngine.Events.UnityAction onStart = null,
	 UnityEngine.Events.UnityAction onComplete = null)
	 {
		_fullcolor.color = startColor;
		_fullcolor.gameObject.SetActive(true);

		_fullcolor.DOColor(targetColor, duration)
		  .OnStart(() =>
		  {
			 onStart?.Invoke();
		  })
		  .OnComplete(() =>
		  {
			 onComplete?.Invoke();

			 if (disableAfter)
				_fullcolor.gameObject.SetActive(false);
		  });
	 }
	 public void FillAndRepeatColor(float duration,
	 UnityEngine.Events.UnityAction onStart = null,
	 UnityEngine.Events.UnityAction onFullFill = null,
	 UnityEngine.Events.UnityAction onComplete = null,
	 float waitMiddle = 1)
	 {
		FillAndRepeatColor(new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 1), duration, onStart, onFullFill, onComplete, waitMiddle);
	 }
	 public void FillAndRepeatColor(Color32 sourceColor,
		Color32 targetColor,
		float duration,
	 UnityEngine.Events.UnityAction onStart = null,
	 UnityEngine.Events.UnityAction onFullFill = null,
	 UnityEngine.Events.UnityAction onComplete = null,
	 float waitMiddle = 1)
	 {
		Debug.Log("FillAndRepeatColor");
		_fullcolor.gameObject.SetActive(true);
		_fullcolor.color = sourceColor;

		_fullcolor.DOColor(targetColor, duration)
		  .OnStart(() =>
		  {
			 onStart?.Invoke();
		  })
		  .OnComplete(() =>
		  {
			 onFullFill?.Invoke();

			 _fullcolor.DOColor(sourceColor, duration)
		  .OnComplete(() =>
		  {
			 onComplete?.Invoke();
			 _fullcolor.gameObject.SetActive(false);
		  }).SetDelay(waitMiddle);

		  });
	 }
  }
}