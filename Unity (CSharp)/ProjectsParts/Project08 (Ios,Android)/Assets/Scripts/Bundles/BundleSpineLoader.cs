using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class BundleSpineLoader : MonoBehaviour {

	public string assetName;

	private SkeletonAnimation _skeleton;

	private SkeletonAnimation skeleton {

		get {

			if (_skeleton == null)
				_skeleton = GetComponent<SkeletonAnimation>();

			return _skeleton;
		}
	}

	private void Start() {
		
		try {
			skeleton.skeletonDataAsset = GraphicManager.Instance.link.singleSpineAssets.Find(x => x.name == assetName);
			skeleton.Initialize(true);
		} catch (System.Exception ex) {
			Debug.LogError(ex.Message);
			Debug.LogError("No exists spine asset " + assetName);
		}
	}
}
