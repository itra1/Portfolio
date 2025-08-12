using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GuidForObject))]
[CanEditMultipleObjects]
public class GuidForObjectEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		GenerateGUIDButton();

		serializedObject.ApplyModifiedProperties();
	}

	public static Guid ConvertGuidToUUID(string guid) {
		Debug.Log(guid);
		byte[] net = new byte[16];
		for (int i = 0; i < 16; i++) {
			net[i] = Convert.ToByte("" + guid[i * 2] + guid[i * 2 + 1], 16);
		}
		return new System.Guid(net);
	}

	protected void GenerateGUIDButton() {
		if (GUILayout.Button("Generate GUID")) {
			foreach (var o in targets) {
				var myScript = o as GuidForObject;
				if (myScript && myScript.IsEditable) {
					string original_id = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(myScript.gameObject));
					myScript.ItemGUID = ConvertGuidToUUID(original_id);
					myScript.ItemGUIDString = myScript.ItemGUID.ToString();
					EditorUtility.SetDirty(myScript.gameObject);
				}
			}
		}
	}
}
#endif

public class GuidForObject : MonoBehaviour {
	private Guid itemGuid = Guid.Empty;
	public Guid ItemGUID {
		get { if (itemGuid == Guid.Empty) return new Guid(ItemGUIDString); return itemGuid; }
		set { itemGuid = value; }
	}
	public string ItemGUIDstr {
		get { if (itemGuid == Guid.Empty) return new Guid(ItemGUIDString).ToString(); return itemGuid.ToString(); }
	}
	public string ItemGUIDString;
	public bool IsEditable;

	void Awake() {
		// Строковый GUID назначается в редакторе, нужно обновить истинный
		if (String.IsNullOrEmpty(ItemGUIDString))
			throw new Exception("Generate GUID for prefab!");
		ItemGUID = new Guid(ItemGUIDString);
	}
}

public interface IGuidObject {
	Guid ItemGUID { get; }
	string ItemGUIDString { get; }
}