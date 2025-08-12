using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor: Editor {
  
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    GameManager.isDebug = GUILayout.Toggle(GameManager.isDebug, "Дебаг");
  }

}

#endif

/// <summary>
/// Основной контроллер игрового процесса (singlton)
/// </summary>
public class GameManager : Singleton<GameManager> {

	public static bool isDebug = true;

	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(this);
	}

  private void Start(){}
  
  #region Работа с полноэкранным режимом

	public static event System.Action OnChangeFullScreen;

	/// <summary>
	/// Установка полного экрана
	/// </summary>
	public void ChangeFullScreen() {
		StartCoroutine(ChangeFullScreen(!Screen.fullScreen));
	}

	IEnumerator ChangeFullScreen(bool flag) {
		bool needChange = flag;

		while (needChange != Screen.fullScreen) {
			Screen.fullScreen = needChange;
			yield return new WaitForFixedUpdate();
		}
		Cursor.lockState = CursorLockMode.None;

		if (OnChangeFullScreen != null) OnChangeFullScreen();

	}

	#endregion

}
