using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Log : MonoBehaviour {

  public Text logText;

  void Awake() {
    //Application.logMessageReceived += LogCallback;
  }

  private void Start() {
    Application.logMessageReceived += LogCallback;
  }

  private void OnDisable() {
    Application.logMessageReceived -= LogCallback;
  }

  void LogCallback(string condition, string stackTrace, LogType type) {
    logText.text = condition + "\n" + stackTrace + "\n\n" + logText.text;
  }

}
