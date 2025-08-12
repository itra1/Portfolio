using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Shop))]
public class ShopEditor: Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if (GUILayout.Button("Find All Product")) {
      ((Shop)target).productList = EditorUtil.GetAllPrefabsOfType<ProductBase>();
      EditorUtility.SetDirty(((Shop)target).gameObject);
    }

  }

}

#endif

public class Shop : Singleton<Shop> {

	public List<ProductBase> productList;

	public void BuyProduct(ProductBase product) {
    if (product.Buy()) {
      DarkTonic.MasterAudio.MasterAudio.PlaySound("UI", 1, 1, 0, "ByeInShop");
    }
	}

}
