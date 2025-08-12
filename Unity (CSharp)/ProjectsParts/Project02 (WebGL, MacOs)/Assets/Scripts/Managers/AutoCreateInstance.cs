using System.Collections;

using UnityEngine;


public class AutoCreateInstance<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
				CreateInstance();
			return _instance;
		}
	}

	private static void CreateInstance()
	{
		if (_instance != null) return;

		var inst = new GameObject();

		_instance = inst.AddComponent<T>();

#if UNITY_EDITOR
		inst.gameObject.name = typeof(T).Name;
		//inst.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif

	}

}