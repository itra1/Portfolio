using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using UnityEngine.UI;
using Game.Weapon;
using Game.User;


namespace Game.UI.Battle
{

	[System.Serializable]
	public struct WeaponIconStruct
	{
		public WeaponType type;
		public GameObject button;
		public GamePlayTimerHelper timer;
		[HideInInspector]
		public float timeWait;
		public Image weaponIcon;
		public GameObject countIcon;
		public Text countText;
		public GameObject addIcon;

		public List<GameObject> activeElements;
		public List<GameObject> deactiveElements;
	}

	public class BattleGamePlay : UiDialog
	{

		public UI.Battle.BattleUiWeapons assistantPanel;
		public UI.Battle.BattleUiWeapons weaponPanel;
		public UI.Battle.BattleUiWeapons abilitiesPanel;

		[SerializeField]
		private GameObject m_ArsenalPanel;
		[SerializeField]
		private GameObject m_TimerPanel;
		[SerializeField]
    private GameObject m_RndWeapon;
		[SerializeField]
    private GameObject m_RndAbilities;

		public GameObject addHealthButton;
		public GameObject addEnergyButton;

		public AudioClipData hearthClip;
		private AudioPoint hearthClipPoint;

		public KeyEnemyUi keyEnemyUi;

		private void OnEnable()
		{

      m_ArsenalPanel.SetActive(UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena);
      m_TimerPanel.SetActive(UserManager.Instance.ActiveBattleInfo.Mode != PointMode.arena);
      m_RndWeapon.SetActive(UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena);
      m_RndAbilities.SetActive(UserManager.Instance.ActiveBattleInfo.Mode == PointMode.arena);

			WeaponGenerator.OnWeaponArrayReady += WeaponsListChange;
			//InitAudio();
			WeaponsListChange();

			assistantPanel.gameObject.SetActive(false);
			weaponPanel.gameObject.SetActive(false);
			abilitiesPanel.gameObject.SetActive(false);

			Invoke("CheckArenaPanel", 0.3f);

#if OPTION_DEBUG
		addHealthButton.SetActive(true);
		addEnergyButton.SetActive(true);
#endif

		}

		private void OnDisable()
		{
			WeaponGenerator.OnWeaponArrayReady -= WeaponsListChange;

			if (hearthClipPoint != null)
			{
				hearthClipPoint.isLocked = false;
				hearthClipPoint.Stop();
			}
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.NumberTimer))]
		public void OnTimer(ExEvent.BattleEvents.NumberTimer newTimer)
		{
			assistantPanel.gameObject.SetActive(newTimer.num <= 3);
			weaponPanel.gameObject.SetActive(newTimer.num <= 2);
			abilitiesPanel.gameObject.SetActive(newTimer.num <= 1);
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.ChangeKeyEnemy))]
		public void ChangeKeyEnemy(ExEvent.BattleEvents.ChangeKeyEnemy keyEnemy)
		{
			if (keyEnemy.useKeyEnemy)
			{
				if (!keyEnemyUi.gameObject.activeInHierarchy)
				{
					keyEnemyUi.gameObject.SetActive(true);
					keyEnemyUi.Show(keyEnemy.enemyType);
				}
				else
					keyEnemyUi.Change(keyEnemy.enemyType);
			}
			else
			{
				if (keyEnemyUi.gameObject.activeInHierarchy)
					keyEnemyUi.Hide();
			}

		}

		[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.GenerateKeyEnemy))]
		public void GenerateKeyEnemy(ExEvent.BattleEvents.GenerateKeyEnemy keyEnemy)
		{
			keyEnemyUi.GenerateKeyEnemy(keyEnemy.enemy);
		}


		#region Weapon

		public void WeaponsListChange()
		{
			if (WeaponGenerator.Instance == null)
				return;

			assistantPanel.WeaponsListChange(WeaponGenerator.Instance.assistantListReady);
			weaponPanel.WeaponsListChange(WeaponGenerator.Instance.weaponsListReady);
			abilitiesPanel.WeaponsListChange(WeaponGenerator.Instance.abilitiesListReady);

		}

		public void ChangeOneIcon(int numIcon, WeaponIconStruct[] iconsArr, WeaponManager weaponManager)
		{
			//iconsArr[numIcon].weaponIcon.sprite = GameDesign.instance.weapons.Find(x => x.type == WeaponGenerator.instance.abilitiesListReady[i].weaponType).iconBattle;
			if (weaponManager.BulletCount > 0)
			{
				iconsArr[numIcon].addIcon.SetActive(false);
				iconsArr[numIcon].countIcon.SetActive(true);
				iconsArr[numIcon].weaponIcon.color = new Color(1f, 1f, 1f, 1);
				iconsArr[numIcon].countText.text = weaponManager.BulletCount.ToString();
			}
			else
			{
				iconsArr[numIcon].addIcon.SetActive(true);
				iconsArr[numIcon].countIcon.SetActive(false);
				iconsArr[numIcon].weaponIcon.color = new Color(0.5f, 0.5f, 0.5f, 1);
			}

		}

		#endregion

		#region Profile

		public void ProfileButton()
		{

			if (Game.User.UserManager.Instance.ActiveBattleInfo.Mode != PointMode.arena)
				return;

			Game.UI.Profile profile = UIController.ShowUi<Game.UI.Profile>();
			profile.gameObject.SetActive(true);
			Time.timeScale = 0;

			profile.OnClose = () =>
			{
				Time.timeScale = 1;
			};

		}

		#endregion

		[ExEvent.ExEventHandler(typeof(BattleEvents.OnCloseBattle))]
		private void OnCloseBattle(BattleEvents.OnCloseBattle cl)
		{
			CloseScene();
		}

		[ExEvent.ExEventHandler(typeof(BattleEvents.BattlePhaseChange))]
		private void BattlePhaseChange(BattleEvents.BattlePhaseChange battlePhase)
		{
			if (battlePhase.phase == BattlePhase.start)
			{

				if (hearthClipPoint != null)
				{
					hearthClipPoint.isLocked = false;
					hearthClipPoint.Stop();
				}
				hearthClipPoint = AudioManager.PlayEffects(hearthClip, AudioMixerTypes.effectPlay);
				hearthClipPoint.isLocked = true;

				assistantPanel.gameObject.SetActive(false);
				weaponPanel.gameObject.SetActive(false);
				abilitiesPanel.gameObject.SetActive(false);
			}

			if (battlePhase.phase == BattlePhase.end)
			{
				if (hearthClipPoint == null)
					return;
				hearthClipPoint.isLocked = false;
				hearthClipPoint.Stop();
			}
		}

		private void CloseScene()
		{
			if (hearthClipPoint != null)
			{
				hearthClipPoint.isLocked = false;
				hearthClipPoint.Stop(0.3f);
			}
		}

		#region Arena

		public Text enemyNumGenerate;
		public Text arenaGroup;
		public Text arenaLevel;

		private void CheckArenaPanel()
		{

		}

		public void GenerateEnemyBtn()
		{
			try
			{
				EnemysSpawn.Instance.GenerateEnemyByNum(int.Parse(enemyNumGenerate.text));
			}
			catch { }
		}

		public void GenerateEnemyWaveBtn()
		{
			try
			{
				EnemysSpawn.Instance.GenerateEnemyWave(int.Parse(arenaGroup.text), int.Parse(arenaLevel.text));
			}
			catch { }
		}

		public void ArenaDeactiveAll()
		{
			EnemysSpawn.Instance.DeactiveAllEnemy();
		}

		#endregion

		/// <summary>
		/// Кнопка полноэкранного режима
		/// </summary>
		public void FullScreenButton()
		{
			UIController.ClickPlay();
			GameManager.Instance.ChangeFullScreen();
		}

		public void AddHealth()
		{
			Game.User.UserHealth.Instance.StartBattle();
		}

		public void RandomWeapon()
		{
			WeaponGenerator.Instance.RandomWeapons(WeaponCategory.weapon);
		}

		public void RandomAbilities()
		{
			WeaponGenerator.Instance.RandomWeapons(WeaponCategory.abilities);
		}

	}

}