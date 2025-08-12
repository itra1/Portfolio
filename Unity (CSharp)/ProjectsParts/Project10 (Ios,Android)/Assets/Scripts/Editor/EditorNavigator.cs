using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EditRun {

	public class EditorNavigator : Editor {
		
		[MenuItem("Utils/Run Manager/Run Blocks Manager %#m")]
		public static void OpenRunManagerMenu() {
			EditorWindow.GetWindow(typeof(RunBlockManagerWindow));
		}
    
    [MenuItem("Utils/Run Manager/Create/Create Map Block Library")]
		public static void CreateMapBlockLibraryMenu() {
			CreateMapBlockLibrary();
		}

		[MenuItem("Utils/Run Manager/Create/Create Spawn Object")]
		public static void CreateSpawnObjectMenu() {
			CreateSpawnObject();
		}

		[MenuItem("Assets/Create/Run Manager/Create Spawn Object")]
		public static void CreateSpawnObjectMenuContext() {
			CreateSpawnObject();
		}

		[MenuItem("Assets/Create/Run Manager/Create Map Block Library")]
		public static void CreateMapBlockLibraryMenuContext() {
			CreateMapBlockLibrary();
		}

    [MenuItem("Utils/Run Manager/Create/Create Stone Downfall Block")]
    [MenuItem("Assets/Create/Run Manager/Create Stone Downfall Block")]
    public static void CreateStoneDownfallBlock() {
      CreateScriptableObject<StoneDownfallBlock>("StoneDownfallBlock", "StoneDownfallBlock");
    }

		[MenuItem("Utils/Run Manager/Create/Create Map Block")]
		public static void CreateMapBlockMenu() {
			CreateMapBlock();
		}

		[MenuItem("Assets/Create/Run Manager/Create Map Block")]
		public static void CreateMapBlockMenuContext() {
			CreateMapBlock();
		}

		private static string GetSavePath(string title, string defaultName, string extension, string message) {
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

			return EditorUtility.SaveFilePanelInProject(title, defaultName, extension, message, path);
		}
       
		[MenuItem("Assets/Create/Run Manager/Create Level Order")]
		public static void CreateLevelOrder() {
			string assetPath = GetSavePath("Level Order", "Level Order", "asset", "Create a new Level Order instance.");

			if (string.IsNullOrEmpty(assetPath))
				return;

			// Create the sprite packer instance
			LevelDataOrders asset = ScriptableObject.CreateInstance("LevelDataOrders") as LevelDataOrders;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
			AssetDatabase.Refresh();
		}

    private static void CreateScriptableObject<T>(string title, string defaultName, string message = "Create a new instance.", string extension = "asset") where T: ScriptableObject {
      string assetPath = GetSavePath(title, defaultName, extension, message);

      if (string.IsNullOrEmpty(assetPath))
        return;

      T asset = ScriptableObject.CreateInstance(title) as T;
      AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
      AssetDatabase.Refresh();
    }

		private static void CreateMapBlockLibrary() {
			string assetPath = GetSavePath("Map Block Library", "Map Block Library", "asset", "Create a new map block library instance.");

			if (string.IsNullOrEmpty(assetPath))
				return;

			// Create the sprite packer instance
			EditRunLibrary asset = ScriptableObject.CreateInstance("EditRunLibrary") as EditRunLibrary;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
			AssetDatabase.Refresh();
		}

		public static void CreateMapBlock() {
			string assetPath = GetSavePath("Map Block", "Map Block", "asset", "Create a new map block instance.");

			if (string.IsNullOrEmpty(assetPath))
				return;

			// Create the sprite packer instance
			RunBlock asset = ScriptableObject.CreateInstance("RunBlock") as RunBlock;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
			AssetDatabase.Refresh();
			
		}

		public static void CreateSpawnObject() {
			string assetPath = GetSavePath("SpawnObject", "SpawnObject", "asset", "Create a spawn object instance.");

			if (string.IsNullOrEmpty(assetPath))
				return;

			// Create the sprite packer instance
			SpawnObjectInfo asset = ScriptableObject.CreateInstance("SpawnObjectInfo") as SpawnObjectInfo;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
			AssetDatabase.Refresh();
		}
		
		[MenuItem("Utils/Game Config")]
		public static void ShowGameConfigWindow() {
			EditorWindow.GetWindow(typeof(ConfigEditor));
			//window.gameManager = (GameManager)target;
			//window.quest = ((GameManager)target).GetComponent<Questions.QuestionManager>();
		}

    [MenuItem("Utils/Map Editor/Start editor")]
    public static void StartMapEditor() {

      if (EditorSceneManager.GetActiveScene().name == "ClassicRun") {
        StartEditor();
      } else {
        EditorSceneManager.OpenScene(Application.dataPath + "/Scenes/" + "ClassicRun.unity");
        StartEditor();
      }

    }

    public static void StartEditor() {
      OpenRunManagerMenu();
    }

  }
}