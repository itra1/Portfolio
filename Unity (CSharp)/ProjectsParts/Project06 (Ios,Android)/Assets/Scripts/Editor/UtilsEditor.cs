using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class UtilsEditor : Editor {

	[MenuItem("Extensions/Util")]
	public static void UtilGame() {
		EditorPlugins window = (EditorPlugins)EditorWindow.GetWindow(typeof(EditorPlugins));
	}

	[MenuItem("Assets/Create/ShopLibrary")]
	public static void CreateShopAsset() {
		EditorUtils.CreateAsset<ShopLibrary>("ShopLibrary", "ShopLibrary");
	}
	
	[MenuItem("Assets/Create/Cat Scene/Cat Scene Library")]
	public static void CreateCatSceneLibraryAsset() {
		EditorUtils.CreateAsset<ZbCatScene.CatSceneLibrary>("CatSceneLibrary", "CatSceneLibrary");
	}

	[MenuItem("Assets/Create/Cat Scene/Cat Scene")]
	public static void CreateCatSceneAsset() {
		EditorUtils.CreateAsset<ZbCatScene.CatScene>("CatScene", "CatScene");
	}

	[MenuItem("Assets/Create/Cat Scene/Blocks/Cat Block Dialog")]
	public static void CreateCatBlockDialogAsset() {
		EditorUtils.CreateAsset<ZbCatScene.CatBlockDialog>("CatBlockDialog", "CatBlockDialog");
	}

	[MenuItem("Assets/Create/Cat Scene/Blocks/Cat Block Focus")]
	public static void CreateCatBlockFocusAsset() {
		EditorUtils.CreateAsset<ZbCatScene.CatBlockFocus>("CatBlockFocus", "CatBlockFocus");
	}

}
