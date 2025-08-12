using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;
using UnityEngine.UI;

public class PlayGamePlay : UiPanel, ICoinsPanelParent, IMiniMenuParent {

	public GameObject alphaRegionPrefab;
	public Animation animComponent;
	public WordTranslateUi wordTranslateUi;
	public WordUseUi textWord;

	private string _showAnim = "show";
	private string _hideAnim = "hide";

	private bool _endGameReady;   // Ожидание окончания боя

	public BonusTimer bonusTimer;

	public GameObject pauseButton;
	public GameObject friendButton;
	public GameObject _gameMiniMenu;
	public GameObject gameMiniMenu { get { return _gameMiniMenu; } }

	public GameMiniMenu _miniMenu;
	public GameMiniMenu miniMenu { get { return _miniMenu; } }

	public GameObject tutorialHelper;

	public LocalizationUiText tutorHelperTest;

	public Canvas _coinsPanel;
	public Canvas coinsPanel { get { return _coinsPanel; } }

	public Canvas gameMiniMenyPanel;
	private int _coinsPanelStartOrder;
	private int _gameMiniMenuOrder;

	protected override void Awake() {
		base.Awake();
		_coinsPanelStartOrder = coinsPanel.sortingOrder;
		_gameMiniMenuOrder = gameMiniMenyPanel.sortingOrder;
	}
	
	protected override void OnEnable() {
		base.OnEnable();
		Show();
		miniMenu.Close();
		_endGameReady = false;

		pauseButton.gameObject.SetActive(!Tutorial.Instance.isTutorial);
		conchIconParent.gameObject.SetActive(!Tutorial.Instance.isTutorial && !PlayerManager.Instance.company.isBonusLevel);
		coinsWidget.gameObject.SetActive(!Tutorial.Instance.isTutorial && !PlayerManager.Instance.company.isBonusLevel);
		friendButton.gameObject.SetActive(!Tutorial.Instance.isTutorial && !PlayerManager.Instance.company.isBonusLevel);
		gameMiniMenu.gameObject.SetActive(!Tutorial.Instance.isTutorial && !PlayerManager.Instance.company.isBonusLevel);
		tutorialHelper.gameObject.SetActive(Tutorial.Instance.isTutorial);

		if (PlayerManager.Instance.company.isBonusLevel) {
			bonusTimer.SetShow();
		}
		else {
			bonusTimer.SetHide();
		}
		bonusTimer.StopTimer();
	}

	protected override void OnDisable() {
		base.OnDisable();
		conchMoverList.ForEach(x=>x.gameObject.SetActive(false));
		coinsMoverList.ForEach(x => x.gameObject.SetActive(false));

		coinsPanel.sortingOrder = _coinsPanelStartOrder;
		gameMiniMenyPanel.sortingOrder = _gameMiniMenuOrder;
	}
	
	[ExEventHandler(typeof(ExEvent.GameEvents.OnLetterLoaded))]
	private void OnLetterLoaded(ExEvent.GameEvents.OnLetterLoaded compl) {
		if (PlayerManager.Instance.company.isBonusLevel)
			bonusTimer.StartTimer();

	}

	[ExEventHandler(typeof(ExEvent.GameEvents.OnDailyBonusResume))]
	private void OnDailyBonusResume(ExEvent.GameEvents.OnDailyBonusResume compl) {

			bonusTimer.ResumeTimer();

	}

	[ExEventHandler(typeof(ExEvent.GameEvents.OnAlphaEnter))]
	private void OnAlphaEnter(ExEvent.GameEvents.OnAlphaEnter compl) {
		miniMenu.Close();
	}
	
	public void PauseButton() {
		AudioManager.Instance.library.PlayClickAudio();

		if (PlayerManager.Instance.company.isBonusLevel) {

			DailyBonusCancelUi panel = (DailyBonusCancelUi) UIManager.Instance.GetPanel(UiType.dailyBonusCancel);
			panel.gameObject.SetActive(true);
			panel.OnCancel = () => {
				panel.OnCancel = null;
			};
			panel.OnExit = () => {
				panel.OnExit = null;
				GameManager.Instance.BackToLevels(() => {
					panel.gameObject.SetActive(false);
				});
			};
			return;
		}
		PlayerManager.Instance.Save();
		GameManager.Instance.BackToLevels();
	}

	public void ShopButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GameManager.Instance.Shop();
	}

	public void FriendsButton() {
		AudioManager.Instance.library.PlayClickAudio();
		GetComponent<Sharing>().Share("");
	}

	public void CreateRegion(Vector3 position, Action OnDown, Action OnEnter) {
		GameObject inst = Instantiate(alphaRegionPrefab);
		inst.transform.SetParent(transform);
		inst.gameObject.SetActive(true);
		inst.GetComponent<RectTransform>().position = (Vector2)Camera.main.WorldToScreenPoint(position);
		AlphaRegionUi alphaRegionUi = inst.GetComponent<AlphaRegionUi>();
		alphaRegionUi.DownEvent = OnDown;
		alphaRegionUi.EnterEvent = OnEnter;
	}

	public void EndGame(BattlePhase phase) {
		GameManager.Instance.EndGame(phase, PlayerManager.Instance.company.GetActualLevel(), PlayerManager.Instance.company.GetSaveLevel());
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBattleChangePhase))]
	public void OnEndGame(ExEvent.GameEvents.OnBattleChangePhase eg) {
		if (_endGameReady) return;
		if (eg.phase != BattlePhase.game)
			StartCoroutine(ReadyEnd(eg.phase));
	}

	public IEnumerator ReadyEnd(BattlePhase gp) {
		_endGameReady = true;

		GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
		if(gp != BattlePhase.full)
			word.PlaySalut();

		if (!Tutorial.Instance.isTutorial && !PlayerManager.Instance.company.isBonusLevel)
			OctopusController.Instance.PlayHide(() => { });

		yield return new WaitForSeconds(1f);
		word.StopSalut();
		EndGame(gp);
		textWord.ClearText();
		_endGameReady = false;
	}

	#region Летающая иконка ракушки

	public RectTransform conchIcon;
	public RectTransform conchIconParent;
	public ConchMover prefabConchMover;
	private List<ConchMover> conchMoverList = new List<ConchMover>();
	public void StartConchMover(Vector3 pos) {
		ConchMover cm = GetInstanceConchMover();
		cm.transform.position = pos;
		cm.gameObject.SetActive(true);
		cm.SetMover(conchIcon, () => {
			conchIcon.GetComponent<ConchUi>().AddNew();
		});
	}

	public void SetCounchPointer(bool isActive, Action OnClickCallback) {
		conchIcon.GetComponent<ConchUi>().SetPointer(isActive, OnClickCallback);
	}

	private ConchMover GetInstanceConchMover() {

		ConchMover cm = conchMoverList.Find(x => !x.gameObject.activeInHierarchy);
		if (cm == null) {
			GameObject inst = Instantiate(prefabConchMover.gameObject);
			inst.transform.SetParent(transform);
			inst.transform.localScale = Vector3.one;
			cm = inst.GetComponent<ConchMover>();
			conchMoverList.Add(cm);
		}
		return cm;
	}

	#endregion

	#region Летающая иконка монеты

	public CoinsWidget _coinsWidget;
	public CoinsWidget coinsWidget {
		get { return _coinsWidget; }
	}
	public RectTransform _coinsTargetIcon;

	public RectTransform coinsTargetIcon {
		get { return _coinsTargetIcon; }
	}

	public CoinsMover prefabCoinsMover;
	private List<CoinsMover> coinsMoverList = new List<CoinsMover>();

	public void StartCoinsMover(Vector3 pos) {
		CoinsMover cm = GetInstanceCoinshMover();
		cm.transform.position = pos;
		cm.gameObject.SetActive(true);
		cm.SetMover(coinsTargetIcon, () => {
			PlayerManager.Instance.coins += UnityEngine.Random.Range(1,4);
		});
	}

	private CoinsMover GetInstanceCoinshMover() {

		CoinsMover cm = coinsMoverList.Find(x => !x.gameObject.activeInHierarchy);
		if (cm == null) {
			GameObject inst = Instantiate(prefabCoinsMover.gameObject);
			inst.transform.SetParent(transform);
			inst.transform.localScale = Vector3.one;
			cm = inst.GetComponent<CoinsMover>();
			coinsMoverList.Add(cm);
		}
		return cm;
	}

	#endregion

	public override void Show(Action OnShow = null) {
		base.Show(OnShow);
		animComponent.Play(_showAnim);
	}

	public override void Hide(Action OnHide = null) {
		base.Hide(OnHide);
		animComponent.Play(_hideAnim);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.ShowWordTranslate))]
	public void ShowWordTranslate(ExEvent.GameEvents.ShowWordTranslate word) {
		wordTranslateUi.ShowData(word.word);
	}

	public AnimationHelper selectWordHelper;

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterReady))]
	public void HintEnyLetterReady(ExEvent.GameEvents.OnHintAnyLetterReady hint) {
		selectWordHelper.gameObject.SetActive(true);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterCompleted))]
	public void HintEnyLetterCompleted(ExEvent.GameEvents.OnHintAnyLetterCompleted hint) {
		//hintLetterIcon.gameObject.SetActive(false);
		selectWordHelper.GetComponent<Animation>().Play("hide");
		selectWordHelper.OnEvent1 = () => {
			selectWordHelper.gameObject.SetActive(false);
		};
	}

	public override void ManagerClose() {
		if (Tutorial.Instance.isTutorial) return;

		PauseButton();

	}

	public void CoinsPanelGiftOpen() {
		coinsPanel.GetComponent<Animation>().Play("show");
		coinsWidget.gameObject.SetActive(true);
		coinsPanel.sortingOrder = 20;
	}

	public void MiniMenyGiftOpen(bool isLocked) {
		gameMiniMenu.gameObject.SetActive(true);
		miniMenu.Open(isLocked);
		gameMiniMenyPanel.sortingOrder = 20;
	}

	public void MiniMenyGiftClose(bool isLocked) {
		miniMenu.Close(isLocked);
	}

	public void FastLevelFinished() {
		PlayerManager.Instance.ForceLevelComplete();
	}




}
