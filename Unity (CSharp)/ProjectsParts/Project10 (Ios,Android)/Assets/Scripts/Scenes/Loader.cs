using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Загрузчик сцены
/// </summary>
public class Loader: MonoBehaviour {

  private AsyncOperation async = null;
  public string sceneName;
  public RectTransform progress;
  public RectTransform progressBack;

  void Start() {
    StartCoroutine(StartLoad());
  }

  private IEnumerator StartLoad() {
    async = SceneManager.LoadSceneAsync(sceneName);
    float tempValue = 0;
    while (!async.isDone) {
      if (async.progress <= 0.5f) {
        if (tempValue < 0.5f) tempValue += 0.01f;
        progress.sizeDelta = new Vector2(progressBack.sizeDelta.x * tempValue, progress.sizeDelta.y);
      } else
        progress.sizeDelta = new Vector2(progressBack.sizeDelta.x * async.progress, progress.sizeDelta.y);

      yield return null;
    }
    yield return async;
  }
}
