using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Spine;
using Spine.Unity;
using UnityEngine;

public class OctopusSpine : MonoBehaviour {

	public Action<string> OnStart;
	public Action<string> OnCompleted;
	public Action<string> OnEnd;
	public Action<string> OnDispose;
	public Action<string> OnInterrupt;
	public Action OnRebuild;

	public string actualAnimation;

	public SkeletonAnimation skeleton;

	private void Awake() {
		skeleton.skeletonDataAsset = GraphicManager.Instance.link.octopus;
	}



	[HideInInspector]
	public const string curiousAnim = "curious";
	[HideInInspector]
	public const string despondencyAnim = "despondency";
	[HideInInspector]
	public const string disgustinglyAnim = "disgustingly";
	[HideInInspector]
	public const string disgustingly2Anim = "disgustingly_2";
	[HideInInspector]
	public const string downHardAnim = "down_hard";
	[HideInInspector]
	public const string downLightAnim = "down_light";
	[HideInInspector]
	public const string fartAnim = "fart";
	[HideInInspector]
	public const string floatsIdleAnim = "floats_idle";
	[HideInInspector]
	public const string floatsStartAnim = "floats_start";
	[HideInInspector]
	public const string floatsFinishAnim = "floats_finish";
	[HideInInspector]
	public const string happyAnim = "happy";
	[HideInInspector]
	public const string happyNoWaweAnim = "happy2";
	[HideInInspector]
	public const string idleAnim = "idle";
	[HideInInspector]
	public const string idleNoWaweAnim = "idle2";
	[HideInInspector]
	public const string noAnim = "no";
	[HideInInspector]
	public const string reactAnim = "react";
	[HideInInspector]
	public const string sadAnim = "sad";
	[HideInInspector]
	public const string singAnim = "sing";
	[HideInInspector]
	public const string sing2Anim = "sing_2";
	[HideInInspector]
	public const string sing3Anim = "sing_3";
	[HideInInspector]
	public const string sleepAnim = "sleep";
	[HideInInspector]
	public const string sleepNoWaweAnim = "sleep2";
	[HideInInspector]
	public const string supriseAnim = "surprise";
	[HideInInspector]
	public const string upAnim = "up";
	[HideInInspector]
	public const string wonderingAnim = "wondering";
	[HideInInspector]
	public const string wondering2Anim = "wondering_2";
	[HideInInspector]
	public const string wondering3Anim = "wondering_3";
	[HideInInspector]
	public const string wondering4Anim = "wondering_4";
	[HideInInspector]
	public const string yesAnim = "yes";

	private void Start() {
		skeleton.Initialize(true);
	}

	private void OnApplicationPause(bool pause) {
		if(!pause && actualAnimation == idleAnim)
			PlayAnim(idleAnim,true);
	}
	
	private void OnEnable() {

		_sounds = GetComponent<OctopusSounds>();

		//skeleton.state.Start += AnimStart;
		//skeleton.state.End += AnimEnd;
		//skeleton.state.Complete += AnimCompleted;
		//skeleton.state.Interrupt += AnimInterrupt;
		//skeleton.state.Dispose += AnimDispose;
		skeleton.OnRebuild += AnimRebuild;
	}

	private void OnDisable() {
		skeleton.state.Start -= AnimStart;
		skeleton.state.Complete -= AnimCompleted;
		skeleton.state.End -= AnimEnd;
		skeleton.state.Interrupt -= AnimInterrupt;
		skeleton.state.Dispose -= AnimDispose;
		skeleton.OnRebuild -= AnimRebuild;
		skeleton.state.Event -= AnimEvent;
	}

	private void AnimStart(TrackEntry trackEntry) {
		if (OnStart != null) OnStart(trackEntry.Animation.Name);
	}

	private void AnimCompleted(TrackEntry trackEntry) {
		//Debug.Log("AnimCompleted " + trackEntry.Animation.Name);
		if (OnCompleted != null) OnCompleted(trackEntry.Animation.Name);
	}

	private void AnimEnd(TrackEntry trackEntry) {
		if (OnEnd != null) OnEnd(trackEntry.Animation.Name);
	}

	private void AnimInterrupt(TrackEntry trackEntry) {
		if (_sounds != null)
			_sounds.StopSleepAudio();
		if (OnInterrupt != null) OnInterrupt(trackEntry.Animation.Name);
	}
	private void AnimEvent(TrackEntry trackEntry, Spine.Event e) {

		if (_sounds == null) return;
		
		if(e.data.Name == "CloseEyes")
			_sounds.PlayEyeAudio();


	}

	private void AnimRebuild(SkeletonRenderer skel) {

		StartCoroutine(Inite());
	}

	IEnumerator Inite() {
		yield return null;
		skeleton.state.Start += AnimStart;
		skeleton.state.End += AnimEnd;
		skeleton.state.Complete += AnimCompleted;
		skeleton.state.Interrupt += AnimInterrupt;
		skeleton.state.Dispose += AnimDispose;
		skeleton.state.Event += AnimEvent;

		if (OnRebuild != null) OnRebuild();
	}

	private void AnimDispose(TrackEntry trackEntry) {
		if (OnDispose != null) OnDispose(trackEntry.Animation.Name);
	}

	private OctopusSounds _sounds;

	public void PlayAnim(string animationName, bool isLoop) {
		actualAnimation = animationName;

		if (skeleton == null || skeleton.AnimationState == null) return;

		if (_sounds != null) {
			switch (animationName) {
				case downHardAnim:
					_sounds.PlayWaterDownHardAudio();
					break;
				case fartAnim:
					_sounds.PlayBubbleAudio();
					break;
				case downLightAnim:
					_sounds.PlayWaterDownLightAudio();
					break;
				case happyAnim:
				case happyNoWaweAnim:
					_sounds.PlayHappyAudio();
					break;
				case idleAnim:
				case idleNoWaweAnim:
					_sounds.PlayIdleAudio();
					break;
				case noAnim:
					_sounds.PlayWordWrongAudio();
					break;
				case sadAnim:
					_sounds.PlaySadAudio();
					break;
				case sleepAnim:
				case sleepNoWaweAnim:
					_sounds.PlaySleepAudio();
					break;
				case supriseAnim:
					_sounds.PlaySurpriseAudio();
					break;
				case upAnim:
					_sounds.PlayWaterUpAudio();
					break;
				case yesAnim:
					_sounds.PlayWordCorrectAudio();
					break;
				case curiousAnim:
					_sounds.PlayCuriousAudio();
					break;
				case despondencyAnim:
					_sounds.PlayDespondencyAudio();
					break;
				case disgustinglyAnim:
					_sounds.PlayDisgustinglyAudio();
					break;
				case disgustingly2Anim:
					_sounds.PlayDisgustingly2Audio();
					break;
				case reactAnim:
					_sounds.PlayReactAudio();
					break;
				case singAnim:
					_sounds.PlaySingAudio();
					break;
				case sing2Anim:
					_sounds.PlaySing2Audio();
					break;
				case sing3Anim:
					_sounds.PlaySing3Audio();
					break;
				case wonderingAnim:
					_sounds.PlayWonderingAudio();
					break;
				case wondering2Anim:
					_sounds.PlayWondering2Audio();
					break;
				case wondering3Anim:
					_sounds.PlayWondering3Audio();
					break;
				case wondering4Anim:
					_sounds.PlayWondering4Audio();
					break;
			}
		}


		skeleton.AnimationState.SetAnimation(0, animationName, isLoop);
	}

}
