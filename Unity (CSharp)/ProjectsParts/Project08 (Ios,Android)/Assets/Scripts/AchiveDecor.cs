using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using Spine.Unity;
using Animation = UnityEngine.Animation;

public class AchiveDecor : MonoBehaviour {

	public SkeletonAnimation skeletonAnimation;

	private void OnEnable() {
		skeletonAnimation.OnRebuild += AnimRebuild;
	}

	private void OnDisable() {
		skeletonAnimation.OnRebuild -= AnimRebuild;
		skeletonAnimation.state.Complete -= AnimCompleted;
	}

	private void AnimRebuild(SkeletonRenderer skel) {
		StartCoroutine(Inite());
	}

	IEnumerator Inite() {
		yield return null;
		if (skeletonAnimation.state != null) {
			skeletonAnimation.state.Complete += AnimCompleted;

			if (isRebuild) {
				isRebuild = false;
				skeletonAnimation.AnimationState.SetAnimation(0, "appear", false);
			}

		}
	}

	private void AnimCompleted(TrackEntry trackEntry) {
		if (trackEntry.Animation.Name == "appear") {
			PlayAnim("idle", false);
		}
	}
	
	public void FirstShow() {
		PlayAnim("appear", false);
	}

	public void PlayAnim(string animationName, bool isLoop) {
		try {
			//if(skeletonAnimation.skeletonDataAsset != null)
			//	skeletonAnimation.AnimationState.SetAnimation(0, animationName, isLoop);
			isRebuild = true;
		} catch { }
	}

	private bool isRebuild = false;

}
