using Spine;
using System;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Аниматор игрока
/// </summary>
public class PlayerAnimator : MonoBehaviour {
	
	[SerializeField]
	private SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
	public event Action<string> OnAnimEvent;
	public event Action<string> OnAnimInterrupt;
	public event Action<string> OnAnimStart;
	public event Action<string> OnAnimComplited;

	public virtual void ResetAnimation() {

		skeletonAnimation.Initialize(true);
		skeletonAnimation.state.Event += AnimEvent;
		skeletonAnimation.state.Interrupt += AnimInterrupt;
		skeletonAnimation.state.Start += AnimStart;
		skeletonAnimation.state.Complete += AnimComplited;
		skeletonAnimation.state.Dispose += AnimDispose;

	}
	public string currentAnimation {
		get {
			return _currentAnimation;
		}
	}
	private string _currentAnimation;

	public virtual void SetAnimation(int index, string animName, bool loop) {
		_currentAnimation = animName;
		skeletonAnimation.state.SetAnimation(index, animName, loop);
	}

	public float animationTimeScale {
		get { return skeletonAnimation.timeScale; }
		set { skeletonAnimation.timeScale = value; }
	}

	public virtual void AnimStart(Spine.TrackEntry trackEntry) {
		if (OnAnimStart != null) OnAnimStart(trackEntry.ToString());
	}
	public virtual void AnimComplited(Spine.TrackEntry trackEntry) {
		if (OnAnimComplited != null) OnAnimComplited(trackEntry.ToString());
	}

	public virtual void AnimInterrupt(Spine.TrackEntry trackEntry) {
		if (OnAnimInterrupt != null) OnAnimInterrupt(trackEntry.ToString());
	}

	public virtual void AnimDispose(Spine.TrackEntry trackEntry) {
	}

	public void AnimEvent(TrackEntry trackEntry, Spine.Event e) {
		if (OnAnimEvent != null) OnAnimEvent(e.data.name);
	}
}