using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct DailyBonusType {
	public Image prefab;
	public ShopElementType type;
	public string title;
}

/// <summary>
/// Дневной бонус
/// </summary>
public class DailyBonus : PanelUi {

	public Action OnOpen;

	public DailyBonusType[] objectPrefabs;

	public ParticleSystem bomEffect;

	bool isReady;
	Animator animComponent;
	public GameObject element;
	public Text countElements;
	public int allInBox;
	public int thusNum;

	public Text elementTitle;
	public Text elemenCount;

	public AudioClip openClip;
	public AudioClip idleClip;
	public AudioClip closeClip;

	AudioSource audioComp;

	bool nextBonusReady;
	int dailyBonusCountShow;

	protected override void OnEnable() {
		base.OnEnable();
		audioComp = GetComponent<AudioSource>();
		animComponent = GetComponent<Animator>();
		countElements.text = allInBox.ToString();
		thusNum = 0;
		AudioManager.PlayEffect(openClip, AudioMixerTypes.mapEffect);
		
#if PLUGIN_VOXELBUSTERS
		string notif = Apikbs.CreateLocalNotification(GetDeltaTime(60 * 60 * 24) , VoxelBusters.NativePlugins.eNotificationRepeatInterval.NONE, "Jack Rover", LanguageManager.GetTranslate("push_DailyBonus"));
		PlayerPrefs.SetString("localPushDailyBonus", notif);
#endif

		dailyBonusCountShow = PlayerPrefs.GetInt("dailyBonusCountShow", 0) + 1;
		PlayerPrefs.SetInt("dailyBonusCountShow", dailyBonusCountShow);
		if (OnOpen != null) OnOpen();
	}

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null) OnClose();
	}

	public static int GetDeltaTime(int deltatimeNeed) {
		int unixTime = (int)( System.DateTime.Now - new System.DateTime(1970 , 1 , 1 , 0 , 0 , 0 , 0) ).TotalSeconds;
		int unixTimeAdd = unixTime + deltatimeNeed;
		int fullTime = ( unixTimeAdd % ( 60 * 60 * 24 )) / (60*60);
		int deltatime = 0;
		if(fullTime < 10)
			deltatime = (10 * 60 * 60) - (fullTime * 60 * 60);
		if(fullTime > 22)
			deltatime = fullTime - (22 * 60 * 60) + (10 * 60 * 60);
		return deltatime + deltatimeNeed;
	}
	
	public void ShowDailyFinish() {
		animComponent.SetTrigger("chestReady");
		isReady = true;
	}

	public void Tap() {
		if(isReady) {
			audioComp.Stop();
			isReady = false;
			animComponent.SetTrigger("chestOpen");
			ShowNextElement();
		}

		if(nextBonusReady) {
			nextBonusReady = false;
			NextCoinsBonus();
		}
	}

	public void EventChestJump() {
		AudioManager.PlayEffect(idleClip, AudioMixerTypes.mapEffect);
	}

	void NextCoinsBonus() {
		element.GetComponent<elementInDailyBonus>().HideElement();
		animComponent.SetBool("showElement", false);
	}

	public void ShowNextElement() {
		thusNum++;

		if(thusNum > allInBox) {
			HideAll();
			return;
		}

		animComponent.SetTrigger("nextNum");

		bomEffect.Play();

		if(thusNum == 1) {

			int countCoins = Random.Range(500, 1101);
			UserManager.coins += countCoins;
			elementTitle.text = LanguageManager.GetTranslate(objectPrefabs[0].title);
			elemenCount.text = countCoins.ToString();
			ActiveSprite(objectPrefabs[0].type);
			//gamePlay.SetCoinsCountText();
		} else {
			int needNum = Random.Range(1,objectPrefabs.Length);
			int value = PlayerPrefs.GetInt(objectPrefabs[needNum].type.ToString());
			int count = Random.Range(1,3);
			PlayerPrefs.SetInt(objectPrefabs[needNum].type.ToString(), value + count);
			elementTitle.text = LanguageManager.GetTranslate(objectPrefabs[needNum].title);
			elemenCount.text = count.ToString();
			ActiveSprite(objectPrefabs[needNum].type);
		}
		element.SetActive(false);
		animComponent.SetBool("showElement", true);
		element.SetActive(true);
		element.GetComponent<elementInDailyBonus>().Resize();
		AudioManager.PlayEffect(closeClip, AudioMixerTypes.mapEffect);
		Invoke("Ready", 1);
	}
	
	public void EventNextNum() {
		countElements.text = (allInBox - thusNum).ToString();
	}

	void Ready() {
		nextBonusReady = true;
	}

	void ActiveSprite(ShopElementType spriteAct) {
		foreach(DailyBonusType one in objectPrefabs)
			one.prefab.gameObject.SetActive(one.type == spriteAct);
	}

	void HideAll() {
		animComponent.SetTrigger("hide");
		Invoke("CloseThis", 1);
	}

	void CloseThis() {

		if(dailyBonusCountShow == 1)
			UiController.ShowPushDialog(null);

		gameObject.SetActive(false);
	}

	public static void PushConfirm() {
		int unixTime = (int)(System.DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
		int nextPush = (PlayerPrefs.GetInt("dailyBonus") + 60*60*24)-unixTime;

#if PLUGIN_VOXELBUSTERS
		string notif = Apikbs.CreateLocalNotification(GetDeltaTime(nextPush) , VoxelBusters.NativePlugins.eNotificationRepeatInterval.NONE, "Jack Rover", LanguageManager.GetTranslate("push_DailyBonus"));
		PlayerPrefs.SetString("localPushDailyBonus", notif);
#endif
	}

	public override void BackButton() {
		CloseThis();
	}
}
