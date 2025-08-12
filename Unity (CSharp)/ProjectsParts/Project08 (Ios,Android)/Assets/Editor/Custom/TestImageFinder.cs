using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TestImageFinder))]
public class TestImageFinderEditor : Editor {

	private string fileName;
	private string fileNameSpine;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Поиск image")) {
			((TestImageFinder) target).FindedImages();
		}

		if (GUILayout.Button("Поиск sprite renderer")) {
			((TestImageFinder)target).FindedSpriteRenderer();
		}

		GUILayout.BeginHorizontal();

		fileName = EditorGUILayout.TextField("Название файла", fileName);
		
		if (GUILayout.Button("Замена")) {
			((TestImageFinder)target).ImageReplace(fileName);
		}

		GUILayout.EndHorizontal();

		if (GUILayout.Button("Поиск spine asset")) {
			((TestImageFinder)target).FindSpineAsset();
		}

		GUILayout.BeginHorizontal();

		fileNameSpine = EditorGUILayout.TextField("Название spine", fileNameSpine);

		if (GUILayout.Button("Замена")) {
			((TestImageFinder)target).SpriteAssetReplace(fileNameSpine);
		}

		GUILayout.EndHorizontal();
		

		if (GUILayout.Button("Поиск graphic spine asset")) {
			((TestImageFinder)target).FindGraphicSpine();
		}

	}
}

#endif

public class TestImageFinder : MonoBehaviour {

	public List<Image> imageArr;

	public void FindedImages() {
		imageArr.Clear();

		Object[] objArr = FindObjectsOfTypeAll(typeof(Image));

		for (int elem = 0; elem < objArr.Length; elem++) {

			Image im = objArr[elem] as Image;

			if (im.sprite != null && im.sprite.texture.name != "UiPacker" && im.sprite.texture.name != "Button") {
				imageArr.Add(im);
			}
		}
	}

	public List<SpriteRenderer> spriteRebderArray;

	public void FindedSpriteRenderer() {
		spriteRebderArray.Clear();

		Object[] objArr = FindObjectsOfTypeAll(typeof(SpriteRenderer));
		
		for (int elem = 0; elem < objArr.Length; elem++) {

			SpriteRenderer im = objArr[elem] as SpriteRenderer;

			if (im.sprite != null && im.sprite.texture.name != "UiPacker" && im.sprite.texture.name != "Button") {
				spriteRebderArray.Add(im);
			}
		}
	}

	public List<SkeletonAnimation> spineLibrary;

	public void FindSpineAsset() {
		spineLibrary.Clear();

		Object[] objArr = FindObjectsOfTypeAll(typeof(SkeletonAnimation));

		for (int elem = 0; elem < objArr.Length; elem++) {

			SkeletonAnimation im = objArr[elem] as SkeletonAnimation;

			if (im.skeletonDataAsset != null) {
				spineLibrary.Add(im);
			}
		}
	}

	/// <summary>
	/// Поиск замена картинок
	/// </summary>
	public void ImageReplace(string imageName) {

		FindedSpriteRenderer();

		Object[] objArr = FindObjectsOfTypeAll(typeof(GraphicLibrary));

		GraphicLibrary lib = objArr[0] as GraphicLibrary;

		int replaceCount = 0;

		foreach (var spritRend in spriteRebderArray) {

			if (spritRend.sprite != null && spritRend.sprite.name == imageName && spritRend.gameObject.GetComponent<BundleSpriteLoader>() == null) {

				Sprite targeSprite = lib.singleImages.Find(x => x.name == imageName);

				if (targeSprite == null) {
					targeSprite = spritRend.sprite;
					lib.singleImages.Add(targeSprite);
					EditorUtility.SetDirty(lib);
				}
				BundleSpriteLoader bsl = spritRend.gameObject.AddComponent<BundleSpriteLoader>();
				bsl.imageName = targeSprite.name;
				spritRend.sprite = null;
				EditorUtility.SetDirty(spritRend);

				replaceCount++;
			}

		}
		
		Debug.Log("Replace count " + replaceCount);

	}

	public void SpriteAssetReplace(string imageName) {

		FindSpineAsset();

		Object[] objArr = FindObjectsOfTypeAll(typeof(GraphicLibrary));

		GraphicLibrary lib = objArr[0] as GraphicLibrary;

		int replaceCount = 0;

		foreach (var spritRend in spineLibrary) {

			if (spritRend.skeletonDataAsset != null && spritRend.skeletonDataAsset.name == imageName && spritRend.gameObject.GetComponent<BundleSpineLoader>() == null) {

				SkeletonDataAsset targeSprite = lib.singleSpineAssets.Find(x => x.name == imageName);

				if (targeSprite == null) {
					targeSprite = spritRend.skeletonDataAsset;
					lib.singleSpineAssets.Add(targeSprite);
					EditorUtility.SetDirty(lib);
				}
				BundleSpineLoader bsl = spritRend.gameObject.AddComponent<BundleSpineLoader>();
				bsl.assetName = targeSprite.name;
				spritRend.skeletonDataAsset = null;
				EditorUtility.SetDirty(spritRend);

				replaceCount++;
			}

		}

		Debug.Log("Replace count " + replaceCount);

	}

	public List<SkeletonGraphic> spineGraphLibrary;
	public void FindGraphicSpine() {
		spineGraphLibrary.Clear();

		Object[] objArr = FindObjectsOfTypeAll(typeof(SkeletonGraphic));

		for (int elem = 0; elem < objArr.Length; elem++) {

			SkeletonGraphic im = objArr[elem] as SkeletonGraphic;

			if (im.skeletonDataAsset != null) {
				spineGraphLibrary.Add(im);
			}
		}
	}


}
