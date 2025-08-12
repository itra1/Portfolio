using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ImageFinder : MonoBehaviour {


	//[MenuItem("Extensions/Bundles/Find Imeges")]
	public static void FindedImages() {

		Object[] imageArr = FindObjectsOfTypeAll(typeof(Image));
		
		for (int elem = 0; elem < imageArr.Length; elem++) {

			Image im = imageArr[elem] as Image;

			if (im.sprite != null && im.sprite.texture.name != "UiPacker") {
				Debug.Log(im.gameObject.name);
			}
		}


	}


}
