using UnityEngine;
using Spine.Unity;

/// <summary>
/// Общий контроллер врагов
/// </summary>
public abstract class Enemy : SpawnAbstract {

	public event EventAction OnDead;                  // Событие сметри
	public EventAction ShootFunction;

	public Transform groundPoint;

	[HideInInspector]
	public bool isGround;                             // Флаг косания земли
	public LayerMask groundMask;                      // Слой с поверхностью
	public RunnerPhase runnerPhase;                   // Текущая фаза забега
	public EnemyTypes enemyType;                      // Тип врага

	[HideInInspector]
	public bool isBarrier;
	[HideInInspector]
	public bool isBreak;

	public virtual void Start() { }

	public virtual void OnEnable() {
		runnerPhase = RunnerController.Instance.runnerPhase;
		RunnerController.OnChangeRunnerPhase += ChangePhase;
		AudioInit();
		ResetAnimation();
	}

	public virtual void OnDisable() {
		RunnerController.OnChangeRunnerPhase -= ChangePhase;
	}

	public void ChangePhase(RunnerPhase newPhase) {
		SetNewPhase(newPhase);
	}

	public virtual void SetNewPhase(RunnerPhase newPhase) {
		runnerPhase = newPhase;
	}

	protected virtual void FixedUpdate() {
		isGround = (Physics2D.OverlapCircle(groundPoint.position, 0.1f, groundMask) != null);
	}

	public virtual void Update() {
		//Атака
		if (ShootFunction != null) ShootFunction();
	}

	/// <summary>
	/// Событие смерти врага
	/// </summary>
	public virtual void DeadEnemy(bool generateCoins = true) {

    GetComponent<ShadowController>().instancePrefab.gameObject.SetActive(false);

		if (OnDead != null) {
			OnDead();
			OnDead = null;
		}
	}

	#region Animation

	[SerializeField]
	SkeletonAnimation skeletonAnimation;                    // Ссылка на скелетную анимацию
	protected string currentAnimation;                      // Текущая анимция

	/// <summary>
	/// Установка основной анимации
	/// </summary>
	/// <param name="anim">Имя анимации</param>
	/// <param name="loop">Флаг зацикливания</param>
	public virtual void SetAnimation(string anim, bool loop) {
		SetAnimation(0, anim, loop);
	}
	public virtual void SetAnimation(int index, string animName, bool loop) {
		if (currentAnimation != animName) {
			currentAnimation = animName;
			skeletonAnimation.state.SetAnimation(index, animName, loop);
		}
	}
	/// <summary>
	/// Повторная инициализация анимации
	/// </summary>
	public virtual void ResetAnimation() {

		skeletonAnimation.Initialize(true);
		skeletonAnimation.state.Event += AnimEvent;
		skeletonAnimation.state.End += AnimEnd;
		skeletonAnimation.state.Complete += AnimComplited;
		currentAnimation = null;
	}
	/// <summary>
	/// Отписываемся от события анимации
	/// </summary>
	public void DeleteEventAnimation() {
		skeletonAnimation.state.Event -= AnimEvent;
		skeletonAnimation.state.End -= AnimEnd;
		skeletonAnimation.state.Complete -= AnimComplited;
	}
	/// <summary>
	/// Добавление анимации к текущей
	/// </summary>
	/// <param name="index">Номер слоя</param>
	/// <param name="animName">Имя анимации</param>
	/// <param name="loop">Флаг зацикливания</param>
	/// <param name="delay">Задержка перед воспроизведением</param>
	public void AddAnimation(int index, string animName, bool loop, float delay = 0) {
		skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
	}
	/// <summary>
	/// Установка скорости воспроизведения анимации
	/// </summary>
	/// <param name="speed"></param>
	public void SpeedAnimation(float speed) {
		skeletonAnimation.timeScale = speed;
	}
	/// <summary>
	/// Событие анимации спайна
	/// </summary>
	/// <param name="state">Имя</param>
	/// <param name="trackIndex">Слой</param>
	/// <param name="e">Собятие</param>
	public virtual void AnimEvent(Spine.AnimationState state, int trackIndex, Spine.Event e) { }
	/// <summary>
	/// Событие окончание анимации спайна
	/// </summary>
	/// <param name="state">Название анимации</param>
	/// <param name="trackIndex">Номер трека</param>
	public virtual void AnimEnd(Spine.AnimationState state, int trackIndex) { }
	public virtual void AnimComplited(Spine.AnimationState state, int trackIndex, int loopCount) { }

	#endregion

	#region Audio

	protected AudioSource audioComp;          // Компонент звука
																						/// <summary>
																						/// Анициализация ссылки
																						/// </summary>
	void AudioInit() {
		audioComp = GetComponent<AudioSource>();
	}

  #endregion
  public virtual void Damage(WeaponTypes weaponType, float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) { }

  public virtual void Damage(WeaponTypes weaponType, ref float power, Vector3 damagePosition, DamagePowen powerDamage, float fire = 0, bool stoneDam = false) { }


	public virtual void OnTriggerEnter2D(Collider2D oth) {

		if (oth.tag == "jumpUp") {
			isBreak = true;
		}
		if (oth.tag == "BarrierUp") {
			isBarrier = true;
		}
	}

}
