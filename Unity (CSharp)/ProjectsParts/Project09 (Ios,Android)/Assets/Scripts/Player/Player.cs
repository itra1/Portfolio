using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;

/// <summary>
/// Игрок, главный персонаж
/// </summary>
public class Player : MonoBehaviour {

	public event Actione<Player> OnChangePlayer;

	private int _team;                                          // Номер команды
	public int team { get { return _team; } set { _team = value; } }
	string _userId;                                             // Идентификатор пользователя
	public string userId { get { return _userId; } set { _userId = value; } }
	int _sceneId;                                               // Норме на сцене
	public int sceneId { get { return _sceneId; } set { _sceneId = value; } }
	string _playerName;                                         // Имя игрока
	public string playerName { get { return _playerName; } set { _playerName = value; } }
	public float moveSpeed;                                     // Скорость бега

	protected Rigidbody2D rb;                                   // Компонент твердого тела
	public bool isFirst;                                        // Первый игрок
	public GameObject mainGraphic;                              // Ссылка на основной компонент графики
	protected Vector3 diffPosition;                             // Смещение персонажа, используется для рассчета перемещения
	protected Vector3 newPosition;                              // Новая позиция игрока, куда выполняется перемещение игрока
	[HideInInspector]
	public float heightPlayer;                                  // Высота плеера
	[HideInInspector]
	public float widthPlayer;                                   // Ширина плеера
	public WeaponManager weaponManager;                         // Менеджер используемого оружия
	protected PlayerScenePosition scenePosition;                // Используемая часть сцены
	protected Vector3 shootPoint;                               // Точка прицеливания

	public Transform centrPlayer;                               // Центр плеера
	protected BattlePhase battlePhase;                          // Фаза сцены

	public Transform shootPointStart;                           // Точка выстрела
	public Transform handPointStatic;                           // Точка руки

	public bool isMove;

	protected void Start() { }

	protected void OnEnable() {
		BattleScene.ChangeBattlePhase += OnChangeBattlePhase;
		PlayEffects.OnEffect += OnEffect;
		ClientBroadcast.OnBroadcast += OnClientBroadcast;

		if(BattleScene.instance != null) battlePhase = BattleScene.instance.battlePhase;
		rb = GetComponent<Rigidbody2D>();
		ResetAnimation();
	}

	protected void OnDisable() {
		BattleScene.ChangeBattlePhase -= OnChangeBattlePhase;
		PlayEffects.OnEffect -= OnEffect;
		ClientBroadcast.OnBroadcast -= OnClientBroadcast;
	}

	protected void OnDestroy() { }
	protected void Update() {
		if(BattleScene.instance != null && BattleScene.instance.battlePhase != BattlePhase.battle) return;
		EnergyUpdate();
		Move(Time.deltaTime);
		Shoot();
		if(isLoad) CheckLoad();
		if(isHidden) CheckHidden();
		if(_isSpeedyShooter) CheckSpeedyShooter();
	}

	void OnChangeBattlePhase(BattlePhase newBattlePhase) {
		battlePhase = newBattlePhase;
	}

	/// <summary>
	/// Первоначальная инициализация персонажа
	/// </summary>
	/// <param name="newName"></param>
	public void Init() {
		InitLive();
		InitEnergy();
	}
	/// <summary>
	/// Установка ориентации плеера
	/// </summary>
	/// <param name="newPosition"></param>
	public void SetScenePosition(PlayerScenePosition newPosition) {

		scenePosition = newPosition;

		if(newPosition == PlayerScenePosition.LEFT)
			mainGraphic.transform.localScale = new Vector3(1, 1, 1);

		if(newPosition == PlayerScenePosition.RIGHT)
			mainGraphic.transform.localScale = new Vector3(-1, 1, 1);
	}

	#region Обработка движения
	protected Vector3 velocity;                                 // Вектор движения
	protected Vector3 lastPosition;                             // Положение плеера до соверщения движения в кадре
	protected bool moveReady = true;                            // Разрешение выполнения движения
	protected Vector3 sendPosition;                             // Отправленная позиция
	protected float positionLastChange;                         // Последняя измененная позиция
	private float checkDist;
	private float speedKoef;
	protected Vector3 targetPosition = Vector3.zero;             // Установка позиции
	protected MoveObjecsElem moveObject;
	Vector2 targetVelocity;
	/// <summary>
	/// Обработка смещения
	/// </summary>
	protected void Move() {
		if(targetPosition == Vector3.zero || targetPosition == transform.position) return;

		checkDist = (targetVelocity != Vector2.zero ? targetVelocity.magnitude : moveSpeed);
		speedKoef = checkDist / moveSpeed;

		if(Vector3.Distance(targetPosition, transform.position) > checkDist * Time.deltaTime) {
			if(isFirst) SetAnimation(runSpineAnimation, true);
			SetPosition(transform.position + (Vector3)((targetPosition - transform.position).normalized * moveSpeed * speedKoef * Time.deltaTime));
		} else {
			if(isFirst && !isMove) SetAnimation(staySpineAnimation, true);
			SetPosition(targetPosition);
		}

		if(transform.position == targetPosition) {
			moveReady = true;
		}
	}

	protected void Move(float speedDeltaTime) {
		if(targetPosition == Vector3.zero || targetPosition == transform.position) {
			SetAnimation(staySpineAnimation, true);
			return;
		}

		// Плавное изменения направления движения (измегание дерганий при резкой смене)
		SmoothVelocity(speedDeltaTime);

		Vector3 pos = transform.position;
		if(targetPosition.x != 0 || targetPosition.y != 0) {
			SetAnimation(runSpineAnimation, true);
			// Симуляция серверного движения
			targetPosition.x += targetVelocity.x * speedDeltaTime;
			targetPosition.y += targetVelocity.y * speedDeltaTime;

			// коррекция скорости основанная на разнице позиций, для плавного сближения
			// чем больше разница, тем выше скорость
			pos = Vector3.Lerp(pos, targetPosition, speedDeltaTime * 2);
			velocity.x -= (pos.x - targetPosition.x) * 1.3f;
			velocity.y -= (pos.y - targetPosition.y) * 1.3f;
		}
		pos = transform.position + velocity * speedDeltaTime;

		// Остановка персонажа
		if(targetVelocity.x == 0 && targetVelocity.y == 0) {
			Vector3 diff = targetPosition-pos;
      float diffDistance = diff.magnitude;

			if(diffDistance< 0.1f) {
        targetPosition = pos;
				//pos = targetPosition;
				velocity = Vector3.zero;
				SetAnimation(staySpineAnimation, true);
			} else {
				//SetAnimation(runSpineAnimation, true);
        float distanceThisFrame = moveSpeed * speedDeltaTime;
        diff = (diffDistance < distanceThisFrame) ? diff : diff.normalized*moveSpeed*speedDeltaTime; 
				pos = transform.position + diff;
			}
		}

		SetPosition(pos);
		if(transform.position == targetPosition) {
			moveReady = true;
		}
	}

	void SmoothVelocity(float deltaTime) {
		float speedUp = 100;
		float distance = Vector2.Distance(velocity, targetVelocity);
		float sin = (targetVelocity.x - velocity.x) / distance;
		float cos = (targetVelocity.y - velocity.y) / distance;

		speedUp *= deltaTime;
		if(speedUp > distance) {
			velocity = targetVelocity;
		} else {
			velocity.x += speedUp * sin;
			velocity.y += speedUp * cos;
		}
	}

	/// <summary>
	/// Пакет с сервера о движении
	/// </summary>
	/// <param name="moveObject"></param>
	public void OnMoveChange(MoveObjecsElem moveObject) {
		if(moveObject.id == this.sceneId) {
			this.moveObject = moveObject;
			targetPosition = moveObject.position;
			targetVelocity = moveObject.velocity;
		}
	}

	/// <summary>
	/// Отправка позиционирования
	/// </summary>
	/// <param name="newPosition">Новая позиция</param>
	public void SetPosition(Vector3 newPosition) {
		lastPosition = transform.position;
		//rb.MovePosition(newPosition);
		transform.position = newPosition;
		positionLastChange = Time.time;
	}

	#endregion

	#region Жизни
	[HideInInspector]
	public float healthMax;                             // Максимальный/стартовый запас жизней
	[HideInInspector]
	public float healthValue;                           // Текущий запас жизней

	/// <summary>
	/// Инифиализация значений жизней
	/// </summary>
	/// <param name="newHealthMax"></param>
	private void InitLive() {
		healthValue = healthMax;
	}

	public List<AudioClip> damageSound;

	public void SetDamage(HealthUpdate.HealthData healhData) {
		healthMax = healhData.healthMax;
		healthValue = healhData.currentHealth;

		GameObject damageVisual = PoolerManager.GetPooledObject("DamageVisual");
		damageVisual.transform.position = (Vector2)transform.position + Vector2.up;
		damageVisual.SetActive(true);
		damageVisual.GetComponent<DamageVisual>().SetText(healhData.damage.ToString());

		GameObject damageHit = PoolerManager.GetPooledObject("PlayerHit");
		damageHit.transform.position = (Vector2)transform.position;
		damageHit.SetActive(true);

		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().clip = damageSound[Random.Range(0, damageSound.Count)];
			GetComponent<AudioSource>().Play();
		}

		ChangeHealth();
	}

	/// <summary>
	/// Изменение оружия
	/// </summary>
	public void ChangeHealth() {
		if(OnChangePlayer != null) OnChangePlayer(this);
	}

	#endregion

	#region Исчезновение

	protected float hiddenTime;         // Время работы исчезновения
	[HideInInspector]
	public bool isHidden;               // Флаг исчезновения

	/// <summary>
	/// Применение невидимости
	/// </summary>
	public void SetHidden() {
		hiddenTime = Time.time + 5;
		isHidden = true;
		if(!isFirst) skeletonAnimation.GetComponent<MeshRenderer>().enabled = false;
		VisibleChangePlayer();
	}

	public void VisibleChangePlayer() {
		skeletonAnimation.skeleton.Slots.ForEach(x => x.a = (isHidden ? (isFirst ? .5f : 0) : 1));
		if(weaponManager != null) weaponManager.SetHidden(isHidden, isFirst);
		CheckHand();
		if(weaponManager != null) DeactiveBone();
	}

	/// <summary>
	/// Проверка исчезновения
	/// </summary>
	protected void CheckHidden() {
		if(hiddenTime <= Time.time) {
			isHidden = false;
			if(!isFirst) skeletonAnimation.GetComponent<MeshRenderer>().enabled = true;
			ResetAnimation();
			VisibleChangePlayer();
		}
	}

	#endregion

	#region Ускоренная стрельба

	protected float speedyShooterTime;          // Время действия ускоренно стрельбы
	protected bool _isSpeedyShooter;            // Действует ускоренная стрельба
	public bool isSpeedyShooter { get { return _isSpeedyShooter; } }

	/// <summary>
	/// Включаем режим ускоренной стрельбы
	/// </summary>
	/// <param name="timeSpeedyShooter"></param>
	public void SetSpeedyShooter() {
		speedyShooterTime = Time.time + 20;
		ChangeWeapon(true);
	}
	/// <summary>
	/// Проверка окончания времени скоростной стрельбы
	/// </summary>
	protected void CheckSpeedyShooter() {
		if(speedyShooterTime <= Time.time)
			ChangeWeapon(false);
	}
	/// <summary>
	/// Событие изменение оружия
	/// </summary>
	/// <param name="flag"></param>
	void ChangeWeapon(bool flag) {
		_isSpeedyShooter = flag;
		WeaponManager.koeffReload = (_isSpeedyShooter ? 0.7f : 1);
		weaponManager.ChangeWeapon();
	}

	#endregion

	#region Оружие
	protected WeaponError weaponError;                                      // Ошибки, возвращаемые оружием
	[HideInInspector]
	public bool isShooting;                                                 // Флаг выполнения стрельбы

	/// <summary>
	/// Установка нового оружия
	/// </summary>
	/// <param name="newWeapon"></param>
	public void SetNewWeapon(WeaponType newType) {
		if(weaponManager != null) weaponManager.Deactive();
		weaponManager = WeaponSpawner.instance.GetPlayerWeapon(isFirst, newType);
		weaponManager.Init(this);
		weaponManager.OnChange += OnShooting;
		weaponManager.checkShootTapOn = true;

		VisibleChangePlayer();

		if(OnChangePlayer != null) OnChangePlayer(this);
	}

	/// <summary>
	/// Отпключение кости под оружие
	/// </summary>
	void DeactiveBone() {
		ExposedList<Slot> slotList = skeletonAnimation.skeleton.slots;
		slotList.Find(x => x.bone.Data.name == boneForWeapon).SetColor(new Color(0, 0, 0, 0));
	}

	/// <summary>
	/// Событие выстрела
	/// </summary>
	/// <param name="weaponType"></param>
	private void OnShooting(WeaponManager weaponManager) {
		if(OnChangePlayer != null) OnChangePlayer(this);
	}

	bool shootReady;

	/// <summary>
	/// Выполняется аттака
	/// </summary>
	public void Shoot() {
		if(isFirst && isShooting && weaponManager != null)
			weaponManager.StartSendShoot(shootPoint);
	}

	/// <summary>
	/// Событие выстрела
	/// </summary>
	public void OnShoot(ObjectSpawn bulletData) {
		if(weaponManager != null) weaponManager.Shoot(bulletData);
	}

	#endregion

	#region Энергия
	[HideInInspector]
	public float energyMax;                                         // Максимальный/стартовый запас энергии
	[HideInInspector]
	public float energyValue;                                       // Текущий запас энергии
	[HideInInspector]
	public float energyRepeat;                                      // Скорость восстановления энергии
																																	/// <summary>
																																	/// Первоначальная инициализация энергии
																																	/// </summary>
	void InitEnergy() {
		energyValue = energyMax;
	}
	/// <summary>
	/// Возобновление энергии
	/// </summary>
	void EnergyUpdate() {
		if(energyValue >= energyMax) return;
		energyValue += energyRepeat * Time.deltaTime;
		energyValue = Mathf.Min(energyValue, energyMax);
		if(OnChangePlayer != null) OnChangePlayer(this);
	}
	/// <summary>
	/// Изменение скорости восстановления энергии
	/// </summary>
	/// <param name="newEnergyRepeat"></param>
	public void ChangeEnegryRepeat(float newEnergyRepeat) {
		energyRepeat = newEnergyRepeat;
	}
	public void EnergyChange(EnergyUpdate.EnergyData energyData) {
		this.energyMax = energyData.maxEnergy;
		this.energyRepeat = energyData.recoverySpeed;
		this.energyValue = energyData.currentEnergy;
		if(OnChangePlayer != null) OnChangePlayer(this);
	}
	#endregion

	#region Animation
	public AtlasAsset atlas;
	public SkeletonAnimation skeletonAnimation;           // Ссылка на спайн анимацию
	string currentAnimation;                              // Текущая анимация
	bool isAlterStopAnim;                                 // Активна анимация простоя

	[SpineBone(dataField: "skeletonRenderer")]
	public string boneForWeapon;                          // Кость, к которой привязывается оружие

	[SpineAnimation(dataField: "skeletonAnimation")]
	public string runSpineAnimation = "";                 // Анимация бега
	[SpineAnimation(dataField: "skeletonAnimation")]
	public string staySpineAnimation = "";                // Анимация простоя
	[SpineAnimation(dataField: "skeletonAnimation")]
	public List<string> stayAlterSpineAnimation;              // Анимация альтернативных действий во время простоя
																														/// <summary>
																														/// Установка основной анимации
																														/// </summary>
																														/// <param name="anim">Название анимации</param>
																														/// <param name="loop">Циклы</param>
	public void SetAnimation(string anim, bool loop) {
		if(!gameObject) return;

		// Если запущена альтернативная анимация, анимацию простоя не запускаем
		if(isAlterStopAnim && anim == staySpineAnimation) return;

		// При запуске анимации бега во время альтернативной анимации, выполняем сброс
		if(isAlterStopAnim && anim == runSpineAnimation) {
			isAlterStopAnim = false;
			ResetAnimation();
		}

		// Устанавливаем анимацию
		if(currentAnimation != anim) {
			skeletonAnimation.state.SetAnimation(0, anim, loop);
			currentAnimation = anim;
		}
	}

	/// <summary>
	/// Сброс анимации
	/// </summary>
	public void ResetAnimation() {
		skeletonAnimation.Initialize(true);
		SubscribeAnimEvents();
		currentAnimation = null;
		VisibleChangePlayer();
	}

	#region Работа с рукой

	[SpineBone(dataField="skeletonAnimation")]
	public string slotHand;                          // Кость, к которой привязывается оружие
	[SpineAtlasRegion(atlasAssetField="atlas")]
	public string slotHandWeapon;                          // Кость, к которой привязывается оружие
	[SpineAtlasRegion(atlasAssetField="atlas")]
	public string slotOpenWeapon;                          // Кость, к которой привязывается оружие

	void CheckHand() {
		Atlas at = atlas.GetAtlas();
		float scale = skeletonAnimation.skeletonDataAsset.scale;
		Slot slot = skeletonAnimation.skeleton.slots.Find(x => x.bone.Data.name == boneForWeapon);
		var region = at.FindRegion(slotOpenWeapon);
		slot.attachment = region.ToRegionAttachment(slotOpenWeapon, scale);
	}

	#endregion

	/// <summary>
	/// Подписываемся на события анимации
	/// </summary>
	protected void SubscribeAnimEvents() {
		skeletonAnimation.state.Start += AnimStart;
		skeletonAnimation.state.Event += AnimEvent;
		skeletonAnimation.state.Complete += AnimComplete;
		skeletonAnimation.state.End += AnimEnd;
		skeletonAnimation.state.Interrupt += AnimInterrupt;
		skeletonAnimation.state.Dispose += AnimDispose;
	}

	/// <summary>
	/// Наложение анимации
	/// </summary>
	/// <param name="index">Номар слоя</param>
	/// <param name="animName">Название анимации</param>
	/// <param name="loop">Зациклено</param>
	/// <param name="delay">Задержка запуска</param>
	public void AddAnimation(int index, string animName, bool loop, float delay) {
		skeletonAnimation.state.AddAnimation(index, animName, loop, delay);
	}

	/// <summary>
	/// Привызяка к событию анимации
	/// </summary>
	/// <param name="state"></param>
	/// <param name="trackIndex"></param>
	/// <param name="e"></param>
	void AnimEvent(Spine.TrackEntry trackEntry, Spine.Event e) { }

	/// <summary>
	/// Привязка к окончанию анимации
	/// </summary>
	/// <param name="state"></param>
	/// <param name="trackIndex"></param>
	void AnimEnd(Spine.TrackEntry trackEntry) { }

	/// <summary>
	/// Событие выполнения анимации при зацикленном воспроизведении
	/// </summary>
	/// <param name="state"></param>
	/// <param name="trackIndex"></param>
	/// <param name="loopCount"></param>
	void AnimComplete(Spine.TrackEntry trackEntry) {

		if(stayAlterSpineAnimation.Exists(x => x == trackEntry.ToString()))
			ResetAnimation();

		// Отключаем флаг выполнения альтернативной анимации
		if(isAlterStopAnim && trackEntry.ToString() != staySpineAnimation)
			isAlterStopAnim = false;

		if(!isAlterStopAnim && trackEntry.ToString() == staySpineAnimation && !isShooting)
			if(Random.value <= 0.3f) StartAlterAnim();

	}
	void AnimInterrupt(Spine.TrackEntry trackEntry) { }
	void AnimDispose(Spine.TrackEntry trackEntry) { }
	void AnimStart(Spine.TrackEntry trackEntry) { }
	/// <summary>
	/// Запуск альтернативной анимации простоя
	/// </summary>
	void StartAlterAnim() {
		SetAnimation(stayAlterSpineAnimation[Random.Range(0, stayAlterSpineAnimation.Count)], false);
		isAlterStopAnim = true;
	}
	#endregion

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector2(widthPlayer, heightPlayer));
		Gizmos.DrawLine(transform.position, shootPoint);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, targetPosition);
		Gizmos.DrawWireCube(targetPosition, new Vector2(widthPlayer, heightPlayer));
	}

	#region Входные сигналы управлени
	/// <summary>
	/// Окончание атаки
	/// </summary>
	/// <param name="pointerPosition">Экранные координаты</param>
	public void OnPointerUp(Vector3 pointerPosition) {
		isShooting = false;
		if(weaponManager != null) weaponManager.shootTapOn = isShooting;
	}

	/// <summary>
	/// Отпускание клавиши мышы
	/// </summary>
	/// <param name="pointerPosition">Экранные координаты</param>
	public void OnPointerDown(Vector3 pointerPosition) {
		OnPointerDrag(pointerPosition);
		isShooting = true;
		if(weaponManager != null) weaponManager.shootTapOn = isShooting;
	}

	/// <summary>
	/// Начало смахивания
	/// </summary>
	/// <param name="pointerPosition">Экранные коорданаты</param>
	void OnPointerDrag(Vector3 pointerPosition) {
		CheckShootPoint(pointerPosition);
	}

	/// <summary>
	/// Проверка точки выстрела
	/// </summary>
	void CheckShootPoint(Vector3 pointerPosition) {
		shootPoint = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, 12.29f));
	}

	#endregion

	#region Эффекты

	Coroutine poisinCor;
	Coroutine freezCor;

	void OnEffect(int ownerId, List<PlayEffects.EffectData> effect) {
		if(sceneId != ownerId) return;

		foreach(PlayEffects.EffectData effectOne in effect) {
			switch(effectOne.effectType) {
				case EffectType.POISON:
					if(poisinCor != null) StopCoroutine(poisinCor);
					poisinCor = StartCoroutine(PoisonEffect(1));
					break;
				case EffectType.FREEZE:
					if(freezCor != null) StopCoroutine(freezCor);
					freezCor = StartCoroutine(FreezEffect(effectOne.time));
					break;
			}
		}

	}

	IEnumerator PoisonEffect(float time) {
		skeletonAnimation.skeleton.Slots.ForEach(x => x.r = 0.5f);
		skeletonAnimation.skeleton.Slots.ForEach(x => x.b = 0.5f);
		for(int i = 0; i < 10; i++) {
			skeletonAnimation.skeleton.Slots.ForEach(x => x.r += 0.05f);
			skeletonAnimation.skeleton.Slots.ForEach(x => x.b += 0.05f);
			yield return new WaitForSeconds(time / 10);
		}
		skeletonAnimation.skeleton.Slots.ForEach(x => x.r = 1f);
		skeletonAnimation.skeleton.Slots.ForEach(x => x.b = 1f);
	}

	IEnumerator FreezEffect(float time) {
		skeletonAnimation.skeleton.Slots.ForEach(x => x.r = 0.2f);
		skeletonAnimation.skeleton.Slots.ForEach(x => x.g = 0.2f);
		for(int i = 0; i < 10; i++) {
			skeletonAnimation.skeleton.Slots.ForEach(x => x.r += 0.08f);
			skeletonAnimation.skeleton.Slots.ForEach(x => x.g += 0.08f);
			yield return new WaitForSeconds(time / 10);
		}
		skeletonAnimation.skeleton.Slots.ForEach(x => x.r = 1f);
		skeletonAnimation.skeleton.Slots.ForEach(x => x.g = 1f);
	}

	#endregion

	#region Панель загрузки

	public GameObject loadPanel;
	private float timeEndLoad;
	private bool isLoad;
	public System.Action OnComplited;

	public void StartLoad(float startValue, float timeLoad, System.Action OnComplited = null) {
		loadPanel.GetComponent<LoadPlayerHelpers>().StartLoad(startValue, timeLoad);
		loadPanel.SetActive(true);
		timeEndLoad = Time.time + timeLoad;
		this.OnComplited = OnComplited;
		isLoad = true;

		if(isFirst) {
			Dictionary<string,string> brodcast = new Dictionary<string, string>();
			brodcast.Add("anim", "bazookaShoot");
			brodcast.Add("startValue", startValue.ToString());
			brodcast.Add("timeLoad", timeLoad.ToString());
			Generals.Network.NetworkManager.SendPacket(new RequestClientBroadcast(MiniJSON.Json.Serialize(brodcast)));
		}
	}

	public void CheckLoad() {
		if(!isLoad) return;
		if(timeEndLoad <= Time.time) {
			isLoad = false;
			if(OnComplited != null) OnComplited();
		}
	}

	#endregion

	void OnClientBroadcast(string playerId, string jsonString) {
		if(playerId != userId || isFirst) return;

		Debug.Log(jsonString);

		object jsonData = MiniJSON.Json.Deserialize(jsonString);
		Dictionary<string, object> arr = (Dictionary<string, object>)jsonData;
		if(arr.ContainsKey("anim")) {
			switch(arr["anim"].ToString()) {
				case "bazookaShoot":
					StartLoad(float.Parse(arr["startValue"].ToString()), float.Parse(arr["timeLoad"].ToString()));
					break;
			}
		}
	}

}
