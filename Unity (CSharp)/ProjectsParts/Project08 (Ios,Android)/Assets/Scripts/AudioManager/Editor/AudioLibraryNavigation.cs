using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AudioManagement {

	public class AudioLibraryNavigation : Editor {

		[MenuItem("Audio Management/Create Audio Library")]
		public static void CreateMapBlockLibraryMenu() {
			CreateAudioLibrary();
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

		private static void CreateAudioLibrary() {
			string assetPath = GetSavePath("Audio Library", "Audio Library", "asset", "Create a new Audio library instance.");

			if (string.IsNullOrEmpty(assetPath))
				return;

			// Create the sprite packer instance
			AudioLibrary asset = ScriptableObject.CreateInstance("AudioLibrary") as AudioLibrary;
			AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
			AssetDatabase.Refresh();
		}

	}
}