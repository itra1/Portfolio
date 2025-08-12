using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Экранная отладка
/// </summary>
public class LogPanel : MonoBehaviour {
  
  [SerializeField]
  Text textLog;

  void Awake() {
    Application.logMessageReceived += LogCallback;
  }

  void OnDestroy() {
    Application.logMessageReceived -= LogCallback;
  }

  void LogCallback(string condition , string stackTrace , LogType type) {
    textLog.text = "***" + '\n' + condition.Substring(0, Mathf.Min(200, condition.Length)) + '\n' + textLog.text;
  }

}
