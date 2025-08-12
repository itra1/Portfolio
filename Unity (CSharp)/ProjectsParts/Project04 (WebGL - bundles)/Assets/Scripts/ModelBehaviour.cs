using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ModelBehaviour))]
public class ModelBehaviourEditor : Editor {
	private Сlothes cv;
	private bool cb;
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		GUILayout.BeginHorizontal();
		cv = (Сlothes)EditorGUILayout.EnumPopup(cv);
		cb = EditorGUILayout.Toggle(cb);
		if (GUILayout.Button("Set")) {
			((ModelBehaviour)target).SetData(cv, cb);
		}
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Fing object")) {
			((ModelBehaviour) target).GetMeshes();
		}

	}
}

#endif

public class ModelBehaviour : MonoBehaviour {

	public List<ClothesConfug> clothesConfig;

	public SkinnedMeshRenderer[] allRenderer;
	public MeshRenderer[] allMeshRenderer;


	public GameObject[] deactiveteStart;

	private void Start() {
		foreach (var value in Enum.GetValues(typeof(Сlothes)))
			SetData((Сlothes) value, true);
	}


	public void SetVisibleFog(bool isVisible) {
		
		if (allRenderer.Length == 0)
			GetMeshes();
		
		for (int i = 0; i < allRenderer.Length; i++)
			allRenderer[i].enabled = isVisible;
		for (int i = 0; i < allMeshRenderer.Length; i++)
			allMeshRenderer[i].enabled = isVisible;

	}
	
	public void GetMeshes() {
		allRenderer = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		allMeshRenderer = GetComponentsInChildren<MeshRenderer>(true);
	}


	private void Awake() {
		for (int i = 0; i < deactiveteStart.Length; i++) {
			deactiveteStart[i].gameObject.SetActive(false);
		}
		allRenderer = GetComponentsInChildren<SkinnedMeshRenderer>(true);
		allMeshRenderer = GetComponentsInChildren<MeshRenderer>(true);
	}

	public void SetData(Сlothes clt, bool isActive) {
		ClothesConfug cc = clothesConfig.Find(x => x.type == clt);

		try {
			cc.showElements.ForEach(x => x.SetActive(isActive));
			cc.hideElements.ForEach(x => x.SetActive(!isActive));
		}
		catch {
		}
	}

}

[System.Serializable]
public struct ClothesConfug {
	public Сlothes type;
	public List<GameObject> showElements;
	public List<GameObject> hideElements;
}

public enum Сlothes {
	helmet,
	shoulders,
	cuirass,
	pants
}