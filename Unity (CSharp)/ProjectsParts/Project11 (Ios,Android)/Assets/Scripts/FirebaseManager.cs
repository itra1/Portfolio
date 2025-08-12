using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager> {

  private void Start() { }

  public void LogEvent(string code) {
		StartCoroutine(LogAsync(code));
	}

  public void LogEvent(string code,string paramName, string paramValue) {
    StartCoroutine(LogAsync(code,paramName, paramValue));
  }

  public void LogEvent(string code, string paramName, double paramValue) {
    StartCoroutine(LogAsync(code, paramName, paramValue));
  }

  public void LogEvent(string code, string paramName, int paramValue) {
    StartCoroutine(LogAsync(code, paramName, paramValue));
  }

  public void LogEvent(string code, string paramName, long paramValue) {
    StartCoroutine(LogAsync(code, paramName, paramValue));
  }

  IEnumerator LogAsync(string code) {
		yield return new WaitForEndOfFrame();
		Firebase.Analytics.FirebaseAnalytics.LogEvent(code);
	}

  IEnumerator LogAsync(string code, string paramName, string paramValue) {
    yield return new WaitForEndOfFrame();
    Firebase.Analytics.FirebaseAnalytics.LogEvent(code, paramName, paramValue);
  }

  IEnumerator LogAsync(string code, string paramName, double paramValue) {
    yield return new WaitForEndOfFrame();
    Firebase.Analytics.FirebaseAnalytics.LogEvent(code, paramName, paramValue);
  }

  IEnumerator LogAsync(string code, string paramName, int paramValue) {
    yield return new WaitForEndOfFrame();
    Firebase.Analytics.FirebaseAnalytics.LogEvent(code, paramName, paramValue);
  }

  IEnumerator LogAsync(string code, string paramName, long paramValue) {
    yield return new WaitForEndOfFrame();
    Firebase.Analytics.FirebaseAnalytics.LogEvent(code, paramName, paramValue);
  }

}
