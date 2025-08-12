using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviourBase where T : MonoBehaviourBase {
  public static T _instance;

	public static T Instance {
		get {
			if (_instance == null) {
				_instance = (T)FindObjectOfType(typeof(T));

				if (_instance == null) {
					Debug.LogError("Экземпляр " + typeof(T) + " отсутствует на сцене");
				}
			}

			return _instance;
		}
	}
}
