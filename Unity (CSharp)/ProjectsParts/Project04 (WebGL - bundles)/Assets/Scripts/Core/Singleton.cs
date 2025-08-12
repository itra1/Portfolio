using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExEvent;

public class Singleton<T> : EventBehaviour where T : EventBehaviour {
	protected static T instance;

	public static T Instance {
		get {

			if(typeof(T).Name == "CellDrawner" && instance == null)
				instance = (T)FindObjectOfType(typeof(T));

			if (instance == null) {
				instance = (T)FindObjectOfType(typeof(T));

				if (instance == null) {
					Debug.LogError("Экземпляр " + typeof(T) + " отсутствует на сцене");
				}
			}

			return instance;
		}
	}
}
