using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(LanguageManager))]
public class LanuageEditor: Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if(GUILayout.Button("Change Lanuage")) {
			((LanguageManager)target).CallEvents();
		}
	}
}

#endif


/// <summary>
/// Локализатор
/// </summary>
public class LanguageManager : Singleton<LanguageManager> {

	/// <summary>
	/// Параметры языка
	/// </summary>
	[System.Serializable]
	public struct lanuageTypesParam {
		public SystemLanguage type;
		public TextAsset fileName;
	}

	private SystemLanguage _selectLanuage = SystemLanguage.Unknown;

	public SystemLanguage selectLanuage {
		get { return _selectLanuage; }
		set {
			if (_selectLanuage == value)
				return;
			_selectLanuage = value;

			ParceFile(Instance.lanuageTypeParam.Find(x => x.type == _selectLanuage).fileName);

			if (changeLanuage != null) changeLanuage();
			ExEvent.LanguageEvents.LanuageChange.Call();
		}
	}


	public List<lanuageTypesParam> lanuageTypeParam;                                    // Ссылки на файлы локализации под настройки
	protected Dictionary<string, string> library = new Dictionary<string, string>();    // Библиотека, содержит переводы по текущей настройки локализации
	public static event Action changeLanuage;                                    // Событие изменения языка
	
	public bool printParsing;

	protected override void Awake() {
		base.Awake();
		GetSelectLanuage();
	}
	
	void GetSelectLanuage() {

		selectLanuage = (Application.systemLanguage == SystemLanguage.Russian ? SystemLanguage.Russian : SystemLanguage.English);

#if UNITY_EDITOR
		selectLanuage = (PlayerPrefs.GetString("lanuage") == SystemLanguage.Russian.ToString() ? SystemLanguage.Russian : SystemLanguage.English);
#endif
	}
	
	/// <summary>
	/// Получить текущие данные по переводу
	/// </summary>
	/// <param name="key">Ключ</param>
	/// <returns>Получить ключ</returns>
	public static string GetTranslate(string key) {
		return (Instance && Instance.library != null && Instance.library.ContainsKey(key.ToLower())
							? Instance.library[key.ToLower()]
							: key);
	}
	

	/// <summary>
	/// Парсинг файла
	/// </summary>
	/// <param name="fileLanuage"></param>
	void ParceFile(TextAsset fileLanuage) {

		library = new Dictionary<string, string>();
		string fileLanuageStr = fileLanuage.text;

		string str = "";
		int stringBlock = 0;

		string keyLib = "";
		string valueLib = "";

		bool toEndLine = false;

		for(int i = 0; i < fileLanuageStr.Length; i++) {

			if(toEndLine && fileLanuageStr[i] != '\n') continue;
			if(toEndLine && fileLanuageStr[i] == '\n') toEndLine = false;
			if(i == fileLanuageStr.Length - 1) str += fileLanuageStr[i];

			if(System.Array.IndexOf(new char[] { '\t', '#', '=', '\n' }, fileLanuageStr[i]) >= 0 || i == fileLanuageStr.Length - 1) {

				if(fileLanuageStr[i] == '=') {
					if(stringBlock == 0) {
						str = str.Trim();

						if(str != "") {
							keyLib = str;
							stringBlock++;
						}
					}
				}

				if(stringBlock > 0 && (fileLanuageStr[i] == '\n' || i == fileLanuageStr.Length - 1)) {
					if(stringBlock == 1) {
						str = str.Replace('\"', ' ').Trim();

						if(str != "") {
							valueLib = Regex.Replace(str, @"\\n", "\n");
						}
					}
				}

				if(fileLanuageStr[i] == '\n' || i == fileLanuageStr.Length - 1 || fileLanuageStr[i] == '#') {

					if(fileLanuageStr[i] == '#') toEndLine = true;
					try {
						if(keyLib != "") library.Add(keyLib.ToLower(), valueLib);
					} catch {
						Debug.Log("Ошибка " + keyLib);
					}
					stringBlock = 0;
					keyLib = "";
					valueLib = "";
				}

				str = "";
			} else {
				str += fileLanuageStr[i];
			}
		}

		if(printParsing) {
			foreach(var l in library) Debug.Log(l.Key + " - " + l.Value);
		}

	}

	public void CallEvents() {

		selectLanuage =
			(selectLanuage == SystemLanguage.Russian
				? SystemLanguage.English
				: SystemLanguage.Russian);
		
	}

}
