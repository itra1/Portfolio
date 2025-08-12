using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;

/// <summary>
/// Тип оружия
/// </summary>
public enum WeaponType {
	pistol,                       // Пистолет
	machinegun,                   // Автомат
	carabine,                     // Дробовик
	poisonArrow,                  // Отравленная стрела
	bazooka,                      // Базоока
	iceGrenade,										// Замораживающая граната
	grenade                       // Зажигательная граната
}

public enum BulletType {
	bullet = 0,                        // Pistol
	spike = 1,                        // Mashinegun
	shotgun = 2,                      // Shotgun
	rocket = 3,                       // Bazooka
	dynamite = 4,                     // Dynamite
	iceBomb = 5,                      // Ice bomb
	poisonArrow = 6                   // Poison arrow
}

/// <summary>
/// Типы ошибок при попытке выстрелить
/// </summary>
public enum WeaponError {
	none,                               // Нет проблем
	noWeapon,                           // Оружие отсутствует
	noTimeWaitShoot,                    // Время между выстрелами еще не истекло
	noBullet,                           // Закончились потроны
	noTapReady,                         // Ждет окончание тапа
	activeShootAnim                     // Активна анимация выстрела
}

/// <summary>
/// Менеджер оружия
/// </summary>
public abstract class WeaponManager : MonoBehaviour {

	public event Actione<WeaponManager> OnChange;

	public    GameObject gunPrefab;                                       // Префаб оружия
	protected GameObject gunInst;                                         // Созданный экземпляр оружия

	public    string  bulletPrefab;                                       // Генерируемый снаряд
	protected float   bulletSpeed;                                        // Скорость полета сняряда
	protected float   rangeAccuracy;                                      // Точность выстрела 
	public    WeaponType weaponType;                                      // Тип оружия
	[HideInInspector]
	public    Player  playerOwner;                                        // Player, выполняющий управление
	protected Vector3 shootPoint;                                         // Координаты выстрела
	protected float   _timeReload = 2;                                    // Время на перезарядку
	public		float		timeReload { get { return _timeReload * koeffReload; } }
	protected float   _timeShoot;                                         // Время последнего выстрела
	public		float		timeShoot { get { return _timeShoot; } }
	protected int     _bulletCountMax;                                    // Максимальное число снарядов в обойме
	protected int     _bulletCount;                                       // Количество снарядов в обойме
	protected WeaponError weaponError;                                    // Ошибка, возвращаемая проверкой
	protected bool    _shootTapOn;                                        // Флаг начаала тапа
	[HideInInspector]
	public		bool    checkShootTapOn = true;                             // Флаг необходимости проверки тапа
	public		static float koeffReload = 1;
	private		float		damage;
	private   float		damageBarrier;

	protected virtual void Start() {
		GetConfig();
	}
	protected virtual void OnEnable() { }
	protected virtual void OnDisable() { }
	public int bulletCount { get { return Mathf.Max(_bulletCount, 0); } }
	/// <summary>
	/// Инифиализация
	/// </summary>
	public virtual void Init(Player newPlayerOwner) {
		OnChange = null;
		playerOwner = newPlayerOwner;
		_bulletCount = _bulletCountMax;
		CreateWeapon();
		playerOwner.VisibleChangePlayer();
	}
	/// <summary>
	/// Установить невидимость
	/// </summary>
	/// <param name="isHidden"></param>
	public void SetHidden(bool isHidden, bool isPlayer = false) {
		gunInst.GetComponent<Weapon>().SetHidden(isHidden, isPlayer);
	}
	/// <summary>
	/// Деактивация
	/// </summary>
	public void Deactive() {
		Destroy(gunInst);
	}
	/// <summary>
	/// Установка орудтя в руку
	/// </summary>
	private void CreateWeapon() {
		gunInst = Instantiate(gunPrefab);
		gunInst.transform.parent = playerOwner.transform;
		gunInst.transform.localPosition = Vector3.zero;
		gunInst.GetComponent<BoneFollowerPlus>().skeletonRenderer = playerOwner.skeletonAnimation;
		gunInst.GetComponent<BoneFollowerPlus>().boneName = playerOwner.boneForWeapon;
		gunInst.GetComponent<BoneFollowerPlus>().Initialize();
		gunInst.GetComponent<Weapon>().graphic.transform.localScale = playerOwner.mainGraphic.transform.localScale;
		gunInst.GetComponent<Weapon>().OnAttackEnd = OnAttackEnd;
		playerOwner.GetComponent<Player>().shootPointStart.localPosition =
			playerOwner.GetComponent<Player>().handPointStatic.localPosition
			+ gunInst.GetComponent<Weapon>().shootPoint.localPosition;
	}

	bool isReadyShoot;

	public void OnShootReady(bool isReady) {
		isReadyShoot = isReady;
		if(gunInst != null) gunInst.GetComponent<Weapon>().ChangeAngle(isReadyShoot);
	}

	public bool shootTapOn {
		set {
			if(value == true && _timeShoot + timeReload > Time.time) return;
			_shootTapOn = value;
		}
	}
	
	protected float timeSendPacket;

	public void StartSendShoot(Vector3 shootPoint) {
		this.shootPoint = shootPoint;
		if(timeSendPacket + 0.1f > Time.time || timeShoot + timeReload > Time.time) return;
		timeSendPacket = Time.time;
		OnStartSendShoot();
	}

	bool CheckWeapon() {
		return true;
	}

	protected virtual void OnStartSendShoot() {
		OnSendShoot();
	}

	protected void OnSendShoot() {
		Generals.Network.NetworkManager.SendPacket(new RequestShot(shootPoint));
	}

	public void OnAttackEnd() {
		OnShootReady(false);
	}

	/// <summary>
	/// Попытка выстрела
	/// </summary>
	public void Shoot(ObjectSpawn objectData) {
		CreateBullet(objectData);
		OnShootReady(true);
		gunInst.GetComponent<Weapon>().ShootAnimPlay();
		_timeShoot = Time.time;
		ChangeWeapon();
	}

	protected virtual void CreateBullet(ObjectSpawn bulletData) {
		_bulletCount--;
	}

	protected bool isReady;

	public void ChangeWeapon() {
		if(OnChange != null) OnChange(this);
	}

	protected Dictionary<string, string> configParametrs;

	protected virtual void GetConfig() {
		configParametrs = GameDesign.instance.cardList.Find(x => x.code == weaponType.ToString()).parameters;
		if(configParametrs == null) return;
		if(configParametrs.ContainsKey("bulletCount"))
			_bulletCountMax = int.Parse(configParametrs["bulletCount"]);
		if(configParametrs.ContainsKey("bulletDelay"))
			_timeReload = float.Parse(configParametrs["bulletDelay"]);
		if(configParametrs.ContainsKey("damage"))
			damage = float.Parse(configParametrs["damage"]);
		if(configParametrs.ContainsKey("damageObstacles"))
			damageBarrier = float.Parse(configParametrs["damageObstacles"]);
		if(configParametrs.ContainsKey("bulletSpeed"))
			bulletSpeed = float.Parse(configParametrs["bulletSpeed"]);
	}

}
