using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

/// <summary>
/// Чебуратор
/// </summary>
public class Cheburator : Enemy {

	int countInit;

	protected override void OnEnable() {
		if (IsInvoking("DeactiveShadow")) CancelInvoke("DeactiveShadow");
		base.OnEnable();
		Create();
	}

	void Create() {
		SetAnimation(appearance, false,false);
		SetPhase(Phase.wait);
	}
	public override void ResetAnimation() {
		if (countInit > 0) return;
		countInit++;
		skeletonAnimation.Initialize(true);
		skeletonAnimation.state.Event += AnimEvent;
		skeletonAnimation.state.End += AnimEnd;
		skeletonAnimation.state.Complete += AnimComplited;
		skeletonAnimation.state.Start += AnimStart;
		skeletonAnimation.state.Dispose += AnimDispose;
		skeletonAnimation.state.Interrupt += AnimInterrupt;
		skeletonAnimation.OnRebuild = OnRebild;
		currentAnimation = null;
	}

	public override void AnimComplited(TrackEntry trackEntry) {
		base.AnimEnd(trackEntry);
		if (trackEntry.ToString() == appearance) {
			//SetRunPhase();
			CheckPhase();

		}
		if (trackEntry.ToString() == deadAnim) {
			if (healthPanel != null)
				Destroy(healthPanel);
			DeactiveEnemy();
			
		}
	}

	protected override void StartDeadAnim() {
		base.StartDeadAnim();
		Invoke("DeactiveShadow", 1.1f);

	}
	public override void SetPhase(Phase newPhase, bool force = false) {
		if (newPhase == Phase.damage) return;
		base.SetPhase(newPhase, force);
		
		

	}

	protected override void DeactiveShadow() {
		
		shadow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
	}
	public override void SetRunAnimation() {
		if (runAnim != null) SetAnimation(runAnim, true, false);
	}

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string appearance = "";     // создания

}
