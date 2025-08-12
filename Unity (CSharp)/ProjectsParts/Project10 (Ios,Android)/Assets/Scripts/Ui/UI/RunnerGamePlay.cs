using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Контроллер интерфейска игрового процесса
/// </summary>
public class RunnerGamePlay : PanelUi {

	public Animation animationComponent;

	public GameObject controlOcject;
	public GameObject pauseObject;
	public GameObject statObject;
	public GameObject armorPanel;
	public GameObject powerPanel;
	public GameObject weaponObject;
	
	public Text distantionCount;
	
	private RunnerPhase runnerPhase;

	private float timeWait;

	private Animator animComp;

	public static RunnerGamePlay instance;

	private void Start() {
		instance = this;
	}

	protected override void OnEnable() {
		base.OnEnable();

		animationComponent.Play("show");
		
		animComp = GetComponent<Animator>();
		RunnerController.OnChangeRunnerPhase += ChangePhase;

		if (RunnerController.Instance.activeLevel == ActiveLevelType.ship) {
			if (powerPanel != null) powerPanel.SetActive(true);
		} else {
			if (powerPanel != null) powerPanel.SetActive(false);
		}
		
	}

	void Update() {
		friendsUpdate();
	}


	protected override void OnDisable() {
		base.OnDisable();

		animationComponent.Play("hide");
		
		RunnerController.OnChangeRunnerPhase -= ChangePhase;
		HideElements();
	}

	public void ChangePhase(RunnerPhase phase) {
		//if (phase == RunnerPhase.run) timeStartRun = Time.time;
	}
	
	public void HideElements() {
		controlOcject.SetActive(false);
		pauseObject.SetActive(false);
		statObject.SetActive(false);
		weaponObject.SetActive(false);
		armorPanel.SetActive(false);
	}
	

	public void ButtomPause() {
		UiController.ClickButtonAudio();
		RunnerController.Instance.Pause(true);
	}
	
	#region Ближайшие друзья
	[Header("Ближайший друз")]
	public GameObject nextFriendPanel;
	public GameObject arrowFriend;
	public Image photoFriend;
	public Text friendDistance;
	int nextFriend;


	public static void SetFriend(Texture photo, int distance) {

		if (!instance.nextFriendPanel.activeInHierarchy)
			instance.nextFriendPanel.SetActive(true);

		instance.nextFriend = distance;
		instance.friendDistance.text = distance.ToString();
		instance.photoFriend.sprite = Sprite.Create((Texture2D)photo, new Rect(0, 0, 48, 48), new Vector2(0.5f, 0.5f));

	}

	int dustantion;

	void friendsUpdate() {

		if (nextFriend <= RunnerController.playerDistantion) return;

		dustantion = (int)Mathf.Round((nextFriend - RunnerController.playerDistantion));
		friendDistance.text = dustantion.ToString();

		if (dustantion <= 1)
			nextFriendPanel.SetActive(false);
		else if (!nextFriendPanel.activeInHierarchy)
			nextFriendPanel.SetActive(true);

	}

	#endregion
	
	#region Сила
	[Header("Сила")]
	public GameObject powerLine;
	float powerMaximum;

	public static void SetPowerValue(float maxValue, float value) {
		if (instance == null) return;

		value = (maxValue < value ? maxValue : value);
		value = (value <= 0 ? 0 : value);
		instance.powerLine.transform.localScale = new Vector3(value / maxValue, 1, 1);
	}

	#endregion
	
	#region Появление бонусов

	[SerializeField]
	AudioClip bonusClip;
	public static void PlayBonusClip() {
		AudioManager.PlayEffect(instance.bonusClip, AudioMixerTypes.runnerEffect);
	}

	public override void BackButton() {
		ButtomPause();
	}

	#endregion

}
