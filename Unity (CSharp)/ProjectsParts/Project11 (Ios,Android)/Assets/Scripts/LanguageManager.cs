using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class LanguageManager: Singleton<LanguageManager> {

  public List<LanuageLibrary> lanuageLibrary;

  private LanuageTypesParam _activeLanuage;

  public LanuageTypesParam activeLanuage {
    get { return _activeLanuage; }
    set {

      if (_activeLanuage != null && _activeLanuage.code == value.code) return;

      _activeLanuage = value;
      ParceFile(_activeLanuage.fileName);
      ExEvent.LanuageEvents.OnChangeLanguage.Call(_activeLanuage);
    }
  }

  public List<LanuageTypesParam> lanuageTypeParam;                                    // Ссылки на файлы локализации под настройки
  protected Dictionary<string, string> library = new Dictionary<string, string>();    // Библиотека, содержит переводы по текущей настройки локализации

  public bool printParsing;

  protected override void Awake() {
    base.Awake();
    Load();
  }

  private void Start() { }

  private void Save() {
    PlayerPrefs.SetString("localization", activeLanuage.code);

  }

  private void Load() {

    if (!PlayerPrefs.HasKey("localization")) {
      InitSelectLanuage(null);
      Save();
    } else {
      InitSelectLanuage(PlayerPrefs.GetString("localization"));
    }

  }

  public string LoadDefaultParamsTranslateWords() {
    
    if (Application.systemLanguage == SystemLanguage.Russian) {
      return "ru";
    } else if (Application.systemLanguage == SystemLanguage.English) {
      return "en";
    }

    LanuageLibrary lib = lanuageLibrary.Find(x => x.type == Application.systemLanguage && x.isWordTranslate);
    if (lib == null) {
      return "en";
    }

    return lib.code;

  }

  void InitSelectLanuage(string code) {

    LanuageTypesParam alc = lanuageTypeParam.Find(x => (String.IsNullOrEmpty(code) && x.type == Application.systemLanguage) || (!String.IsNullOrEmpty(code) && x.code == code) && x.isInterface);

//#if UNITY_EDITOR
//    alc = lanuageTypeParam.Find(x => x.type == SystemLanguage.English && x.isInterface);
//#endif

    if (alc == null) {
      alc = lanuageTypeParam.Find(x => x.type == SystemLanguage.English);
    }
    activeLanuage = alc;

  }

  /// <summary>
  /// Получить текущие данные по переводу
  /// </summary>
  /// <param name="key">Ключ</param>
  /// <returns>Получить ключ</returns>
  public static string GetTranslate(string key) {

    //return key;

    return (Instance && Instance.library != null && Instance.library.ContainsKey(key.ToLower())
              ? Instance.library[key.ToLower()]
              : "");
  }

  /// <summary>
  /// Текущий язык
  /// </summary>
  /// <returns>SystemLanguage тип языка</returns>
  public static LanuageTypesParam GetLanuage() {
    return Instance.activeLanuage;
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

    for (int i = 0; i < fileLanuageStr.Length; i++) {

      if (toEndLine && fileLanuageStr[i] != '\n') continue;
      if (toEndLine && fileLanuageStr[i] == '\n') toEndLine = false;
      if (i == fileLanuageStr.Length - 1) str += fileLanuageStr[i];

      if (System.Array.IndexOf(new char[] { '\t', '#', '=', '\n' }, fileLanuageStr[i]) >= 0 || i == fileLanuageStr.Length - 1) {

        if (fileLanuageStr[i] == '=') {
          if (stringBlock == 0) {
            str = str.Trim();

            if (str != "") {
              keyLib = str;
              stringBlock++;
            }
          }
        }

        if (stringBlock > 0 && (fileLanuageStr[i] == '\n' || i == fileLanuageStr.Length - 1)) {
          if (stringBlock == 1) {
            str = str.Replace('\"', ' ').Trim();

            if (str != "") {
              valueLib = Regex.Replace(str, @"\\n", "\n");
            }
          }
        }

        if (fileLanuageStr[i] == '\n' || i == fileLanuageStr.Length - 1 || fileLanuageStr[i] == '#') {

          if (fileLanuageStr[i] == '#') toEndLine = true;
          try {
            if (keyLib != "") library.Add(keyLib.ToLower(), valueLib);
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

    if (printParsing) {
      foreach (var l in library) Debug.Log(l.Key + " - " + l.Value);
    }

  }

}

/// <summary>
/// Параметры языка
/// </summary>
[System.Serializable]
public class LanuageTypesParam {
  public string code;
  public SystemLanguage type;
  public TextAsset fileName;
  public Font font;
  public bool isInterface;
}

[System.Serializable]
public class LanuageLibrary {
  public string title;
  public string code;
  public string nativeTitle;
  public SystemLanguage type;
  public Sprite sprite;
  public bool isWordTranslate;
  public bool isGameLanguage;
  public bool selectTranslateList;
}