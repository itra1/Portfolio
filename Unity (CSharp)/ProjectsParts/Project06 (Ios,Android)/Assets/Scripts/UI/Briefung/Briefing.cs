using System;
using System.Collections.Generic;
using System.Linq;
using Game.Weapon;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{


	/// <summary>
	/// Брифинг
	/// </summary>
	public class Briefing : UiDialog
	{

		public Action OnCancel;

		[Serializable]
		public struct DropItems
		{
			public GameObject element;
			public Animation contentAnimation;
			public Animation lightAnimation;
			public Image icon;
		}

		public Animation mainAnimation;

		public Animation arrowAnim;

		public ScrollSelectWeapon scrollSelect;

		public GameObject dropTitle;
		public List<DropItems> dropList;

		public System.Action<LevelInfo> OnStart;

		public GameObject leftArrowButton;
		public GameObject rightArrowButton;
		public GameObject solidBack;

		public SkeletonGraphic skeletonGraphic;

		public Text title;
		public Text description;
		public Text pageInfo1;
		public Text pageInfo2;
		public GameObject particleCloud;
		public GameObject particleCloudMap;
		//public Text status;

		public List<GameObject> pageList;
		private int _activePage;

		private LevelInfo selectLevelInfo;
		private Configuration.Level _levelInfoData;

		public List<BriefingWeapon> weaponList;
		public List<BriefingWeapon> abilitiesList;
		public List<BriefingWeapon> assistantList;

		private bool startBattle = false;

		private void OnEnable()
		{
			usedEnemy = null;
			startBattle = false;
			mainAnimation.Play("BriefingShow");
			_activePage = 0;
			ChangePage(0);
			scrollSelect.gameObject.SetActive(false);
			solidBack.SetActive(false);
			isStartedBattle = false;
		}

		private EnemyType _enemyType;

		private void ActiveSceletonData()
		{

			_enemyType = EnemyType.DremuchiyRisovod;
			try
			{
				_enemyType = (EnemyType)GameDesign.Instance.allConfig.chapters.Find(x => x.level == selectLevelInfo.Level && x.chapter == selectLevelInfo.Group && x.mobType == "keyEnemy").idMob;
			}
			catch { }

			if (_enemyType == EnemyType.None)
				_enemyType = EnemyType.DremuchiyRisovod;

			EnemyInfo ei = GameDesign.Instance.enemyInfo.Find(x => x.type == _enemyType);
			skeletonGraphic.skeletonDataAsset = ei.skeletonDataAsset;
			skeletonGraphic.startingAnimation = ei.runAnim;
			skeletonGraphic.Initialize(true);

		}

		private void ActivateDropAnimation()
		{

			foreach (var drop in dropList)
			{
				drop.element.GetComponent<Animation>()["DropLightIdle"].time = UnityEngine.Random.Range(0f, 3f);
				drop.element.GetComponent<Animation>().Play("DropLightIdle");
				drop.contentAnimation["ContentIdle"].time = UnityEngine.Random.Range(0f, 1f);
				drop.contentAnimation.Play("ContentIdle");
				drop.lightAnimation["DropLight"].time = UnityEngine.Random.Range(0f, 1f);
				drop.lightAnimation.Play("DropLight");
			}
		}

		public void Close()
		{

			gameObject.SetActive(false);
			if (OnCancel != null)
				OnCancel();
		}

		private WeaponManager GetWeaponByCondition()
		{

			Configuration.Level level = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == selectLevelInfo.Group && x.level == selectLevelInfo.Level);

			if (level == null)
			{
				Debug.LogWarning("Не найден конфиг на уровень, если зпущена не арена, проверьте настройки");
				return null;
			}

			int? openNum = level.openWeapon;

			if (!openNum.HasValue || openNum.Value == 0)
				return null;

			WeaponType wep = (WeaponType)openNum.Value;
			WeaponManager wm = Game.User.UserWeapon.Instance.weaponsManagers.Find(x => x.weaponType == wep);
			wm.BulletCount = (int)GameDesign.Instance.allConfig.weapon.Find(x => x.id == (int)wep).startBullet.Value;

			return Game.User.UserWeapon.Instance.weaponsManagers.Find(x => x.weaponType == wep);

		}

		private void InitWeapons()
		{
			weaponList.ForEach(x => x.SetWeapon(null));
			abilitiesList.ForEach(x => x.SetWeapon(null));
			assistantList.ForEach(x => x.SetWeapon(null));

			List<WeaponType> wpList = Game.User.UserWeapon.Instance.GetSelected(selectLevelInfo.Mode == PointMode.survival);

			List<WeaponManager> wepList = new List<WeaponManager>();
			List<WeaponManager> abiList = new List<WeaponManager>();
			List<WeaponManager> assistList = new List<WeaponManager>();

			foreach (var VARIABLE in wpList)
			{
				WeaponManager wm = Game.User.UserWeapon.Instance.weaponsManagers.Find(x => x.GetComponent<WeaponManager>().weaponType == VARIABLE);

				if (wm == null)
					continue;

				switch (wm.category)
				{
					case WeaponCategory.abilities:
						abiList.Add(wm);
						break;
					case WeaponCategory.asisstant:
						assistList.Add(wm);
						break;
					case WeaponCategory.weapon:
						wepList.Add(wm);
						break;
				}
			}
			WeaponManager justUnlockedWM = GetWeaponByCondition();
			/*string justUnlockedStr = PlayerPrefs.GetString("JustUnlockedWeapon", "");
      int justUnlockedGr = PlayerPrefs.GetInt("JustUnlockedWeapon_gr", 0);
      int justUnlockedLvl = PlayerPrefs.GetInt("JustUnlockedWeapon_lvl", 0);
      WeaponManager justUnlockedWM = null;
      WeaponType justUnlockedWT = WeaponType.none;
        Debug.Log("just unlocked: " + justUnlockedStr + "_" + justUnlockedGr + "_"  +justUnlockedLvl + "_" + selectLevelInfo.Group + "_" + selectLevelInfo.Level);
      if (justUnlockedStr != "" && selectLevelInfo.Group == justUnlockedGr && selectLevelInfo.Level == justUnlockedLvl) {
        justUnlockedWT = (WeaponType)Enum.Parse(typeof(WeaponType), justUnlockedStr);
        justUnlockedWM = Game.User.UserWeapon.Instance.GetWeapon(justUnlockedWT);
        if (justUnlockedWM.BulletCount == 0) {
          justUnlockedWM = null;
        }
      }*/
			int i = 0;
			foreach (var wm in wepList)
			{
				if (Game.User.UserWeapon.Instance.GetWeapon(wm.weaponType).BulletCount == 0)
					continue;
				weaponList[i].SetWeapon(wm);
				i++;
			}
			i = 0;
			if (justUnlockedWM != null && justUnlockedWM.category == WeaponCategory.abilities)
			{
				abilitiesList[0].SetWeapon(justUnlockedWM);
				i = 1;
			}
			foreach (var wm in abiList)
			{
				if (Game.User.UserWeapon.Instance.GetWeapon(wm.weaponType).BulletCount == 0 || (justUnlockedWM != null && wm.weaponType == justUnlockedWM.weaponType))
					continue;
				abilitiesList[i].SetWeapon(wm);
				i++;
			}
			if (justUnlockedWM != null && justUnlockedWM.category == WeaponCategory.asisstant)
			{
				assistantList[0].SetWeapon(justUnlockedWM);
				i = 1;
			}
			i = 0;
			foreach (var wm in assistList)
			{
				if (Game.User.UserWeapon.Instance.GetWeapon(wm.weaponType).BulletCount == 0 || (justUnlockedWM != null && wm.weaponType == justUnlockedWM.weaponType))
					continue;
				assistantList[i].SetWeapon(wm);
				i++;
			}

			CheckReady();

		}

		public void SetData()
		{

			//weaponGroup.Init(weaponElementPrefab, _levelInfoData, allWeapon);
			//weaponGroup.Draw();
			//abilityGroup.Init(weaponElementPrefab, _levelInfoData, allWeapon);
			//abilityGroup.Draw();
			//assistantGroup.Init(weaponElementPrefab, _levelInfoData, allWeapon);
			//assistantGroup.Draw();

		}

		private void SaveSelectable()
		{

			List<WeaponType> weapList = new List<WeaponType>();

			foreach (var elem in weaponList)
			{
				if (elem.weaponManager != null)
					weapList.Add(elem.weaponManager.weaponType);
			}

			foreach (var elem in abilitiesList)
			{
				if (elem.weaponManager != null)
					weapList.Add(elem.weaponManager.weaponType);
			}

			foreach (var elem in assistantList)
			{
				if (elem.weaponManager != null)
					weapList.Add(elem.weaponManager.weaponType);
			}
			Game.User.UserWeapon.Instance.SetSelected(weapList, selectLevelInfo.Mode == PointMode.survival);
		}

		public void FillInfo(LevelInfo newPointInfo)
		{

			selectLevelInfo = newPointInfo;

			rightArrowButton.GetComponent<Button>().transition = (CheckForwardButton() ? Selectable.Transition.Animation : Selectable.Transition.None);

			if (selectLevelInfo.Status != PointSatus.open || selectLevelInfo.Status != PointSatus.complited)

				if (selectLevelInfo.Mode == PointMode.survival || selectLevelInfo.Mode == PointMode.arena)
				{
					title.text = "Режим выживания";
					description.text = "Для настоящих хардкорщиков. Вам нужно выжить любой ценой, отбивая бесконечные волны зомбаков. Рискованных ждет достойная награда";
					pageInfo1.text = "";
				}
				else
				{
					_levelInfoData = GameDesign.Instance.allConfig.levels.Find(x => x.chapter == selectLevelInfo.Group && x.level == selectLevelInfo.Level);
					title.text = _levelInfoData.title;
					description.text = _levelInfoData.description;
					pageInfo1.text = _levelInfoData.brifing;
				}
			pageInfo2.text = "Кликни по слоту для выбора оружия";

			foreach (var elem in weaponList)
			{
				elem.OnPointerDown = PointDownBriefingWeapon;
				elem.OnPointerUp = PointUpBriefingWeapon;
			}

			foreach (var elem in abilitiesList)
			{
				elem.OnPointerDown = PointDownBriefingWeapon;
				elem.OnPointerUp = PointUpBriefingWeapon;
			}

			foreach (var elem in assistantList)
			{
				elem.OnPointerDown = PointDownBriefingWeapon;
				elem.OnPointerUp = PointUpBriefingWeapon;
			}

			try
			{
				ActiveSceletonData();
			}
			catch { }

			InitDropList();

			InitWeapons();
		}

		private bool CheckForwardButton()
		{

			return (_activePage == 0 || CheckSelectWeapon()) &&
						 (selectLevelInfo != null && (selectLevelInfo.Status & PointSatus.IsActive) != 0);
		}

		private void InitDropList()
		{

		}

		private int _countNotFull = 0;

		public void PointDownBriefingWeapon(BriefingWeapon brif, WeaponCategory category)
		{

			if (scrollSelect.gameObject.activeInHierarchy)
				scrollSelect.gameObject.SetActive(false);

			//List<WeaponManager> readyManagers = new List<WeaponManager>();
			List<WeaponManager> readyManagers = Game.User.UserWeapon.Instance.WeaponList.FindAll(x => x.category == category);

			//foreach (var VARIABLE in Game.User.UserWeapon.Instance.WeaponList) {

			//  //WeaponManager wm = Game.User.UserWeapon.Instance.weaponsManagers.Find(
			//  //x =>
			//  //  x.category == category &&
			//  //  x.weaponType == VARIABLE.type);

			//  //if (VARIABLE != null) {
			//  readyManagers.Add(VARIABLE);
			//  //}
			//}

			WeaponManager cond = GetWeaponByCondition();

			if (cond != null && !readyManagers.Exists(x => x.weaponType == cond.weaponType))
			{
				cond.GetConfig();
				readyManagers.Add(cond);
			}

			List<WeaponManager> removeList = new List<WeaponManager>();

			_countNotFull = 0;
			foreach (var oneManager in readyManagers)
			{

				if (oneManager != null)
				{
					oneManager.GetConfig();
					if (oneManager.BulletCount > 0)
						_countNotFull++;

					bool ext = false;
					foreach (var elem in weaponList)
					{
						if (brif.weaponManager != oneManager && elem.weaponManager == oneManager)
							ext = true;
					}
					foreach (var elem in abilitiesList)
					{
						if (brif.weaponManager != oneManager && elem.weaponManager == oneManager)
							ext = true;
					}
					foreach (var elem in assistantList)
					{
						if (brif.weaponManager != oneManager && elem.weaponManager == oneManager)
							ext = true;
					}

					if (ext)
						removeList.Add(oneManager);
				}
			}

			readyManagers.RemoveAll(x => removeList.Contains(x));
			readyManagers = readyManagers.FindAll(x => x.category == category);

			if (readyManagers.Count == 0 || !readyManagers.Exists(x => x.category == category))
			{
				return;
			}
			if (readyManagers.Count == 1 && readyManagers[0].category == category)
			{
				brif.SetWeapon(readyManagers[0]);
				ScrollClose(brif, category);
				return;
			}

			//brif.gameObject.SetActive(false);
			brif.content.SetActive(false);
			solidBack.gameObject.SetActive(true);

			readyManagers = readyManagers.OrderBy(x => x.orderId).ToList<WeaponManager>();
			readyManagers.Insert(0, null);

			scrollSelect.SetListManagers(readyManagers, brif);
			scrollSelect.SetFirstMove();

			scrollSelect.OnClose = ScrollClose;
			scrollSelect.gameObject.SetActive(true);
			scrollSelect.GetComponent<RectTransform>().position = brif.GetComponent<RectTransform>().position;
		}

		public void PointUpBriefingWeapon(BriefingWeapon brif, WeaponCategory category)
		{
			scrollSelect.PointUp();
		}

		public void ScrollClose(BriefingWeapon brif, WeaponCategory category)
		{

			brif.content.SetActive(true);

			solidBack.gameObject.SetActive(false);

			if (SceneManager.GetActiveScene().name == "Map")
			{
				if (!particleCloudMap.activeInHierarchy)
					particleCloudMap.SetActive(true);
				particleCloudMap.transform.position = brif.transform.position;

				particleCloudMap.GetComponent<ParticleSystem>().Play();
			}
			else
			{
				if (!particleCloud.activeInHierarchy)
					particleCloud.SetActive(true);
				particleCloud.transform.position = brif.transform.position;

				particleCloud.GetComponent<ParticleSystem>().Play();
			}

			rightArrowButton.GetComponent<Button>().transition = ((_activePage == 0 || CheckSelectWeapon()) && (selectLevelInfo != null && selectLevelInfo.Status == PointSatus.open) ? Selectable.Transition.Animation : Selectable.Transition.None);
			CheckReady();
		}

		private bool CheckSelectWeapon()
		{
			int countSelect = 0;

			foreach (var elem in weaponList)
			{
				if (elem.weaponManager != null)
				{
					countSelect++;
				}
			}

			foreach (var elem in abilitiesList)
			{
				if (elem.weaponManager != null)
				{
					countSelect++;
				}
			}
			return countSelect > 0;
		}

		private void CheckReady()
		{

			bool allSelect = true;
			int countSelect = 0;
			int countNotFull = 0;

			foreach (var elem in weaponList)
			{
				if (elem.weaponManager != null)
				{
					countSelect++;
					if (elem.weaponManager.BulletCount > 0)
						countNotFull++;
				}
				else
				{
					allSelect = false;
				}
			}

			foreach (var elem in abilitiesList)
			{
				if (elem.weaponManager != null)
				{
					countSelect++;
					if (elem.weaponManager.BulletCount > 0)
						countNotFull++;
				}
				else
				{
					allSelect = false;
				}
			}

			foreach (var elem in assistantList)
			{
				if (elem.weaponManager != null)
				{
					countSelect++;
					if (elem.weaponManager.BulletCount > 0)
						countNotFull++;
				}
				else
				{
					allSelect = false;
				}
			}

			if (allSelect || Game.User.UserWeapon.Instance.WeaponList.Count - 2 == countSelect || _countNotFull == countNotFull)
			{
				arrowAnim.Play("ReadyButtonRight");
			}
			else
			{
				arrowAnim.Stop();
				arrowAnim.transform.eulerAngles = Vector3.zero;
			}

		}

		public void ButtonLeft()
		{
			if (_activePage == pageList.Count - 1)
				ChangePage(-1);
			UIController.ClickPlay();
		}

		private bool isStartedBattle;

		public void ButtonRight()
		{

			if (!CheckForwardButton())
			{
				UIController.RejectPlay();
				return;
			}

			if (_activePage == pageList.Count - 1 && isStartedBattle)
				return;

			rightArrowButton.SetActive(false);
			UIController.ClickPlay();
			rightArrowButton.SetActive(true);
			rightArrowButton.GetComponent<Animator>().Rebind();


			if (_activePage == pageList.Count - 1)
			{
				isStartedBattle = true;
				StartButton(1);
			}
			else
				ChangePage(1);
			//rightArrowButton.GetComponent<Animator>().Rebind();
		}

		public void CloseButton()
		{

			mainAnimation.Play("BriefingHide");
		}

		private void ChangePage(int increment)
		{
			_activePage += increment;
			if (_activePage < 0)
				_activePage = pageList.Count - 1;
			if (_activePage >= pageList.Count)
				_activePage = 0;

			for (int i = 0; i < pageList.Count; i++)
				pageList[i].SetActive(i == _activePage);
			leftArrowButton.SetActive(_activePage != 0);


			if (_activePage == 0)
			{
				arrowAnim.Play("ReadyButtonRight");
			}
			else
			{
				CheckReady();
			}

			rightArrowButton.GetComponent<Button>().transition = (CheckForwardButton() ? Selectable.Transition.Animation : Selectable.Transition.None);

			ActivateDropAnimation();
		}

		public void StartButton(int farmPoint = 1)
		{

			SaveSelectable();

			if (Game.User.UserWeapon.Instance.GetSelected(selectLevelInfo.Mode == PointMode.survival).Count <= 0)
				return;

			if ((selectLevelInfo.Status & PointSatus.IsClose) != 0)
				return;
			DarkTonic.MasterAudio.MasterAudio.StopPlaylist("PlaylistController");
			//backSound.Stop();
			//backMusicPoint.Stop();
			//AudioManager.backMusic.UnPause();

			if (selectLevelInfo.Mode == PointMode.farm)
			{
				if (selectLevelInfo.FarmPoint <= selectLevelInfo.FarmPointActive)
					return;
				//selectLevelInfo.FarmPointActive = farmPoint;
			}

			if (OnStart != null)
				OnStart(selectLevelInfo);
		}

		private void ShowErrorDialog(string testMessage)
		{
			ErrorDialog gui = UIController.ShowUi<ErrorDialog>();
			gui.gameObject.SetActive(true);
			gui.errorText.text = testMessage;
		}


		private Enemy usedEnemy;

		/// <summary>
		/// Клик по enemy
		/// </summary>
		public void EnemyClick()
		{
			Debug.Log("enemy click");
			if (usedEnemy == null)
			{
				GameObject inst = ResourceManager.Instance.LoadResources<GameObject>("Enemy/" + _enemyType.ToString());
				usedEnemy = inst.GetComponent<Enemy>();
			}

			AudioBlock playBlock;

			int var = UnityEngine.Random.Range(0, 2);

			playBlock = var == 0 ? usedEnemy.attackAudioBlock : usedEnemy.deadAudioBlock;

			if (playBlock == null)
				playBlock = var == 0 ? usedEnemy.deadAudioBlock : usedEnemy.attackAudioBlock;
			Debug.Log(playBlock);
			if (playBlock != null)
				playBlock.PlayRandom(this);

		}

	}


}