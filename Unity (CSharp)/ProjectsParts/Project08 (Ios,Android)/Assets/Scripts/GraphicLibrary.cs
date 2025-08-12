using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GraphicLibrary))]
public class GraphicLibraryEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Save")) {
			EditorUtility.SetDirty(target);
		}

	}
}

#endif

public class GraphicLibrary : ScriptableObject {

#if UNITY_EDITOR
	[MenuItem("Assets/Create/Graphic Library Create")]
	public static void CreateGraphicLibrary() {
		string assetPath = GetSavePath();

		if (string.IsNullOrEmpty(assetPath))
			return;

		// Create the sprite packer instance
		GraphicLibrary asset = ScriptableObject.CreateInstance("GraphicLibrary") as GraphicLibrary;
		AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
		AssetDatabase.Refresh();
	}

	private static string GetSavePath() {
		string path = "Assets";
		Object obj = Selection.activeObject;

		if (obj != null) {
			path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

			if (path.Length > 0) {
				if (!System.IO.Directory.Exists(path)) {
					string[] pathParts = path.Split("/"[0]);
					pathParts[pathParts.Length - 1] = "";
					path = string.Join("/", pathParts);
				}
			}
		}

		return EditorUtility.SaveFilePanelInProject("Graphic Library", "Graphic Library", "asset", "Create a new sprite packer instance.", path);
	}
	
#endif


	public SkeletonDataAsset octopus;
	
	public List<LocationGraphic> graphicLocationList;

	public LocationGraphic GetGraphic(int num) {
		return graphicLocationList.Find(x => x.num == num);
	}

	public List<AchivcGraph> achiveList;

	public List<SkeletonDataAsset> fishList;

	public List<Sprite> cunchBig;
	public List<Sprite> cunchMini;

	public List<Sprite> singleImages;
	public List<SkeletonDataAsset> singleSpineAssets;
	
}
