using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


/// <summary>
/// Панель паузы
/// </summary>
public class PausePanel : ScrollingMenu {

	public Action OnHome;
	public Action OnContinue;

	RunnerController runner;
	
	public GameObject[] leftDecor;
	public GameObject[] rightDecor;

	public Text distanceText;                           // Дистанция
	public Text coinsText;                              // Монет
	public Text cristallText;                           // Кристалов
	public Text blackMarkText;                          // Черных меток

	public GameObject FBAuthPanel;                      // ПАнель авторизации фейсбука
	public GameObject RatingPanel;                      // Панель лучших

	private bool deactiveReady;                                 // Флаг готовности деактивироваться
	public AudioClip clickClip;
	//public int incrementCoins;

	bool isContinue;
	
	void Start() {
		//menu.SetActive(false);
		contentPanel.gameObject.SetActive(false);
		//backUi.SetActive(false);
	}

	protected override void OnEnable() {
		base.OnEnable();
		deactiveReady = false;
		//menu.SetActive(false);
		isContinue = false;

		runner = RunnerController.Instance;

		FacebookAuth();

		SetDecor();

		int dist = (int)runner.thisDistantionPosition;
		distanceText.text = dist.ToString() + " M";
		coinsText.text = runner.moneyInRace.ToString();
		cristallText.text = runner.cristallInRace.ToString();
		blackMarkText.text = runner.blackPointsInRace.ToString();
		ChangeLanuage();
	}

	protected override void OnDisable() {
		base.OnDisable();
		if (OnClose != null)
			OnClose();
	}

	void NoPause() {
		GetComponent<Animator>().SetTrigger("hide");
		runner.Pause();
	}

	void SetDecor() {
		int leftNum = Random.Range(0, leftDecor.Length);
		for (int i = 0; i < leftDecor.Length; i++)
			leftDecor[i].SetActive(i == leftNum);

		int rightNum = Random.Range(0, rightDecor.Length);
		for (int i = 0; i < rightDecor.Length; i++)
			rightDecor[i].SetActive(i == rightNum);
	}

	public void Show(bool anim) {
		if (anim) {
			GetComponent<Animator>().SetTrigger("showPausePanel");
		} else {
			GetComponent<Animator>().SetTrigger("showPausePanelSpeed");
		}
	}

	// Снимаем с паузы
	public void ContinueButton() {

		UiController.ClickButtonAudio();
		deactiveReady = true;
		isContinue = true;
		GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().SetTrigger("hidePausePanel");
		
		if (OnContinue != null)
			OnContinue();

	}

	public void ButtomHome() {
		
		UiController.ClickButtonAudio();

		if (OnHome != null)
			OnHome();
	}

	public void animEvent() {
		if (deactiveReady)
			gameObject.SetActive(false);
	}

	#region Facebook

	public void FBLogin() {
#if PLUGIN_FACEBOOK
		UiController.ClickButtonAudio();
		if (FBController.instance != null) {
			FBController.instance.FBlogin(FacebookAuth);
		}
#endif
	}

	// калбак авторизации
	void FacebookAuth() {
		if (FBController.CheckFbLogin)
			GetComponent<Animator>().SetBool("fbAuth", true);
		else
			GetComponent<Animator>().SetBool("fbAuth", false);
	}

	// Инвайт друзей из ФБ
	public void FbInvate() {
		UiController.ClickButtonAudio();
#if PLUGIN_FACEBOOK
		FBController.instance.InvateFriendFb();
#endif
	}
	#endregion

	public void AcceptCoins() {
		//runner.AddRaceCoins(incrementCoins);
		coinsText.text = runner.moneyInRace.ToString();
		//incrementCoins = 0;
	}

	public void ChangeDoubleJump() {
		RunnerController.Instance.doubleJump = !RunnerController.Instance.doubleJump;
	}

	public void ReserInvates() {
		Apikbs.Instance.ResetInvates();
	}

	#region Lanuage

	public Text homeBtnText;
	public Text continueBtnText;
	public Text contentTitleQuestion;
	public Text contentTitleRating;
	public Text questionInfoText;
	public Text friendsInfoText;
	public Text loginFb;

	void ChangeLanuage() {
		homeBtnText.text = LanguageManager.GetTranslate("pause_Home");
		continueBtnText.text = LanguageManager.GetTranslate("pause_Continue");
		contentTitleQuestion.text = LanguageManager.GetTranslate("pause_Question");
		contentTitleRating.text = LanguageManager.GetTranslate("pause_Ratings");
		questionInfoText.text = LanguageManager.GetTranslate("pause_QuestonInfo");
		friendsInfoText.text = LanguageManager.GetTranslate("pause_FriendsInfo");
		loginFb.text = LanguageManager.GetTranslate("pause_LoginFb");
	}

	public override void BackButton() {
		ButtomHome();
	}

	#endregion

}
