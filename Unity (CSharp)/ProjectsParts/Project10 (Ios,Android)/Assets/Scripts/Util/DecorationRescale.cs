using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DecorationRescale))]
public class DecorationRescaleEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Rescale")) {
			((DecorationRescale)target).RescaleDecorStruct();
		}
	}
}

#endif


/// <summary>
/// Рескалирование декораций
/// </summary>
public class DecorationRescale : MonoBehaviour {

	public float allScale;
	public GameObject graphic;

	public List<GameObject> decorList;
	
	public void RescaleDecorStruct() {

		RecusRescale(graphic, new Vector3(allScale, allScale, allScale));

		foreach (GameObject oneDecor in decorList) {

			Debug.Log("Обработка декора " + oneDecor.name);
			//RescalParam(oneDecor.name, 1 * oneDecor.transform.localScale.x);
			RecusRescale(oneDecor.transform.GetChild(0).gameObject, new Vector3(allScale, allScale, allScale));
		}

	}


	Vector3 RecusRescale(GameObject obj, Vector3 scaleData) {

		Debug.Log("Объект " + obj.name);

		Vector3 scale = obj.transform.localScale;
		scaleData = new Vector3(scaleData.x * (scale.x / Vector3.one.x), scaleData.y * (scale.y / Vector3.one.y), scaleData.z * (scale.z / Vector3.one.z));
		
		obj.transform.localPosition = new Vector3(obj.transform.localPosition.x * scaleData.x, obj.transform.localPosition.y * scaleData.y, obj.transform.localPosition.z * scaleData.z);

		if (obj.transform.childCount > 0) {
			obj.transform.localScale = Vector3.one;
			return RecusRescale(obj.transform.GetChild(0).gameObject, scaleData);
		} else {
			obj.transform.localScale = scaleData;
			Debug.Log("Скалировани " + scaleData);
			return scaleData;
		}

	}

	void RescalParam(string gameObjectName, float scale) {

		List<WallDecorationParametrs> decorationParametrs = GetComponent<WallDecor>().decorationParametrs;

		foreach (WallDecorationParametrs oneWallParam in decorationParametrs) {
			foreach (var decorObj in oneWallParam.param.decorObjects) {
				for (int i = 0; i < decorObj.prefabs.Length; i++) {
					if (decorObj.prefabs[i].prefab.name == gameObjectName) {
						decorObj.prefabs[i].sceles.min *= scale;
						decorObj.prefabs[i].sceles.max *= scale;
					}

				}
			}
		}

	}

}

