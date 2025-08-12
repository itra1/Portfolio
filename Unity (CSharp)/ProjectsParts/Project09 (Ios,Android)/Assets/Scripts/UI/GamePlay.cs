using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Интерфейс игрового процесса
/// </summary>
public class GamePlay : MonoBehaviour {

  public PlayersStats firstPlayer;                            // Состояние первого плеера
  public PlayersStats secondPlayer;                           // Состояние второго плеера
  public GameObject statsPanel;                               // Панель состояний игроков
  public GameObject winnerPanel;                              // Панель победителя
  public GameObject lostPanel;                                // Панель победителя
  public GameObject drandPanel;                               // Панель победителя
  public GameObject energyDelimetr;                           // Префаб делителя панелей энергии
  public GameObject starPrefab;                               // Префаб звезды на панели жизней
  public Text pingText;

	BattlePhase battlePhase;

	void Start() {
    InitCards();
    winnerPanel.SetActive(false);
  }

  void OnEnable() {
    BattleScene.OnChangeBattleTimer += SetTimerBattle;
    PlayersManager.PlayerCreated += PlayerCreated;
    BattleScene.OnBattleEnd += OnBattleEnd;
		BattleScene.ChangeBattlePhase += ChangeBattlePhase;
		CardManager.OnCreateCard += OnCreateCard;
    Generals.Network.NetworkManager.OnPingChange += OnPingChange;
		lastTimeStart = 0;


		if(BattleScene.instance != null) {
      SetTimerBattle(BattleScene.instance.timeEndBattle);
			ChangeBattlePhase(BattleScene.instance.battlePhase);
		}
  }

  void OnDisable() {
    BattleScene.OnChangeBattleTimer -= SetTimerBattle;
    PlayersManager.PlayerCreated -= PlayerCreated;
		BattleScene.ChangeBattlePhase -= ChangeBattlePhase;
		BattleScene.OnBattleEnd -= OnBattleEnd;
    CardManager.OnCreateCard -= OnCreateCard;
    Generals.Network.NetworkManager.OnPingChange -= OnPingChange;
  }

  void OnDestroy() { }

  void OnPingChange(int num) {
    pingText.text = num.ToString();
  }

  void Update() {
    firstPlayer.Update();
    secondPlayer.Update();
  }

  void OnBattleEnd(TeamResult result) {

    if(result.type == TeamResult.ResultType.draw) {
      ShowDrawnGame();
    } else if(result.type == TeamResult.ResultType.win) {
      YouWinner();
    } else {
      YouLost();
    }
  }

  /// <summary>
  /// Событие добавления плеера на сцену
  /// </summary>
  /// <param name="playerType">Тип плеера</param>
  /// <param name="playerParametrs">Параметры плеера</param>
  void PlayerCreated(Player player) {

    PlayersStats statPlayer = new PlayersStats();

    statPlayer = player.isFirst ? firstPlayer : secondPlayer;
    statPlayer.Init(player);
    statPlayer.SetStarsDelimetr(starPrefab);
    statPlayer.SetEnergyDelimetr(energyDelimetr);
  }

  /// <summary>
  /// Победа
  /// </summary>
  /// <param name="newWinnerName"></param>
  public void YouWinner() {
    winnerPanel.SetActive(true);
  }

  /// <summary>
  /// Проигрыш
  /// </summary>
  /// <param name="newWinnerName"></param>
  public void YouLost() {
    lostPanel.SetActive(true);
  }

  /// <summary>
  /// Показать победителя
  /// </summary>
  /// <param name="newWinnerName"></param>
  public void ShowDrawnGame() {
    drandPanel.SetActive(true);
  }


	void ChangeBattlePhase(BattlePhase battlePhase) {
		this.battlePhase = battlePhase;

		if(this.battlePhase == BattlePhase.ready || this.battlePhase == BattlePhase.load) {
			timerPanel.SetActive(true);
		} else {
			timerPanel.SetActive(false);
		}

	}

	#region Таймер старта
	
	public GameObject timerPanel;
	public Text timerPanelTime;
	public Animator timerPanelAnim;

	int lastTimeStart = 0;

	void TimerStart(float newTimerEndValue) {

		if(lastTimeStart != (int)Mathf.Floor(newTimerEndValue)) {
			lastTimeStart = (int)Mathf.Floor(newTimerEndValue);
			if(lastTimeStart != 0)
				timerPanelTime.text = lastTimeStart.ToString();
			else
				timerPanelTime.text = "GO!";
		}
	}
	
	#endregion

	#region Таймер

	public Text timerText;          // Интерфейс таймера 

  /// <summary>
  /// Установка текущего значения таймера
  /// </summary>
  /// <param name="newTimerValue">Новое значение таймера</param>
  public void SetTimerBattle(float newTimerEndValue) {

		if(this.battlePhase == BattlePhase.ready || this.battlePhase == BattlePhase.load) {
			TimerStart(newTimerEndValue);
		} 
		timerText.text = string.Format("{0}:{1:d2}", (int)Mathf.Floor(newTimerEndValue / 60), (int)Mathf.Floor(newTimerEndValue % 60));

	}

  #endregion

  #region Карты
  List<GameObject> cardList = new List<GameObject>();
  public GameObject cardsPanel;                             // Панель карт
  public GameObject cardGuiPrefab;                          // Префаб одной карты
  public int cardsMaxDisplay;                               // Количество карт на экране

  void OnCreateCard(List<CardInfo> newCard) {
    cardList.ForEach(x => Destroy(x));
    cardList.Clear();
    foreach(CardInfo oneCard in newCard) {
      CardGenerate(oneCard);
    }
  }

  /// <summary>
  /// Первоначальная инициализация карт
  /// </summary>
  void InitCards() {
    Vector2 sizeDelta = cardsPanel.GetComponent<RectTransform>().sizeDelta;
    sizeDelta = new Vector2(cardsMaxDisplay * 100 + (cardsMaxDisplay - 1) * 10, sizeDelta.y);
    cardsPanel.GetComponent<RectTransform>().sizeDelta = sizeDelta;
  }

  /// <summary>
  /// Генерация новой карты
  /// </summary>
  /// <param name="localPositionGenerate"></param>
  void CardGenerate(CardInfo newCard) {
    GameObject instCard = Instantiate(cardGuiPrefab);
    instCard.transform.SetParent(cardsPanel.transform);
    instCard.transform.localScale = Vector3.one;
    instCard.GetComponent<Card>().Init(newCard);
    instCard.GetComponent<Card>().TapCard = CardTap;
    cardList.Add(instCard);
    UpdatePosition();
  }

  /// <summary>
  /// Обработка события тапа по карте
  /// </summary>
  /// <param name="cardType"></param>
  void CardTap(CardInfo cardInfo) {
    CardManager.instance.UseCardTry(cardInfo);
  }

  /// <summary>
  /// Устанвока позиционирования карт
  /// </summary>
  void UpdatePosition() {
    Vector2 sizeDelta = cardsPanel.GetComponent<RectTransform>().sizeDelta;
    for(int i = 0; i < cardList.Count; i++) {
      cardList[i].transform.localPosition = new Vector3(-(sizeDelta.x / 2) + 50 + (i * 100 + (i > 0 ? i * 10 : 0)), sizeDelta.y / 2, 0);
    }
  }

  #endregion

  /// <summary>
  /// Параметры игрока на геймплее
  /// </summary>
  [System.Serializable]
  public class PlayersStats {

    //[HideInInspector]
    public Player player;

    [SerializeField]
    private Text nameText;                                      // Интерфейское отображение имени
    string _name;                                               // Текущее имя персонажа

    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string name {
      set {
        _name = value;
        nameText.text = _name;
      }
      get { return _name; }
    }

    [SerializeField]
    private Text weaponCountText;                               // Запас оружия

    public int weaponCount {
      set { weaponCountText.text = value.ToString(); }
    }

    float weaponTimeReaload = 1;
    float weaponTimeShoot = 1;

    public void Init(Player player) {
      this.player = player;
      ChangePlayer(this.player);
      player.OnChangePlayer += ChangePlayer;
      name = player.playerName;
    }

    public void SetStarsDelimetr( GameObject starPrefab ) {
      float deltaSizeX = player.isFirst ? energyValueLine.sizeDelta.x : energyValueLine.sizeDelta.x;
      // Размещение звезд на полосе жизней
      for(int i = 0; i < PlayersManager.instance.starsParam.Length; i++) {
        GameObject oneStar = Instantiate(starPrefab);
        oneStar.transform.SetParent(healthValueLine.parent);
        oneStar.transform.localScale = Vector3.one;
        oneStar.transform.localPosition = new Vector3((deltaSizeX - PlayersManager.instance.starsParam[i] * deltaSizeX) * (player.isFirst ? 1 : -1), 0, 0);
        oneStar.SetActive(true);
        starLive.Add(oneStar);
      }
    }

    public void SetEnergyDelimetr(GameObject energyDelimetr) {
      float deltaSizeX = player.isFirst ? energyValueLine.sizeDelta.x : energyValueLine.sizeDelta.x;

      // Размещение делителей на полосе энергии
      for(int i = 0; i < player.energyMax - 1; i++) {
        GameObject oneDel = Instantiate(energyDelimetr);
        oneDel.transform.SetParent(energyValueLine.parent);
        oneDel.transform.localScale = Vector3.one;
        oneDel.transform.localPosition = new Vector3((i + 1) * (deltaSizeX / (int)player.energyMax) * (player.isFirst ? 1 : -1), 0, 0);
        oneDel.SetActive(true);
      }
    }
    
    /// <summary>
    /// Событие изменения состояния компонента оружия
    /// </summary>
    /// <param name="weaponManager">Менеждер используемого оружия</param>
    public void ChangeWeapon(WeaponManager weaponManager) {

      if(weaponManager == null) {
        weaponCount = 0;
        return;
      }

      if(weaponManager.bulletCount > 0 && weaponManager.timeReload != 0 && weaponManager.timeShoot != 0) {
        weaponTimeReaload = weaponManager.timeReload;
        weaponTimeShoot = weaponManager.timeShoot;
      } else {
        weaponTimeReaload = 1;
        weaponTimeShoot = 1;
      }

      weaponCount = weaponManager.bulletCount;
    }

    private void WeaponUpdate() {
      if(weaponTimeReaload != 0 && weaponTimeShoot != 0)
        weaponValueLine.localScale = new Vector3(Mathf.Min((Time.time - weaponTimeShoot) / weaponTimeReaload, 1), weaponValueLine.localScale.y, weaponValueLine.localScale.z);
    }

    public RectTransform healthValueLine;                       // Полоса жизней
    public RectTransform energyValueLine;                       // Полоса энергии
    public RectTransform weaponValueLine;                       // Время готовности оружия

    /// <summary>
    /// Звезды жизней
    /// </summary>
    public List<GameObject> starLive = new List<GameObject>();
    float? openStar;

    public void ChangePlayer(Player player) {

      // Корректируем состояние жизней
      healthValueLine.localScale = new Vector3(player.healthValue / player.healthMax, healthValueLine.localScale.y, healthValueLine.localScale.z);
      float valStar = 1 - player.healthValue/player.healthMax;
      for(int i = 0; i < PlayersManager.instance.starsParam.Length; i++) {
        if(valStar >= PlayersManager.instance.starsParam[i] && (openStar == null || valStar < i)) {
          openStar = i;
          starLive[i].transform.localScale = Vector3.one * 2;
        }
      }

      // Корректируем состояние энергии
      energyValueLine.localScale = new Vector3(player.energyValue / player.energyMax, energyValueLine.localScale.y, energyValueLine.localScale.z);

      ChangeWeapon(player.weaponManager);
    }

    /// <summary>
    /// Проверка присутствия достаточного количества энергии
    /// </summary>
    /// <param name="energyValueCheck"></param>
    /// <returns></returns>
    public bool CheckEnergyExists(float energyValueCheck) {
      return player.energyValue >= energyValueCheck;
    }

    public void Update() {
      WeaponUpdate();
    }
  }
}

