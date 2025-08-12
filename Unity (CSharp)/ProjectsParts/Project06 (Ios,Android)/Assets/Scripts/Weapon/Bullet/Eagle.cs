using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using System;

namespace Game.Weapon
{
  


/// <summary>
/// Орел
/// </summary>
public class Eagle : ExEvent.EventBehaviour {

	public Action OnDeactive;
	private EaglePhase phase;

	public Rigidbody2D rb;

	public MeshRenderer spineFlyUp;
	public MeshRenderer spineFlyDown;

	public SkeletonAnimation skeletonAnimationTop;
	public SkeletonAnimation skeletonAnimationDown;

	public ColliderHelper collHelperTop;
	public ColliderHelper collHelperDown;
	
	public Transform mainGraphic;

	private Transform enemyTransform;
	private Enemy enemyController;
	private Vector3 targetPosition;
	private Vector3 targetPoint;

	private string _transitionAnim = "transition";

	public WeaponType weaponType;

	float radiusPoint = 3;
	float speed = 1;
	float timeLive;
	float startTime;

	Vector3 velocity = Vector3.zero;
	
	protected Dictionary<string, object> xmlConfig;
	Enemy[] enemyMasToMagic;
	Enemy targetMagicObject;
	
	float timeNextAttack;
	float distanse;

	bool isTopGraphic;

	Vector3 lastVelocity = Vector3.zero;
	
	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
	public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase) {
		if (phase.phase == BattlePhase.start) {
			gameObject.SetActive(false);
		}
	}


	protected string currentAnimation;                  // Текущая анимция

	protected override void Awake() {
		base.Awake();
		ResetAnimation();
	}

	void Start() {
		timeNextAttack = Time.time;
	}

	public SkeletonAnimation activeSceletonAnimation() {
		return (spineFlyUp.enabled ? skeletonAnimationTop : skeletonAnimationDown);
	}

	void ActivateGraphic(bool isTopGraph) {


		if (this.isTopGraphic != isTopGraph)
			SetAnimation(transitionAnim, false, false);


	}

	void OnEnable() {
		GetConfig();
		startTime = Time.time;
		phase = EaglePhase.forvard;

		isTopGraphic = false;
		ChangeAnimation();
		//timeNextAttack = Time.time;
	}

	private void OnDisable() {
		if (OnDeactive != null) OnDeactive();
	}
	
	private void OnDrawGizmos() {
		Gizmos.DrawWireSphere(targetPoint, radiusPoint);
	}

	private void FixedUpdate() {
		FixedMove();
	}

	void CheckEnemy() {

		if (enemyTransform != null && enemyTransform.gameObject.activeInHierarchy && (enemyTransform.position - targetPoint).magnitude < radiusPoint * 1.5f)
			return;

		if(enemyTransform != null)
			Debug.Log((enemyTransform.position - targetPoint).magnitude + " : " + radiusPoint * 2);

		enemyTransform = null;
		if (phase == EaglePhase.attackForvard)
			SetPhase(EaglePhase.idle);

		RaycastHit2D[] hits = Physics2D.CircleCastAll(targetPoint, radiusPoint, Vector3.zero);

		foreach (var elem in hits) {
			if (elem.collider.GetComponent<Sterva>() && elem.collider.GetComponent<Sterva>().phase != Enemy.Phase.dead && elem.collider.gameObject.activeInHierarchy) {
				enemyTransform = elem.collider.transform;
				enemyController = elem.collider.GetComponent<Enemy>();
			}
		}

		if(phase == EaglePhase.idle && enemyTransform != null)
			SetPhase(EaglePhase.attackForvard);
	}

	void FixedMove() {

		if (phase != EaglePhase.forvard && phase != EaglePhase.back && phase != EaglePhase.attackAnim)
			CheckEnemy();

		if (phase != EaglePhase.attackAnim) {
			lastVelocity = velocity;
		}

		if (phase == EaglePhase.attackForvard) {
			velocity = (enemyTransform.position - transform.position).normalized * speed*2;
		} else if(phase == EaglePhase.attackAnim) {
			velocity = velocity.normalized;
		}else {
			velocity = (targetPosition - transform.position).normalized * speed;
		}

		if (phase != EaglePhase.attackAnim) {

			if (Mathf.Sign(lastVelocity.y) != Mathf.Sign(velocity.y))
				ActivateGraphic(velocity.y < 0);
			mainGraphic.transform.localScale = new Vector3(Mathf.Sign(velocity.x), 1, 1);
		}


		rb.velocity = velocity;
	}

	private void Update() {
		CheckPhase();

		if(phase == EaglePhase.attackForvard && enemyTransform != null && enemyController.phase == Enemy.Phase.dead) {
			enemyTransform = null;
			SetPhase(EaglePhase.idle);
		}

		if (phase == EaglePhase.attackForvard && phase != EaglePhase.attackAnim && enemyTransform != null && enemyTransform.gameObject.activeInHierarchy)
			Debug.Log((enemyTransform.position - transform.position).magnitude);


		if (phase == EaglePhase.attackForvard && phase != EaglePhase.attackAnim && enemyTransform != null && enemyTransform.gameObject.activeInHierarchy && (enemyTransform.position - transform.position).magnitude < 1f) {
			SetPhase(EaglePhase.attackAnim);
			SetAnimation((isTopGraphic ? attackTopAnim : attackDownAnim), true, false);
		}
	}

	private void CheckPhase() {

		if (phase == EaglePhase.forvard || phase == EaglePhase.idle) {
			if ((targetPosition - transform.position).magnitude <= 0.2f) {

				if (enemyTransform != null) {
					SetPhase(EaglePhase.attackForvard);
				} else {
					SetPhase(EaglePhase.idle);
				}

			}
		}

		if (phase == EaglePhase.idle && (targetPosition - transform.position).magnitude <= 0.2f) {
			CalcNewPoint();
		}

		if (phase != EaglePhase.back && Time.time - startTime > timeLive) {
			SetPhase(EaglePhase.back);
		}

		if (phase == EaglePhase.back && (targetPosition - transform.position).magnitude <= 0.2f) {
			gameObject.SetActive(false);
		}

	}

	private void CalcNewPoint() {
		do {
			targetPosition = targetPoint + new Vector3(UnityEngine.Random.Range(-radiusPoint, radiusPoint), UnityEngine.Random.Range(-radiusPoint, radiusPoint), 0);
		} while ((targetPosition - transform.position).magnitude <= radiusPoint / 2 || targetPosition.y > 4.5f);
	}

	private void SetPhase(EaglePhase phase) {

		if (phase == EaglePhase.back) {
			enemyTransform = null;
			targetPosition = new Vector3(PlayerController.Instance.transform.position.x - 4, PlayerController.Instance.transform.position.y, 0);
		}

		//Debug.Log(phase);

		this.phase = phase;
	}

	public void SetPointTap(Vector3 pointTap) {
		this.targetPosition = pointTap;
		targetPoint = pointTap;
		velocity = pointTap - transform.position;
	}

	void GetTargetAttack() {
		targetMagicObject = null;
		distanse = 100;
		enemyMasToMagic = EnemysSpawn.GetAllEnemy;
		Debug.Log("count  " + enemyMasToMagic.Length);
		for (int i = 0; i < enemyMasToMagic.Length; i++) {
			if (enemyMasToMagic[i].enemyType == EnemyType.Amurka && distanse < Vector3.Distance(transform.position, enemyMasToMagic[i].transform.position)) {
				distanse = Vector3.Distance(transform.position, enemyMasToMagic[i].transform.position);
				targetMagicObject = enemyMasToMagic[i];
			}
		}
	}
	
	public virtual void GetConfig() {
		
		Configuration.Weapon wep = GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)weaponType);

		speed = wep.param1.Value;
		radiusPoint = wep.param2.Value;
		timeLive = wep.param3.Value;
	}

	public virtual void AttackEvent() {

		if(enemyTransform == null || !enemyTransform.gameObject.activeInHierarchy) {
			enemyTransform = null;
			SetPhase(EaglePhase.idle);
			return;
		}

		Enemy targetEnemy = enemyTransform.GetComponent<Enemy>();

		targetEnemy.Damage(gameObject, 20, 0, 0, GetComponent<Enemy>());

		targetEnemy = null;
		SetPhase(EaglePhase.idle);

	}


	[Flags]
	public enum EaglePhase {
		forvard = 0,
		back = 1,
		idle = 2,
		attackForvard = 4,
		attackAnim = 8
	}

	void ResetAnimation() {

		//	public SkeletonAnimation skeletonAnimationTop;
		//public SkeletonAnimation skeletonAnimationDown;

		skeletonAnimationTop.Initialize(true);
		skeletonAnimationDown.Initialize(true);

		skeletonAnimationTop.state.Event += AnimEvent;
		skeletonAnimationDown.state.Event += AnimEvent;
		skeletonAnimationTop.state.End += AnimEnd;
		skeletonAnimationDown.state.End += AnimEnd;
		skeletonAnimationTop.state.Complete += AnimComplited;
		skeletonAnimationDown.state.Complete += AnimComplited;
		skeletonAnimationTop.state.Start += AnimStart;
		skeletonAnimationDown.state.Start += AnimStart;
		skeletonAnimationTop.state.Dispose += AnimDispose;
		skeletonAnimationDown.state.Dispose += AnimDispose;
		skeletonAnimationTop.state.Interrupt += AnimInterrupt;
		skeletonAnimationDown.state.Interrupt += AnimInterrupt;
		skeletonAnimationTop.OnRebuild = OnRebild;
		skeletonAnimationDown.OnRebuild = OnRebild;
		currentAnimation = null;
	}

	public virtual void AnimEnd(Spine.TrackEntry trackEntry) { }


	private void ChangeAnimation() {

		if (isTopGraphic) {
			spineFlyUp.enabled = true;
			spineFlyDown.enabled = false;
			skeletonAnimationDown.state.ClearTracks();
		} else {
			spineFlyUp.enabled = false;
			spineFlyDown.enabled = true;
			skeletonAnimationTop.state.ClearTracks();
		}
		
		SetAnimation((isTopGraphic ? runTopAnim : runDownAnim), true, false);
	}


	public virtual void AnimStart(Spine.TrackEntry trackEntry) {}
	public virtual void AnimDispose(Spine.TrackEntry trackEntry) { }
	public virtual void AnimInterrupt(Spine.TrackEntry trackEntry) { }
	protected virtual void OnRebild(SkeletonRenderer skeletonRenderer) { }
	public virtual void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
		if (e.Data.Name == "hit")
			AttackEvent();
	}
	public virtual void AnimComplited(Spine.TrackEntry trackEntry) {
		Debug.Log("AnimComplited " + trackEntry.ToString());
		if (trackEntry.ToString() == transitionAnim) {

			isTopGraphic = !isTopGraphic;
			ChangeAnimation();
		}

		if (phase == EaglePhase.attackAnim && trackEntry.ToString() == attackTopAnim || trackEntry.ToString() == attackDownAnim) {
			SetPhase(EaglePhase.idle);
		}
	}


	[SpineAnimation(dataField: "skeletonAnimationTop")]
	public string runTopAnim = "";     // Анимация хотьбы

	[SpineAnimation(dataField: "skeletonAnimationDown")]
	public string runDownAnim = "";     // Анимация хотьбы

	[SpineAnimation(dataField: "skeletonAnimationTop")]
	public string attackTopAnim = "";     // Анимация хотьбы

	[SpineAnimation(dataField: "skeletonAnimationDown")]
	public string attackDownAnim = "";     // Анимация хотьбы

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string transitionAnim = "";     // Анимация хотьбы


	/// <summary>
	/// Установка основной анимации
	/// </summary>
	/// <param name="anim">Имя анимации</param>
	/// <param name="loop">Флаг зацикливания</param>
	public virtual void SetAnimation(string anim, bool loop, bool reset = true) {

		if (reset) ResetAnimation();
		if (activeSceletonAnimation() != null) SetAnimation(0, anim, loop);
	}
	public virtual void SetAnimation(int index, string animName, bool loop) {
		if (currentAnimation != animName || !loop) {
			
			currentAnimation = animName;
			activeSceletonAnimation().state.SetAnimation(index, animName, loop);
		}
	}

	
}

}