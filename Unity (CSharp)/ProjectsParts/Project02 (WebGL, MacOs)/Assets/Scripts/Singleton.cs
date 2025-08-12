using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
  public static T _instance;

	public static T Instance {
		get {
			if (_instance == null) {
				_instance = (T)FindObjectOfType(typeof(T),true);

				if (_instance == null) {
					//it.Logger.LogError("Экземпляр " + typeof(T) + " отсутствует на сцене");
				}
			}

			return _instance;
		}
	}
}
