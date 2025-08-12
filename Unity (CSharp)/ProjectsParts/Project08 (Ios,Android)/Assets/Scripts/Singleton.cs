using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;

public class Singleton<T> : EventBehaviour where T : EventBehaviour {
	protected static T _instance;

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
