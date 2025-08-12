using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonGraphic))]
public class BundleSpineGraphicLoader : MonoBehaviour {

	public string assetName;

	private SkeletonGraphic _skeleton;

	private SkeletonGraphic skeleton {

		get {

			if (_skeleton == null)
				_skeleton = GetComponent<SkeletonGraphic>();

			return _skeleton;
		}
	}

	private void Start() {

		foreach (var elem in GraphicManager.Instance.link.singleSpineAssets)
			Debug.Log(elem.name);

		try {
			skeleton.skeletonDataAsset = GraphicManager.Instance.link.singleSpineAssets.Find(x => x.name == assetName);
			skeleton.Initialize(true);
		} catch (System.Exception ex) {
			Debug.LogError(ex.Message);
			Debug.LogError("No exists spine asset " + assetName);
		}
	}
}
