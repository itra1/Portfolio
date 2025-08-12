using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
/// <summary>
/// Ворон
/// </summary>
public class Sterva : Enemy {


	public MeshRenderer SpineFlyUp;
	public MeshRenderer SpineFlyDown;

	float leftBorder;               // Левая граница перемещения
	float rightBorder;              // Правая граница перемещения

	protected override void OnEnable() {

		base.OnEnable();
		leftBorder = CameraController.leftPointX.x + 4f;

		rightBorder = CameraController.rightPoint.x - 2f;
		//skeletonAnimationUp.Initialize(true);
		ResetAnimationUp();
		velocity.y = 2;

		skeletonAnimationUp.state.End -= AnimEnd;
		skeletonAnimationUp.state.End += AnimEnd;
		SpineFlyUp.enabled = true;
		SpineFlyDown.enabled = false;

		transform.position = new Vector3(transform.position.x, Random.Range(move1BottomBorder, move1TopBorder), transform.position.z);
	}
	
	public override void Update() {
		if (phase == Phase.dead) return;
		if (transform.position.x < leftBorder) {
			SetDirectionVelocity(1);
		}
		if(transform.position.x > rightBorder) {
			SetDirectionVelocity(-1);
		}

		if (phase == Phase.damage) {
			Move();
		}

		base.Update();
	}
	
	float move1TopBorder = 1.8f;
	float move1BottomBorder = 0;
	float move1MaxTopSpeed = 1;
	float move1MaxBottomSpeed = -1.5f;
	float gravityDown = -3f;
	float gravityUp = 2f;
	float gravity;


	public AudioBlock idleAudioBlock;
	public List<AudioClipData> karAudio;

	protected virtual void PlayIdleAudio() {
		idleAudioBlock.PlayRandom(this);
	}

	public override void Move() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1);
    if (gravity == 0)
    {
      AudioManager.PlayEffects(karAudio[Random.Range(0, karAudio.Count)], AudioMixerTypes.effectPlay);
      gravity = gravityUp;
    }

		velocity.y += gravity * Time.deltaTime;

		if(gravity > 0) {
			if(transform.position.y < move1TopBorder) {
				velocity.y = Mathf.Min(move1MaxTopSpeed, velocity.y);
			} else {
        AudioManager.PlayEffects(karAudio[Random.Range(0, karAudio.Count)], AudioMixerTypes.effectPlay);
        gravity = gravityDown;
        SetAnimationUp(0, transitionAnim, false);
			}
		} else {
			if(transform.position.y > move1BottomBorder) {
				//velocity.y += gravity * Time.deltaTime;
				velocity.y = Mathf.Max(move1MaxBottomSpeed, velocity.y);
			} else {
        AudioManager.PlayEffects(karAudio[Random.Range(0, karAudio.Count)], AudioMixerTypes.effectPlay);
        gravity = gravityUp;
        SetAnimation(transitionAnim, false);
			}
		}

		transform.position += velocity * Time.deltaTime;
		//rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}


	protected override void StartDeadAnim() {
		SpineFlyUp.enabled = false;
		SpineFlyDown.enabled = true;
		SetAnimation(deadAnim, false);
	}

	public override void AnimComplited(TrackEntry trackEntry) {
		if (phase == Phase.dead) return;
		if (trackEntry.ToString() == transitionAnim && gravity == gravityDown) {
			SpineFlyUp.enabled = false;
			SpineFlyDown.enabled = true;
			SetRunAnimation();  
		}
		if(trackEntry.ToString() == transitionAnim && gravity == gravityUp) {
			SpineFlyUp.enabled = true;
			SpineFlyDown.enabled = false;
			SetRunAnimation();
		}
	}
	
	public SkeletonAnimation skeletonAnimationUp;

	public override void SetRunAnimation() {

		if(gravity == gravityDown) {
			SetAnimation(runAnim, true);
		} else {
			SetAnimationUp(0, runAnimUp, true);
		}
	}


	public override void SetAnimation(int index, string animName, bool loop) {
		if(currentAnimation != animName || !loop) {
			try {
				currentAnimation = animName;
				skeletonAnimation.state.SetAnimation(index, animName, loop);
			} catch { }
		}
	}
	public void SetAnimationUp(int index, string animName, bool loop) {
		if(currentAnimation != animName || !loop) {


			try {
				currentAnimation = animName;
				skeletonAnimationUp.state.SetAnimation(index, animName, loop);
			} catch { }
		}
	}

	public virtual void ResetAnimationUp() {

		skeletonAnimationUp.Initialize(true);
		skeletonAnimationUp.state.End += AnimEnd;
		skeletonAnimationUp.state.Complete += AnimComplited;
		skeletonAnimationUp.state.Start += AnimStart;
		skeletonAnimationUp.state.Dispose += AnimDispose;
		skeletonAnimationUp.state.Interrupt += AnimInterrupt;
		currentAnimation = null;
	}

	[SpineAnimation(dataField: "skeletonAnimationUp")]
	public string runAnimUp = "";     // Анимация хотьбы

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string transitionAnim = "";     // Анимация хотьбы
	
}
