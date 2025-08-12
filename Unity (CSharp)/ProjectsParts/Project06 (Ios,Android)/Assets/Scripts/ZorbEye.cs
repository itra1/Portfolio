using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[ExecuteInEditMode]
public class ZorbEye : MonoBehaviour {
	public float animationTime;
	public Transform headTransform;

	#region Inspector
	[Range(0,1)]
	public float fillPercent = 0;

	[SpineAnimation]
	public string fillAnimationName;
	#endregion

	void Update() {
		SetEye(Mathf.Abs(0.5f - (headTransform.localEulerAngles.z % 360) / 360));
	}

	public SkeletonRenderer skeletonRenderer;
	Spine.Animation fillAnimation;
	
	public void SetEye(float percent) {
		if (skeletonRenderer == null) return;
		var skeleton = skeletonRenderer.skeleton; if (skeleton == null) return;

		// Make super-sure that fillAnimation isn't null.
		if (fillAnimation == null) {
			fillAnimation = skeleton.Data.FindAnimation(fillAnimationName);
			if (fillAnimation == null) return;
		}
		fillAnimation.Apply(skeleton, 0, animationTime * percent, false, null, 1f, Spine.MixPose.Current, Spine.MixDirection.In);

		skeleton.Update(Time.deltaTime);
		skeleton.UpdateWorldTransform();
	}

	private void FixedUpdate() {
		//SetEye(0.8f);
		//SetEye((headTransform.localEulerAngles.z % 360) / 360);
	}

}

