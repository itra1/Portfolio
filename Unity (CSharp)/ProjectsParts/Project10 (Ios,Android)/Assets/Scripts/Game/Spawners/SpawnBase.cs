using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;

public abstract class SpawnBase<T>: EventBehaviour where T : EventBehaviour {

  protected static T _instance;

  public static T Instance {
    get {

      if (_instance == null) {
        _instance = (T)FindObjectOfType(typeof(T));

        if (_instance == null)
          Debug.LogError("Экземпляр " + typeof(T) + " отсутствует на сцене");
      }

      return _instance;
    }
  }

  protected DisplayDiff displayDiff;                            // Смещение видимой части

  protected void CalcDisplay() {
    displayDiff = CameraController.Instance.CalcDisplayDiff(transform.position.z);
  }

  /// <summary>
  /// Это iPad
  /// </summary>
  protected bool IsIpad {
    get { return GameManager.isIPad; }
  }

  /// <summary>
  /// Ключ пула
  /// </summary>
  public abstract string POOL_KEY { get; }

  protected List<float> cd = new List<float>();

}
