using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ResourceManager))]
public class ResourceManagerEditro : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Unload")) {
			((ResourceManager)target).UnloadUnusedAssets();
		}
	}
}

#endif

public class ResourceManager : Singleton<ResourceManager> {

	private List<object> resourcesList = new List<object>();

	protected override void Awake() {
		base.Awake();
		SceneManager.sceneUnloaded += SceneChange;
  }

  private void Start()
  {

  }

  protected override void OnDestroy() {
		base.OnDestroy();
		SceneManager.sceneUnloaded -= SceneChange;
	}

	private void SceneChange(Scene scene) {
		UnloadUnusedAssets();
	}

	/// <summary>
	/// Загрузка ресурсов
	/// </summary>
	/// <typeparam name="T">Желаемый тип объекта</typeparam>
	/// <param name="resourcePath">Путь до ресурса от каталога Resources</param>
	/// <returns></returns>
	public T LoadResources<T>(string resourcePath) where T : Object {
		object res = Resources.Load<GameObject>(resourcePath);
		resourcesList.Add(res);
		return (T)res;
	}

	public bool UnloadResources(Object resourceObject) {
		try {
			Resources.UnloadAsset(resourceObject);
			resourcesList.Remove(resourceObject);
			return true;
		} catch (Exception ex) {
			Debug.LogWarning(ex);
			return false;
		}
	}

	public void UnloadUnusedAssets() {
		Resources.UnloadUnusedAssets();
		resourcesList.RemoveAll(x => x == null);
	}

}
