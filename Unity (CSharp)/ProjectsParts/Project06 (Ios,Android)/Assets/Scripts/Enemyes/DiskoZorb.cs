using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class DiskoZorb : Enemy {

	private bool _isActiveSound;
	public static int countInScene;			// Количество зорбов на сцене

	public Transform raundTransform;
	public Transform headTransform;
	public Transform headShadowTransform;
	public List<Transform> roundPoint;
	public ZorbEye zorbEye;


	int countInit;

	protected override void OnEnable() {

		base.OnEnable();
		skeletonAnimation.skeleton.a = 1;

		transform.position = new Vector3(transform.position.x, DecorationManager.Instance.loaderLocation.roadSize.max, 0);
		skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = (int)(System.Math.Round(transform.position.y, 3) * -1000);
	}

	public override void OnDisable() {
		base.OnDisable();

		if (_isActiveSound)
			DeactiveSound();


	}

	void DeactiveSound() {
		try {
			_isActiveSound = false;
			countInScene--;
			if (countInScene <= 0) {
				audioFon.Stop();
				countInScene = 0;
			}
		} catch { }
	}

	public override void Init() {
		base.Init();
		_isActiveSound = true;
		countInScene++;
		if (countInScene == 1) PlayTrowAudio();
	}

	protected override void ChangeBattlePhase(BattlePhase phase) {
		base.ChangeBattlePhase(phase);
		if (phase == BattlePhase.end) {
			if(audioFon != null) audioFon.Stop();
		}
	}

	public override void Update() {
		base.Update();

		if (directionVelocity == 1 && transform.position.x > 14)
			SetDirectionVelocity(-1, false);
	}

	public override void Move() {
		base.Move();
		RotationObject();
	}

	void RotationObject() {

		raundTransform.localEulerAngles = new Vector3(raundTransform.localEulerAngles.x,
																									raundTransform.localEulerAngles.y,
																									raundTransform.localEulerAngles.z - velocity.x * Time.deltaTime * 50);

		

		headTransform.localEulerAngles = new Vector3(headTransform.localEulerAngles.x,
																									headTransform.localEulerAngles.y,
																									headTransform.localEulerAngles.z - velocity.x * Time.deltaTime * 160);

		headShadowTransform.localEulerAngles = new Vector3(headShadowTransform.localEulerAngles.x,
																									headShadowTransform.localEulerAngles.y,
																									headShadowTransform.localEulerAngles.z - velocity.x * Time.deltaTime * 160);
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

	protected override void HomeEnter(Collider2D newTarget) {
		if (LayerMask.LayerToName(newTarget.gameObject.layer) == "Player") {
			SetDirectionVelocity(1, false);
			RoundChange(headTransform.position + Vector3.left, 1f);
		}
	}

	protected override void SetDeadPhase() {
		base.SetDeadPhase();
		GenerateBullet();
		DeactiveSound();
		PlayDeadAudio();
	}

	public void GenerateBullet() {
		GameObject inst = PoolerManager.Spawn("ZorbHead");
		inst.SetActive(true);
		inst.transform.position = headTransform.position;
		inst.GetComponent<ZorbHead>().Init(transform.position.y);
	}

	// Кровотечение не получает
	public override void SetBleeding(float timeWorkFlameOfFire, float periodWorkFlameOfFire, float newDamageFlameOfFire) { }

	void RoundChange(Vector3 position, float radius) {

		List<Transform> points = roundPoint.FindAll(x => Vector3.Distance(x.position , position) <= radius );
		points.ForEach(x => x.GetComponent<ZorbPoint>().ChangePosition(1 - Vector3.Distance(x.position, position) / radius));
	}

	public void SetBound(GameObject damager) {
		RoundChange(headTransform.position + (damager.transform.position - headTransform.position).normalized * 1.1f, 0.5f);
	}

	public override bool Damage(GameObject damager, float value, float newSpeedDelay = 0, float newTimeSpeedDelay = 0, Component parent = null) {
		RoundChange(headTransform.position + (damager.transform.position - headTransform.position).normalized * 1.1f, 0.5f);
		return base.Damage(damager, value, newSpeedDelay, newTimeSpeedDelay, parent);
	}

	protected override void GetDamage(float stunDelay = 0, float stunTime = 0) {
		//ChangeHealthPanel();
	}

	public List<AudioClipData> trowAudio;
	public AudioBlock trowAudioBlock;
	protected void PlayTrowAudio() {
		trowAudioBlock.PlayRandom(this);
	}
	public static AudioPoint audioFon;
	
}
