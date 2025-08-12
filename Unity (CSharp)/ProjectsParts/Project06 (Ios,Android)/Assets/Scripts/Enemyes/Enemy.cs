using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Spine.Unity;
using Random = UnityEngine.Random;

/// <summary>
/// Общий контроллер врагов
/// </summary>
public abstract class Enemy : ExEvent.EventBehaviour, IEnemyDamage, ISpriteOrder {

	[Flags]
	public enum Phase {
		run = 0,
		attack = 1,
		dead = 2,
		wait = 4,
		damage = 8,
		stun = 16
	}

	public event System.Action<Enemy> OnDead;

	private BattlePhase battlePhase;

	public event System.Action<Enemy, GameObject, float> OnDamageEvnt;
	public EnemyType enemyType;                                 // Тип врага
	public Phase phase;                                   // Состояние врага
	public EnemyColliderHelper homeZone;    // Область дамага
	public GameObject shadow;
	public GameObject graphic;
	[HideInInspector]
	public bool isAngry;      // Злой

	public bool firstOnlyReset;
	protected int resetCount = 0;


	protected override void Awake() {
		base.Awake();
		GetConfig();
	}

	protected virtual void Start() { }

	protected virtual void OnEnable() {

		EnemysSpawn.OnBaffSpeed += OnBaffSpead;
		isBaffSpead = false;

		rb = GetComponent<Rigidbody2D>();
		ResetAnimation();
		skeletonAnimation.skeleton.a = 1;

		GetComponent<PolygonCollider2D>().enabled = true;
		InitHealth();

		SetPhase(Phase.run, true);

		directionVelocity = -1;

		if (shadow != null)
			shadow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

		EnemyLiveLine liveLineInst = GetComponentInChildren<EnemyLiveLine>();
		if (liveLineInst != null) {
			Destroy(liveLineInst.gameObject);
		}
		if (isMelafon) StopMelafon();

		damageList.Clear();

		if (damageZone != null) {
			damageZone.TriggerEnter = TargetEnter;
			damageZone.TriggerExit = TargetExit;
			damageZone.TriggerStay = TargetStay;
		}
		if (homeZone != null) {
			homeZone.TriggerEnter = HomeEnter;
			homeZone.TriggerExit = HomeExit;
		}

		//stunTime = -100;
		//isStun = false;
		//DeactiveBlood();
		//DeactiveBurn();
		//DeactiveRain();
		//_rainValue = 1;
		//StopAllCoroutines();
	}

	public virtual void Init() {

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
	protected void OnChangeBattlePhase(ExEvent.BattleEvents.BattlePhaseChange battlePhase) {
		ChangeBattlePhase(battlePhase.phase);
	}

	protected virtual void ChangeBattlePhase(BattlePhase phase) {

		battlePhase = phase;

	}
	public virtual void OnDisable() {
		EnemysSpawn.OnBaffSpeed -= OnBaffSpead;
		if (skeletonAnimation.GetComponent<LightTween>() != null)
			Destroy(skeletonAnimation.GetComponent<LightTween>());

		if (isMelafon) StopMelafon();
		OnBaffSpead(false);

		stunTime = -100;
		isStun = false;
		StopAllCoroutines();
		DeactiveBlood();
		DeactiveBurn();
		DeactiveRain();
		_rainValue = 1;
	}

	public virtual void Update() {
		if (phase == Phase.dead /*|| battlePhase == BattlePhase.end*/) return;

		if (phase == Phase.run) {
			if (isMelafon) MoveWerwoolf();
			Move();
		}

		if (phase == Phase.attack) Attack();

		CheckState();
	}

	protected virtual void CheckPhase() {

		if (isStun && stunDelay == 1) {
			SetPhase(Phase.stun);
			return;
		}
		if (damageList.Count > 0) {
			SetPhase(Phase.attack);
			return;
		}
		if (damageList.Count <= 0) {
			SetPhase(Phase.run);
			return;
		}
	}

	protected virtual void SetRunPhase() {
		SetRunAnimation();
		skeletonAnimation.timeScale = 1f;
	}
	protected virtual void SetDamagePhase() {

		SetDamageAnim();
	}
	protected virtual void SetAttackPhase() { }
	protected virtual void SetDeadPhase() {
		if (healthPanel != null)
			Destroy(healthPanel);
		DeactiveBlood();
		DeactiveBurn();
		Invoke("DeactiveBurn", 2f);
		GetComponent<PolygonCollider2D>().enabled = false;
		StartDeadAnim();
		//Helpers.Invoke(this, Hide, 2);
		if (OnDead != null) {

			OnDead(this);
			OnDead = null;
		}
		OnDamageEvnt = null;
	}
	protected virtual void SetWaitPhase() { }
	protected virtual void SetStunPhase() {
		SetStunAnim();
	}

	protected virtual void CheckState() {
		StopBurn();
		if (isStun) CheckStun();
	}

	protected virtual void CheckStun() {
		if (stunTime < Time.time) {
			isStun = false;
			CheckPhase();
		}
	}

	[HideInInspector]
	public bool isBaffSpead;
	public virtual void OnBaffSpead(bool baffSpeadFlag) {
		isBaffSpead = baffSpeadFlag;
	}

	/// <summary>
	/// Установка нового значения состояния
	/// </summary>
	/// <param name="phase">Новое состояние</param>
	public virtual void SetPhase(Phase phase, bool force = false) {
		if (!force) {
			if (this.phase == Phase.dead) return;
			if (this.phase == phase) return;
		}
    
		switch (phase) {
			case Phase.run:
				SetRunPhase();
				break;
			case Phase.dead:
				SetDeadPhase();
				break;
			case Phase.damage:
				SetDamagePhase();
				break;
			case Phase.wait:
				SetWaitPhase();
				break;
			case Phase.stun:
				SetStunPhase();
				break;
		}

		this.phase = phase;
	}

	#region Аттака
	public bool attackOnStartAnim;            // Атакая по событию hit в анимации
	public EnemyColliderHelper damageZone;    // Область дамага
	protected float attackLastTime;           // Время последней аттаки
	protected float attackWaitTime;           // Время ожидания между аттаками
	protected float attackDamageBase;                   // Базовый урон
	protected float attackDamage { get { return Mathf.Round(attackDamageBase + (level * attackDamageBase * 0.1f)); } }

	public virtual void Attack() {
		if (attackLastTime + attackWaitTime > Time.time) return;

		damageList.RemoveAll(x => x == null);
		if (damageList.Count == 0) return;

		attackLastTime = Time.time;
		SetAttackAnim();
		if (attackOnStartAnim) AttackEvent();

	}

	public virtual void AttackEvent() {

		damageList.RemoveAll(x => x == null);
		if (damageList.Count > 0) {

			var damageObject = damageList[0].GetComponent<IDamage>();

			if (damageObject != null) {

				if (damageObject is IPlayerDamage) {
					var attackObject = damageObject as IPlayerDamage;

					if (attackObject.DamageReady) {
						attackObject.Damage(attackDamage);
						PlayAttackAudio();
					}
				}

				if (isMelafon && damageObject is IEnemyDamage) {
					var attackObject = damageObject as IEnemyDamage;

					if (attackObject.DamageReady) {
						attackObject.Damage(gameObject, attackDamage, 0, 0, GetComponent<Enemy>());
						PlayAttackAudio();
					}
				}

			}



			//switch (LayerMask.LayerToName(damageList[0].layer)) {

			//	case ("Player"):

			//		var attackObject = damageList[0].GetComponent<IPlayerDamage>();

			//		if (attackObject.DamageReady) {
			//			attackObject.Damage(attackDamage);
			//			PlayAttackAudio();
			//		}
			//		break;

			//	//HouseController player = damageList[0].GetComponent<HouseController>();
			//	//if (player != null) {
			//	//	player.Damage(attackDamage);
			//	//	PlayAttackAudio();
			//	//}
			//	//Bear bear = damageList[0].GetComponent<Bear>();
			//	//if (bear != null) {
			//	//	bear.Damage(attackDamage);
			//	//	PlayAttackAudio();
			//	//}
			//	//break;

			//	//case ("Bonuses"):

			//	//	PostController post = damageList[0].GetComponent<PostController>();
			//	//	if (post != null) {
			//	//		post.DamageEnemy(attackDamage);
			//	//		post.destroyThis += SubscribeDestroy;
			//	//		PlayAttackAudio();
			//	//	}

			//	//	BonusController bonus = damageList[0].GetComponent<BonusController>();
			//	//	if (bonus != null) {
			//	//		bonus.DamageEnemy(attackDamage);
			//	//		bonus.destroyThis += SubscribeDestroy;
			//	//		PlayAttackAudio();
			//	//	}
			//	//	break;

			//	case ("Enemy"):

			//		Enemy enemy = damageList[0].GetComponent<Enemy>();
			//		if (enemy != null) {
			//			enemy.Damage(gameObject, attackDamage, 0, 0, GetComponent<Enemy>());
			//			PlayAttackAudio();
			//		}
			//		break;
			//}
		}
	}


	/// <summary>
	/// Подпись на уничтожение атакуемого объекта
	/// </summary>
	protected void SubscribeDestroy(GameObject target) {
		damageList.Remove(target);
	}

	protected virtual void HomeEnter(Collider2D newTarget) {

		var attackObject = newTarget.GetComponent<IPlayerDamage>();

		if (attackObject != null && !isMelafon && attackObject.DamageReady) {
			if (!damageList.Exists(x => x == newTarget.gameObject)) {
				damageList.Add(newTarget.gameObject);
				CheckPhase();
			}
		}

	}

	protected virtual void HomeExit(Collider2D newTarget) {

		var attackObject = newTarget.GetComponent<IPlayerDamage>();

		if (attackObject != null) {
			damageList.Remove(newTarget.gameObject);
		}

	}

	/// <summary>
	/// Регистрация входящих в область действия объектов
	/// </summary>
	/// <param name="newTarget"></param>
	protected virtual void TargetEnter(Collider2D newTarget) {

		if (isMelafon) {
			var enemyAttack = newTarget.GetComponent<Enemy>();
			if (enemyAttack != null) {
				if (!damageList.Exists(x => x == newTarget.gameObject)) {
					damageList.Add(newTarget.gameObject);
					CheckPhase();
				}
			}
		}

		var attackObject = newTarget.GetComponent<IPlayerDamage>();

		if (attackObject != null && !isMelafon && attackObject.DamageReady) {
			if (!damageList.Exists(x => x == newTarget.gameObject)) {
				damageList.Add(newTarget.gameObject);
				CheckPhase();
			}
		}

		CheckPhase();

	}

	/// <summary>
	/// Забываем исходящие из области действия объектов
	/// </summary>
	/// <param name="newTarget"></param>
	protected virtual void TargetExit(Collider2D newTarget) {

		damageList.Remove(newTarget.gameObject);
		CheckPhase();

		if (isMelafon)
			FindEnemyTarget();
	}

	/// <summary>
	/// Забываем исходящие из области действия объектов
	/// </summary>
	/// <param name="newTarget"></param>
	void TargetStay(Collider2D newTarget) {
		if (!isMelafon) return;

		if (LayerMask.LayerToName(newTarget.gameObject.layer) == "Enemy" && !damageList.Exists(x => x == newTarget.GetComponent<Enemy>()))
			damageList.Add(newTarget.gameObject);
	}

	protected virtual void AttackTargetDead(Enemy enemy) {
		damageList.Remove(enemy.gameObject);
	}

	#endregion

	protected int level = 0;                    // Уровень персонажа

	public void SetLevel(int newLevel) {
		level = newLevel;
		InitHealth();
	}

	#region Движение

	protected Rigidbody2D rb;                     // Компонент твердого тела
	public float speedX;                          // Скорость горизонтального движения
	private float _armor;                          // Значение брони
	public float armor { get { return _armor; } }
	protected Vector3 velocity;                   // Вектор движения
	[HideInInspector]
	public float directionVelocity = -1;          // Текущий вектор горизонтального движения
	protected float stunDelay;                   // Степень скоростного штрафа (0-1)
	protected float stunTime;               // Время скоростного штрафа
	public List<GameObject> damageList = new List<GameObject>();        // список атакуемых объектов
																																			/// <summary>
																																			/// Движение
																																			/// </summary>
	public virtual void Move() {
		velocity.x = Mathf.Abs(speedX) * directionVelocity * (isBaffSpead ? 4 : 1) * (_rainWait + 6 > Time.time ? _rainValue : 1);
		if (isStun) velocity.x *= 1 - stunDelay;
		//ransform.position += velocity * Time.deltaTime;
		rb.MovePosition(transform.position + velocity * Time.deltaTime);
	}
	/// <summary>
	/// Изменение направления движения
	/// </summary>
	/// <param name="vector">Направление</param>
	public void SetDirectionVelocity(float vector, bool graphicInvert = true) {
		directionVelocity = vector;

		if (graphicInvert && directionVelocity == 1) {
			RightGraphic();
		}

		if (graphicInvert && (directionVelocity == -1)) {
			LeftGraphic();
		}
	}

	protected void RightGraphic() {
		graphic.transform.localScale = new Vector3(-1f, 1f, 0f);
	}

	protected void LeftGraphic() {
		graphic.transform.localScale = new Vector3(1f, 1f, 0f);
	}

	#endregion

	#region Дождь

	private float _rainWait;
	private Coroutine _rainUse;
	private Coroutine _rainDeacrive;
	private float _rainValue = 1;

	public void SetRain() {
		_rainWait = Time.time + 5;
		_rainValue = -1;

		if (_rainDeacrive != null) StopCoroutine(_rainDeacrive);

		if (_rainUse == null)
			_rainUse = StartCoroutine(RainUse());
	}

	IEnumerator RainUse() {

		while (_rainWait > Time.time) yield return null;
		_rainDeacrive = StartCoroutine(RainDeactive());
	}

	IEnumerator RainDeactive() {
		float startTime = Time.time;
		while (startTime + 6 > Time.time) {
			yield return null;
			_rainValue = -1 + ((Time.time - startTime) / 6) * 2;
		}
		_rainValue = 1;
	}

	private void DeactiveRain() {
		_rainValue = 1;
	}

	#endregion

	#region Мелафон

	// Действием Мелафона

	[HideInInspector]
	public bool isMelafon;

	/// <summary>
	/// Массив врагов на сцене
	/// </summary>
	Enemy[] enemyToScene;
	Enemy targetEnemy = null;

	public virtual void SetMelafon(float timeWorkMode) {
		StartCoroutine(MelafoneUse(timeWorkMode));
	}

	IEnumerator MelafoneUse(float timeWorkMode) {
		StartMelafon();
		FindEnemyTarget();
		yield return new WaitForSeconds(timeWorkMode);
		StopMelafon();
	}

	/// <summary>
	/// Событие смерти целевого объекта
	/// </summary>
	void DieTargetMagicObject(Enemy enemy) {
		if (isMelafon) Invoke("FindEnemyTarget", 0.5f);
	}

	public virtual void StartMelafon() {
		isMelafon = true;
		damageList.Clear();
		SetPhase(Phase.run);
	}
	public virtual void StopMelafon() {
		//деактивировать мозг
		isMelafon = false;
		//enemyPhase = EnemyPhases.run;
		SetDirectionVelocity(-1);
		damageList.Clear();
		SetPhase(Phase.run);
		SetRunAnimation();
	}

	/// <summary>
	/// Искать жертву
	/// </summary>
	/// <param name="rightOnly">Выполнять поиск только справа</param>
	public void FindEnemyTarget(bool rightOnly = true) {

		enemyToScene = EnemysSpawn.GetAllEnemy;
		float? distanse = null;
		/// Ищем ближайшего 
		for (int i = 0; i < enemyToScene.Length; i++) {
			if (enemyToScene[i].transform.position.x < CameraController.rightPoint.x
					&& enemyToScene[i] != this
					&& (!rightOnly || enemyToScene[i].transform.position.x >= transform.position.x)
					&& !enemyToScene[i].isMelafon) {

				if (distanse == null || distanse > Vector3.Distance(transform.position, enemyToScene[i].transform.position)) {
					distanse = Vector3.Distance(transform.position, enemyToScene[i].transform.position);
					targetEnemy = enemyToScene[i];
				}
			}
		}
		/*
    if(targetEnemy == null) {
      if (rightOnly) return;
      // РЕкурсивный вызов но с общим поиском
      FindEnemyTarget(false);
    }
    */


		/// Подписывыемся на смерть объекта, на случай если не успеем дойти
		if (targetEnemy != null) {
			targetEnemy.SubscribeDead(DieTargetMagicObject);
			SetDirectionVelocity(1);
			if (distanse <= 0.9f) EnemyContact(targetEnemy);
		}
		if (targetEnemy == null) {
			Invoke("FindEnemyTarget", 1);
		}
	}

	protected virtual void EnemyContact(Enemy enemy) {
		SetPhase(Phase.attack);
		damageList.Add(enemy.gameObject);
		enemy.SubscribeDead(AttackTargetDead);

	}

	public void MoveWerwoolf() {
		if (targetEnemy != null && Mathf.Abs(transform.position.x - targetEnemy.transform.position.x) < 0.4f) return;
		if (targetEnemy != null && transform.position.x > targetEnemy.transform.position.x && directionVelocity == 1)
			SetDirectionVelocity(-1);
		if (targetEnemy != null && transform.position.x < targetEnemy.transform.position.x && directionVelocity == -1)
			SetDirectionVelocity(1);
	}

	#endregion

	#region Здоровье

	float[] healthLevel = { 0.5f, 1f, 1.5f, 2f };

	float baseLive;             // Стартовое значение эизней
	public float startLive {
		get {
			return Mathf.Round((baseLive * healthLevel[Game.User.UserManager.Instance.difficultyLevel.Value]) + (level * 0.1f * baseLive));
		}
	}

	//[HideInInspector]
	public float liveNow;               // Текущее значение жизней

	/// <summary>
	/// Начальная инициализация жизней
	/// </summary>
	void InitHealth() {
		liveNow = startLive;
		//healthPanel.SetActive(false);
	}

	/// <summary>
	/// Инкремент жизней
	/// </summary>
	/// <param name="incValue">Изменяемое значение</param>
	public void HealthIncrement(float incValue) {
		liveNow = Mathf.Max(startLive, liveNow + incValue);
		ChangeHealthPanel();
	}

	/// <summary>
	/// Бэк панели состояния здоровья
	/// </summary>
	protected GameObject healthPanel;
	public Transform healthPanelPosition;

	/// <summary>
	/// Изменение текущее состояние панели здоровья
	/// </summary>
	protected void ChangeHealthPanel() {
		if (phase == Phase.dead) return;
		//От генератора получаем объект полосы жизней и позиционируем его
		if (healthPanel == null) {
			healthPanel = EnemysSpawn.Instance.GetEnemyLiveLine();
			healthPanel.transform.SetParent(transform);
			//healthPanel.transform.parent = transform;
			healthPanel.transform.localScale = Vector3.one;
			healthPanel.transform.localPosition = healthPanelPosition.localPosition;

		}

		healthPanel.SetActive(true);

		healthPanel.GetComponent<EnemyLiveLine>().SetValue(this);

	}

	#endregion

	#region Настройки

	protected Configuration.Enemy enemy;
	/// <summary>
	/// Запрос на получение настроек
	/// </summary>
	protected virtual void GetConfig() {

		enemy = GameDesign.Instance.allConfig.enemy.Find(x => x.id == (int)enemyType);


		//List<object> allConfig = (List<object>)GameDesign.Instance.allConfig["enemy"];

		//foreach (object oneEnem in allConfig) {
		//	if (((Dictionary<string, object>)oneEnem)["ID"].ToString() == ((int)enemyType).ToString()) {
		//		xmlConfig = (Dictionary<string, object>)oneEnem;
		//	}
		//}

		speedX = (float)enemy.speed;
		baseLive = (float)enemy.health;
		_armor = (float)enemy.armor;
		attackWaitTime = (float)enemy.cooldown;
		attackDamageBase = (float)enemy.damage;
	}

	#endregion

	#region Урон

	[HideInInspector]
	public float lastDamage;
	protected bool isStun;

	public bool IsMelafon { get { return isMelafon; } }

	public bool DamageReady { get { return true; } }

	public bool playerContact {
		get {
			foreach (var VARIABLE in damageList) {
				if (VARIABLE.GetComponent<IPlayerDamage>() != null)
					return true;
			}
			return false;
		}
	}

	public virtual bool Damage(GameObject damager, float value, float newSpeedDelay = 0, float newTimeSpeedDelay = 0, Component parent = null) {

		value = Mathf.Round(value);

		if (parent != null && parent.GetComponent<Enemy>() != null) EnemyContact(parent.GetComponent<Enemy>());

		if (isShield) {
			ChangeHealthPanel();
			return false;
		}
		if (phase == Phase.dead) return false;

		value = value - _armor;

		if (OnDamageEvnt != null) OnDamageEvnt(this, damager, Mathf.Min(Mathf.Max(0, value), liveNow));

		value = Mathf.Max(0, value);

		ShowDamageValue(value);

		lastDamage = Mathf.Min(liveNow, value); ;

		liveNow = liveNow - value;
		if (liveNow <= 0) {

			try {

			  Game.Weapon.Bullet damagerBullet = damager.GetComponent<Game.Weapon.Bullet>();

				if (!string.IsNullOrEmpty(damagerBullet.killEnemySound)) {
					GameObject inst = PoolerManager.Spawn("bloodBazookaEffect");
					inst.transform.position = transform.position + Vector3.up * 0.5f;
					inst.transform.localEulerAngles = new Vector3(-90 + Random.Range(-45f, 45f), 90, -90);
					inst.SetActive(true);
					DarkTonic.MasterAudio.MasterAudio.PlaySound(damagerBullet.killEnemySound);
				} else
					PlayDeadAudio();

			} catch { }

			SetPhase(Phase.dead);
			return true;
		} else {
			ChangeHealthPanel();
			//if (value <= 0) {
			//	SetStun(newSpeedDelay, newTimeSpeedDelay);
			//	return false;
			//}
			GetDamage(newSpeedDelay, newTimeSpeedDelay);
			//OnDamage();
		}
		return false;
	}

	void ShowDamageValue(float value) {

		GameObject inst = PoolerManager.Spawn("DamageValue");
		inst.transform.position = transform.position + Vector3.up;
		inst.SetActive(true);
		inst.GetComponent<DamageValueVisual>().SetValue(value);
	}

	protected virtual void StartDeadAnim() {
		SetAnimation(deadAnim, false);
	}

	protected virtual void GetDamage(float stunDelay = 0, float stunTime = 0) {
		SetStun(stunDelay, stunTime);
		SetPhase(Phase.damage);
	}


	/// <summary>
	/// Получение повреждения
	/// </summary>
	/// <param name="value">Сила дамага</param>
	/// <param name="newSpeedDelay">Степень задержки скорости (0 - нет, 1 - 100%(не двигается))</param>
	/// <param name="newTimeSpeedDelay">Время, на которое выполняется задержка</param>
	/// <returns></returns>
	public virtual void SetStun(float stunDelay = 0, float stunTime = 0) {
		if (stunDelay == 0 || stunTime == 0) return;
		if (stunnAnim != "" && damageAnim == "") { SetAnimation(stunnAnim, true); }

		isStun = true;
		this.stunDelay = stunDelay;
		this.stunTime = Time.time + stunTime;
	}

	protected virtual void OnDamage() {
		if (!isStun) {
			if (IsInvoking("ReplacePhase")) CancelInvoke("ReplacePhase");
			Invoke("ReplacePhase", 0);
		}
	}



	/// <summary>
	/// Окончание получения урона
	/// </summary>
	protected virtual void EndDamage() {

		CheckPhase();
		//enemyPhase = EnemyPhases.run;
	}

	/// <summary>
	/// Плавное сокрытие с отключением
	/// </summary>
	protected void Hide() {
		//if (isWerwoolf) Destroy(brainIcon);
		LightTween.SpriteColorTo(skeletonAnimation, new Color(1, 1, 1, 0), 2f, 0, LightTween.EaseType.linear, gameObject, DeactiveEnemy);
		DeactiveShadow();
	}

	protected virtual void DeactiveShadow() {
		if (shadow != null)
			LightTween.SpriteColorTo(shadow.GetComponent<SpriteRenderer>(), new Color(1, 1, 1, 0), 1.8f, 0, LightTween.EaseType.linear, gameObject, null);
	}
	/// <summary>
	/// Отключение врага
	/// </summary>
	public void DeactiveEnemy() {
		isStun = false;
		//healthPanel.SetActive(false);
		if (skeletonAnimation.GetComponent<LightTween>() != null)
			DestroyImmediate(skeletonAnimation.GetComponent<LightTween>());
		Helpers.Invoke(this, DeactiveNow, 0.2f);
	}

	void DeactiveNow() {
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Подпись на смерть
	/// </summary>
	/// <param name="dead">Делега подписывающийся на смерть</param>
	public void SubscribeDead(System.Action<Enemy> dead) {
		OnDead += dead;
	}

	#endregion

	#region Кровотечение

	[HideInInspector]
	public GameObject bloodInst;
	private Coroutine _bleedingCoroutine;
	private float _bleedingPeriod;
	private float _bleedingTimeEnd;
	private float _bleedingDamage;
	public virtual void SetBleeding(float timeWork, float periodBlood, float damageBlood) {
		if (phase == Phase.dead) return;

		_bleedingPeriod = periodBlood;
		_bleedingTimeEnd = Time.time + timeWork;
		_bleedingDamage = damageBlood;

		if (_bleedingCoroutine == null)
			_bleedingCoroutine = StartCoroutine(Blood());
	}

	private IEnumerator Blood() {

		StartBlood();

		while (_bleedingTimeEnd > Time.time) {
			yield return new WaitForSeconds(_bleedingPeriod);
			Damage(gameObject, _bleedingDamage);
		}

		DeactiveBlood();
	}

	private void StartBlood() {

		if (bloodInst == null)
			bloodInst = PoolerManager.Spawn("EnemyBlood");
		bloodInst.transform.position = transform.position + Vector3.up;
		bloodInst.transform.parent = transform;
		bloodInst.SetActive(true);
	}

	public void DeactiveBlood() {
		if (_bleedingCoroutine != null) StopCoroutine(_bleedingCoroutine);
		if (bloodInst == null) return;
		//bloodInst.transform.parent = PoolerManager.instance.transform;
		bloodInst.SetActive(false);
	}

	#endregion

	#region Горение

	[HideInInspector]
	public GameObject flameInst;

	public void SetBurn(float timeWorkFlameOfFire, float periodWorkFlameOfFire, float newDamageFlameOfFire) {
	}

	private IEnumerator Burn(float timeWorkFlameOfFire, float periodDamageBurn, float damageBurn) {

		StartBurn();

		float timeStart = Time.time;
		while (timeStart + timeWorkFlameOfFire > Time.time) {
			yield return new WaitForSeconds(periodDamageBurn);
			Damage(gameObject, damageBurn);
		}

		StopBurn();
	}

	private void StartBurn() {
		if (flameInst == null)
			flameInst = PoolerManager.Spawn("MolotovFlame");

		flameInst.transform.position = transform.position;
		flameInst.transform.parent = transform;
		flameInst.SetActive(true);
	}

	public void StopBurn() {
		DeactiveBurn();
	}

	public void DeactiveBurn() {
		if (flameInst == null) return;
		//flameInst.transform.parent = PoolerManager.instance.transform;
		flameInst.SetActive(false);
		flameInst = null;
	}

	#endregion

	#region Щит
	[HideInInspector]
	public bool isShield;
	public void SetShield(float timeUse) {
		StartCoroutine(Shield(timeUse));
	}
	private IEnumerator Shield(float timeUse) {
		isShield = true;
		yield return new WaitForSeconds(timeUse);
		isShield = false;
	}

	#endregion

	#region Animation

	public SkeletonAnimation skeletonAnimation;         // Ссылка на скелетную анимацию
	protected string currentAnimation;                  // Текущая анимция

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runAnim = "";     // Анимация хотьбы
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string damageAnim = ""; // Анимация damage
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string deadAnim = ""; // Анимация смерти
	[SpineAnimation(dataField: "skeletonAnimation")]
	public List<string> attackAnim; // Анимация атаки
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string stunnAnim; // Анимация стана


	public int spriteOrderValue { get; set; }
	public void SetSpriteOrder(int order) {
		spriteOrderValue = order;
		if (skeletonAnimation != null)
			skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = order;
	}

	public void ClearTrack(int index) {
		currentAnimation = null;
		skeletonAnimation.state.ClearTrack(index);
	}
	/// <summary>
	/// Установка основной анимации
	/// </summary>
	/// <param name="anim">Имя анимации</param>
	/// <param name="loop">Флаг зацикливания</param>
	public virtual void SetAnimation(string anim, bool loop, bool reset = false) {

		if (reset) ResetAnimation();
		if (skeletonAnimation != null) SetAnimation(0, anim, loop);
	}
	public virtual void SetAnimation(int index, string animName, bool loop) {
		if (currentAnimation != animName || !loop) {

			//Debug.Log(animName + " : " + loop);

			currentAnimation = animName;
			skeletonAnimation.state.SetAnimation(index, animName, loop);
		}
	}
	/// <summary>
	/// Повторная инициализация анимации
	/// </summary>
	public virtual void ResetAnimation() {
		if (firstOnlyReset && resetCount > 0) return;
		resetCount++;
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
	/// <summary>
	/// Отписываемся от события анимации
	/// </summary>
	public void DeleteEventAnimation() {
		skeletonAnimation.state.Event -= AnimEvent;
		skeletonAnimation.state.End -= AnimEnd;
		skeletonAnimation.state.Complete -= AnimComplited;
		skeletonAnimation.state.Start -= AnimStart;
		skeletonAnimation.state.Dispose -= AnimDispose;
		skeletonAnimation.state.Interrupt -= AnimInterrupt;
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
	protected virtual void OnRebild(SkeletonRenderer skeletonRenderer) { }
	/// <summary>
	/// Событие анимации спайна
	/// </summary>
	/// <param name="state">Имя</param>
	/// <param name="trackIndex">Слой</param>
	/// <param name="e">Собятие</param>
	public virtual void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) {
		if (e.Data.Name == "hit") AttackEvent();
	}
	public virtual void AnimStart(Spine.TrackEntry trackEntry) { }
	public virtual void AnimDispose(Spine.TrackEntry trackEntry) { }
	public virtual void AnimInterrupt(Spine.TrackEntry trackEntry) { }
	/// <summary>
	/// Событие окончание анимации спайна
	/// </summary>
	/// <param name="state">Название анимации</param>
	/// <param name="trackIndex">Номер трека</param>
	public virtual void AnimEnd(Spine.TrackEntry trackEntry) { }
	public virtual void AnimComplited(Spine.TrackEntry trackEntry) {

		if (trackEntry.ToString() == deadAnim) {
			Hide();
		}

		if (trackEntry.ToString() == damageAnim && phase != Phase.dead) {
			EndDamage();
		}

		if (attackAnim.Exists(x => x == trackEntry.ToString())) CheckPhase();
	}

	public virtual void SetRunAnimation() {
		if (runAnim != null) SetAnimation(runAnim, true);
	}
	protected virtual void SetDamageAnim() {

		if (damageAnim != "") SetAnimation(damageAnim, false, false);
	}
	protected virtual void SetStunAnim() {
		if (stunnAnim != "") SetAnimation(stunnAnim, true, false);
	}
	protected virtual void SetAttackAnim() {
		SetAnimation(attackAnim[Random.Range(0, attackAnim.Count)], false, false);
	}
	#endregion

	#region Звуки

	public AudioBlock attackAudioBlock;
	protected virtual void PlayAttackAudio() {
		attackAudioBlock.PlayRandom(this);
	}
	public AudioBlock deadAudioBlock;
	protected virtual void PlayDeadAudio() {
		deadAudioBlock.PlayRandom(this);
	}

	#endregion

}
