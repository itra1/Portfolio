using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ZbCatScene;
using Game.User;

namespace Game.Weapon
{


	/// <summary>
	/// Структура (название оружие, изображение и иконка), отображаемая в редакторе 
	/// </summary>
	[System.Serializable]
	public struct WeaponStruct
	{
		public string name;             // Имя объекта
		public Game.Weapon.WeaponType type;        // Тип
		public Sprite icon;             // Иконка
		public int burrefSize;          // Размер буфера
	}


	/// <summary>
	/// Генератор оружия
	/// </summary>
	[RequireComponent(typeof(Pool))]
	public class WeaponGenerator : Singleton<WeaponGenerator>
	{

		//public List<GameObject> weaponsControllersPrefabs;                          // Массив всех существующих мнеджеров
		[HideInInspector]
		public List<WeaponManager> weaponsControllers;                              // Все созданные менеджеры оружия
		[HideInInspector]
		public List<WeaponManager> weaponsListReady = new List<WeaponManager>();    // Список менеджеров простого оружия
		[HideInInspector]
		public List<WeaponManager> abilitiesListReady = new List<WeaponManager>();  // Список менеджеров сбилок
		[HideInInspector]
		public List<WeaponManager> assistantListReady = new List<WeaponManager>();  // Список менеджеров сбилок

		public static event System.Action OnWeaponArrayReady;

		// Событие выбора оружия
		public static event System.Action<WeaponType> WeaponSelected;

		// Событие тапа по экрану и необходимости совершения вустрела
		public static event System.Action<WeaponManager> shootEvent;

		public delegate void StartReloadDelegate(WeaponManager weaponManager, float fullTime, float fromTime = 1, float toTime = 0);
		public static event StartReloadDelegate StartReload;

		private Pool pool;

		protected void Start()
		{
			pool = GetComponent<Pool>();
			CreateWeaponsControllers();
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattlePhaseChange))]
		public void PhaseChange(ExEvent.BattleEvents.BattlePhaseChange phase)
		{
			if (phase.phase == BattlePhase.start)
			{
				BattleManager.Instance.ClearBulletCount();
			}
		}

		/// <summary>
		/// перезарядка всего
		/// </summary>
		public void OverchargeAllWeapon()
		{
			weaponsControllers.ForEach(x => x.Overcharge());
		}
		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.KeyDown))]
		void OnKeyDown(ExEvent.GameEvents.KeyDown keyDown)
		{
			switch (keyDown.keyCode)
			{
				case KeyCode.Alpha1:
					if (assistantListReady.Count <= 0)
						break;
					SetActiveWeaponManager(assistantListReady[0].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha2:
					if (assistantListReady.Count <= 1)
						break;
					SetActiveWeaponManager(assistantListReady[1].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha3:
					if (weaponsListReady.Count <= 0)
						break;
					SetActiveWeaponManager(weaponsListReady[0].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha4:
					if (weaponsListReady.Count <= 1)
						break;
					SetActiveWeaponManager(weaponsListReady[1].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha5:
					if (weaponsListReady.Count <= 2)
						break;
					SetActiveWeaponManager(weaponsListReady[2].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha6:
					if (abilitiesListReady.Count <= 0)
						break;
					SetActiveWeaponManager(abilitiesListReady[0].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha7:
					if (abilitiesListReady.Count <= 1)
						break;
					SetActiveWeaponManager(abilitiesListReady[1].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Alpha8:
					if (abilitiesListReady.Count <= 2)
						break;
					SetActiveWeaponManager(abilitiesListReady[2].weaponType, WeaponSelectType.number);
					break;
				case KeyCode.Mouse2:
					selectWeaponInverce = !selectWeaponInverce;
					break;
			}
		}
		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.MouseScroll))]
		private void OnMouseScroll(ExEvent.GameEvents.MouseScroll mouseScroll)
		{
			if (mouseScroll.deltaScroll == 0) return;

			bool increment = (mouseScroll.deltaScroll > 0 ? !selectWeaponInverce : selectWeaponInverce);

			int index = weaponsControllers.FindIndex(x => x.isSelected);

			index += (increment ? 1 : -1);

			if (index > weaponsControllers.Count - 1)
				index = 0;
			if (index < 0)
				index = weaponsControllers.Count - 1;

			SetActiveWeaponManager(weaponsControllers[index].weaponType, WeaponSelectType.scroll);
		}

		public void RandomWeapons(WeaponCategory weaponcategory)
		{

			List<WeaponManager> managers = Game.User.UserWeapon.Instance.weaponsManagers;

			Game.User.UserWeapon.Instance.GetSelected().RemoveAll(x => managers.Exists(y => y.weaponType == x && y.category == weaponcategory));

			List<WeaponType> types = new List<WeaponType>();

			for (int i = 0; i < managers.Count; i++)
			{
				if (managers[i].category == weaponcategory)
					types.Add(managers[i].weaponType);
			}

			int count = 0;

			while (count < 3)
			{
				WeaponType typeSer = types[Random.Range(0, types.Count)];
				if (typeSer == WeaponType.cat)
					continue;

				if (!Game.User.UserWeapon.Instance.GetSelected().Contains(typeSer))
				{
					Game.User.UserWeapon.Instance.GetSelected().Add(typeSer);
					count++;
				}
			}

			CreateWeaponsControllers();
		}

		bool selectWeaponInverce;

		private void AddOpenCondition(ref List<WeaponType> weaponList)
		{

			if (weaponList.Count > 5)
				return;

			if (UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena)
				return;

			int? openNum = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == UserManager.Instance.ActiveBattleInfo.Group && x.level == UserManager.Instance.ActiveBattleInfo.Level).openWeapon;

			if (!openNum.HasValue || openNum.Value == 0)
				return;
			Debug.Log("add open condition");
			WeaponType wep = (WeaponType)openNum.Value;
			Debug.Log("add open condition");
			if (Game.User.UserWeapon.Instance.AddOpenCondition(wep))
			{


			}

			if (!weaponList.Contains(wep))
				weaponList.Add(wep);

		}

		/// <summary>
		/// Создание экземпляров общих контроллеров
		/// </summary>
		public void CreateWeaponsControllers()
		{

			weaponsControllers.ForEach((inst) =>
			{
				Destroy(inst.gameObject);
			});
			weaponsControllers.Clear();
			weaponsListReady.Clear();
			abilitiesListReady.Clear();
			assistantListReady.Clear();

			List<WeaponManager> managers = Game.User.UserWeapon.Instance.weaponsManagers;
			List<WeaponBullets> bulletsArr = new List<WeaponBullets>();

			WeaponCategory activeCategoryWeapon = WeaponCategory.asisstant;

			bool isSelect = false;

			List<WeaponType> weaponSelectList = new List<WeaponType>(Game.User.UserWeapon.Instance.GetSelected(UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival));

			AddOpenCondition(ref weaponSelectList);

			Game.User.UserWeapon.Instance.SetSelected(weaponSelectList, UserManager.Instance.ActiveBattleInfo.Mode == PointMode.survival);

			if (CatSceneManager.Instance.isSpecLevel)
			{
				weaponSelectList.Clear();
				weaponSelectList.Add(WeaponType.tomato);
				weaponSelectList.Add(WeaponType.radiator);
				weaponSelectList.Add(WeaponType.molotov);
				weaponSelectList.Add(WeaponType.holyGrenade);
				weaponSelectList.Add(WeaponType.bazooka);
				weaponSelectList.Add(WeaponType.bear);
				weaponSelectList.Add(WeaponType.varganich);
			}

			WeaponManager.UnlimBullet = CatSceneManager.Instance.isSpecLevel;

			for (int i = 0; i < managers.Count; i++)
			{

				if (weaponSelectList.Exists(x => x == managers[i].weaponType))
				{
					GameObject instWeapon = Instantiate(managers[i].gameObject, transform);
					WeaponManager manager = instWeapon.GetComponent<WeaponManager>();

					instWeapon.SetActive(true);
					manager.GetConfig();
					//weaponsControllers.Add(manager);

					if (!isSelect || activeCategoryWeapon != WeaponCategory.weapon)
					{
						activeCategoryWeapon = manager.category;
						isSelect = true;
					}

					switch (manager.category)
					{
						case WeaponCategory.weapon:
							weaponsListReady.Add(manager);
							WeaponBullets bull = new WeaponBullets();
							bull.count = 0;
							bull.weaponType = manager.weaponType;
							bulletsArr.Add(bull);
							break;
						case WeaponCategory.abilities:
							abilitiesListReady.Add(manager);
							WeaponBullets bull2 = new WeaponBullets();
							bull2.count = 0;
							bull2.weaponType = manager.weaponType;
							bulletsArr.Add(bull2);
							break;
						case WeaponCategory.asisstant:
							assistantListReady.Add(manager);
							break;
					}
				}
			}

			foreach (WeaponManager oneMeneger in weaponsListReady)
				weaponsControllers.Add(oneMeneger);
			foreach (WeaponManager oneMeneger in abilitiesListReady)
				weaponsControllers.Add(oneMeneger);
			foreach (WeaponManager oneMeneger in assistantListReady)
				weaponsControllers.Add(oneMeneger);

			assistantListReady = assistantListReady.OrderBy(x => x.orderId).ToList<WeaponManager>();
			weaponsListReady = weaponsListReady.OrderBy(x => x.orderId).ToList<WeaponManager>();
			abilitiesListReady = abilitiesListReady.OrderBy(x => x.orderId).ToList<WeaponManager>();

			WeaponType activeWeapon = WeaponType.tomato;
			if (weaponsListReady.Count > 0)
			{
				activeWeapon = weaponsListReady[0].weaponType;
			}
			else if (abilitiesListReady.Count > 0)
			{
				activeWeapon = abilitiesListReady[0].weaponType;
			}
			else
			{
				activeWeapon = assistantListReady[0].weaponType;
			}

			BattleManager.Instance.weaponBullets = bulletsArr.ToArray();
			SetActiveWeaponManager(activeWeapon);
			if (OnWeaponArrayReady != null)
				OnWeaponArrayReady();
		}

		[HideInInspector]
		public bool changeWeapon = true;
		[System.Flags]
		public enum WeaponSelectType
		{
			none = 0
			, iconClick = 1
			, scroll = 2
			, number = 4
		}

		public void SetActiveWeaponManager(WeaponType weaponType, WeaponSelectType selectType = WeaponSelectType.none, bool isSpecial = false)
		{
			WeaponManager manager = weaponsControllers.Find(x => x.weaponType == weaponType);
			SetActiveWeaponManager(manager, selectType, isSpecial);
		}

		public void SetActiveWeaponManager(string uuid, WeaponSelectType selectType = WeaponSelectType.none, bool isSpecial = false)
		{
			WeaponManager manager = weaponsControllers.Find(x => x.uuid.Equals(uuid));
			SetActiveWeaponManager(manager, selectType, isSpecial);
		}

		public void SetActiveWeaponManager(WeaponManager weapon, WeaponSelectType selectType = WeaponSelectType.none, bool isSpecial = false)
		{

			if (!changeWeapon || BattleManager.battlePhase == BattlePhase.end)
				return;

			if (weapon.IsReloading && (selectType & (WeaponSelectType.iconClick | WeaponSelectType.number)) != 0)
				UIController.RejectPlay();

			//if (!CatSceneManager.Instance.isSpecLevel && manager.category != WeaponCategory.asisstant && manager.bulletCount <= 0) {
			//  BuyWeapon(weaponsControllers.Find(x => x.weaponType == weaponType), isSpecial);
			//} else {
			//  SetActiveWeaponController(weaponType);
			//}
			//if (!CatSceneManager.Instance.isSpecLevel && manager.category != WeaponCategory.asisstant && manager.bulletCount <= 0) {
			//  BuyWeapon(weaponsControllers.Find(x => x.weaponType == weaponType), isSpecial);
			//} else {
			SetActiveWeaponController(weapon.weaponType);
			//}
		}


		WeaponType lastWeaponType = WeaponType.tomato;

		/// <summary>
		/// Установка используемого контроллера
		/// </summary>
		public void SetActiveWeaponController(WeaponType weaponType)
		{

			WeaponManager manager = PlayerController.Instance.GetWeaponManager();
			if (manager != null)
			{
				// ЗАпоминаем предыдущий тип оружия, если это обычное оружие
				if (manager.category == WeaponCategory.weapon)
					lastWeaponType = manager.weaponType;

				// При повторном клике по иконки абилити, переключаемся на предыдущее оружие
				if (manager.category == WeaponCategory.abilities && manager.weaponType == weaponType)
				{
					SetBeforeWeaponType();
					return;
				}
			}
			for (int i = 0; i < weaponsControllers.Count; i++)
			{
				if (weaponsControllers[i].weaponType == weaponType)
				{

					PlayerController.Instance.SetNewWeapon(weaponsControllers[i]);

					if (WeaponSelected != null)
						WeaponSelected(weaponType);
				}
			}
		}

		public void SetBeforeWeaponType()
		{
			return;
			SetActiveWeaponController(lastWeaponType);
		}

		public void StartWeaponReload(WeaponManager weaponManager, float fullTime, float fromTime = 1, float toTime = 0)
		{
			if (StartReload != null)
				StartReload(weaponManager, fullTime, fromTime, toTime);
		}

		/// <summary>
		/// Событие совершения выстрела
		/// </summary>
		/// <param name="queueNext">Необходимо переключить очередь</param>
		public void WeaponOnShoot(WeaponManager weaponManager, bool queueNext = false)
		{
			if (shootEvent != null)
				shootEvent(weaponManager);

			if (queueNext)
			{
				//if (bardyShootType) GenerateNewWeapon();
			}
			else
			{
				//SetActiveWeaponController(weaponManager.weaponType);
				//SetActiveWeaponController(lastWeaponType);
			}
		}

		#region Покупка Оружия

		public void BuyWeapon(WeaponManager buyWeaponManager, bool isSpecial = false)
		{
			Time.timeScale = 0;
			WeaponBuyPanel panel = UIController.ShowUi<WeaponBuyPanel>();
			panel.gameObject.SetActive(true);
			panel.SetWeaponManager(buyWeaponManager, isSpecial);
		}

		public void BuyWeaponConfirm(WeaponManager buyWeaponManager, bool isSpecial)
		{
			if (!isSpecial)
				(buyWeaponManager as Game.Weapon.WeaponStandart).IncrementBulletCount(50);
			else
			{
				(buyWeaponManager as Game.Weapon.WeaponStandart).IncrementBulletCount(3);
			}
			BuyWeaponCancel();
			SetActiveWeaponController(buyWeaponManager.weaponType);
		}

		public void BuyWeaponCancel()
		{
			Time.timeScale = 1;
		}

		#endregion

		#region Счетчик

		//public static int hitCount;

		//public static void DestroyBullet(WeaponType wt, Enemy enemy, Vector3 pos) {

		//	if (enemy != null) {
		//		hitCount++;
		//	} else {
		//		hitCount = 0;
		//	}
		//	if (hitCount >= 5) {
		//		if (wt != WeaponType.tomato || wt != WeaponType.axe || wt != WeaponType.brick || wt != WeaponType.radiator)
		//			BattleEventEffects.Instance.VisualEffect(BattleEffectsType.enyHitAfter5, pos + Vector3.down * 2, hitCount - 5);
		//	}
		//}


		#endregion

	}



}