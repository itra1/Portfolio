using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingBird.AdMob;
using ExEvent;

public class GameBehaviour: Singleton<GameBehaviour> {

  //public BezierManager bezierManager;					// Контроллер кривой
  public LetterSelector letterSelector;

  public GamePhase gamePhase;                 // Контроллер текущей фазы

  private int _hearts = 3;                    // Количество сердец

  public int hearts {
    get { return _hearts; }
    set {
      _hearts = value;
      GameEvents.OnChangeHeart.Call(_hearts);
    }
  }

  public List<AlphaFloatBehaviour> selectList = new List<AlphaFloatBehaviour>();
  Vector3 _lastPosition = Vector3.zero;

  private bool _isWord;

  [ExEventHandler(typeof(GameEvents.OnPointerDown))]
  void OnPointerDown(GameEvents.OnPointerDown pointDown) {
    if (_isWord) return;
    _isWord = true;
    selectList.Clear();
    _lastPosition = pointDown.pointPosition;
    //bezierManager.StartDrag(pointDown.pointPosition);
    letterSelector.StartDrag(pointDown.pointPosition);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
  public void OnEndGame(ExEvent.GameEvents.OnChangeGamePhase eg) {
    if (eg.last == GamePhase.game && eg.next != GamePhase.game && _isWord) {
      _isWord = false;
    }
  }

  [ExEventHandler(typeof(GameEvents.OnPointerUp))]
  void OnPointerUp(GameEvents.OnPointerUp pointUp) {
    if (!_isWord) return;
    ReadWord(true);
    _lastPosition = pointUp.pointPosition;
    //bezierManager.ClearPoint();
    letterSelector.HideAll();
    _isWord = false;
  }

  [ExEventHandler(typeof(GameEvents.OnPointerDrag))]
  void OnPointerDrag(GameEvents.OnPointerDrag pointDrag) {
    if (!_isWord) return;
    _lastPosition = pointDrag.pointPosition;
    //bezierManager.ChangePosition(pointDrag.pointPosition);
    letterSelector.ChangePosition(pointDrag.pointPosition);
  }

  [ExEventHandler(typeof(GameEvents.OnAlphaEnter))]
  void OnAlphaEnter(GameEvents.OnAlphaEnter alphaEnter) {
    if (!_isWord) return;
    if (!SelectAlpha(alphaEnter.alphaBehaviour))
      //bezierManager.AddPoint(alphaEnter.pointPosition);
      letterSelector.AddPoint(alphaEnter.pointPosition);
  }

  [ExEventHandler(typeof(GameEvents.OnAlphaDown))]
  void OnAlphaDown(GameEvents.OnAlphaDown alphaDown) {
    GameEvents.OnPointerDown.Call(alphaDown.pointPosition);
    GameEvents.OnAlphaEnter.Call(alphaDown.pointPosition, alphaDown.alphaBehaviour);
  }

  // Проверка на возможность выбрать букву
  bool SelectAlpha(AlphaFloatBehaviour alphaBeh) {

    int? useNum = null;
    for (int i = 0; i < selectList.Count; i++) {
      if (selectList[i] == alphaBeh)
        useNum = i;
    }

    if (useNum == null) {
      selectList.Add(alphaBeh);
      ReadWord();
      return false;
    }

    if (useNum == selectList.Count - 2) {
      //bezierManager.TrimList((int)useNum + 2);
      letterSelector.TrimList((int)useNum + 1);
      selectList.RemoveRange((int)useNum + 1, selectList.Count - ((int)useNum + 1));
      ReadWord();
    }

    return true;
  }

  // Чтение набора выбранных букв
  void ReadWord(bool isClose = false) {

    string word = "";
    selectList.ForEach(x => word += x.alpha);

    if (word.Length == 0) return;

    ExEvent.GameEvents.OnChangeWord.Call(word);

    if (isClose)
      PlayerManager.Instance.company.ReadWord(word);

  }

  #region RewardVideo

  private DateTime _lastShowReward = DateTime.MinValue;

  private int _lostCount = 0;
  private int _showCount = 0;

  [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSelect))]
  public void OnSelectWord(ExEvent.GameEvents.OnWordSelect selectWord) {

    switch (selectWord.select) {
      case SelectWord.repeat:
      case SelectWord.no:
      case SelectWord.specialRepeat:
        _lostCount++;
        break;
      default:
        _lostCount = 0;
        break;
    }

    if (_lostCount >= 5)
      ShowDialogReward();

  }

  private void ShowDialogReward() {

    // Реклама еще не загружена
    Debug.Log(AdMobManager.Instance.IsRewardedReady());

    if (PlayerManager.Instance.company.isBonusLevel) return;

    if (!AdMobManager.Instance.IsRewardedReady()) return;
    // последний показ меньше 10 минут
    if (_showCount >= 3 && (DateTime.Now - _lastShowReward).TotalMinutes <= 10) return;
    // 3 раза уже показали
    //if (_showCount > 3) return;
    // Присутствует хоть одна подсказка
    if (PlayerManager.Instance.hintAnyLetter > 0 || PlayerManager.Instance.hintFirstLetter > 0 || PlayerManager.Instance.hintFirstWord > 0) return;

    _showCount++;
    _lostCount = 0;
    _lastShowReward = DateTime.Now;

    RewardVideoQuestion panel = UIManager.Instance.GetPanel<RewardVideoQuestion>();
    panel.gameObject.SetActive(true);

    if (_showCount == 1)
      panel.SetHint(GiftElement.Type.hintLetter);
    else
      panel.SetHint(GiftElement.Type.hintAnyletter);

    panel.OnOk = () => {

      panel.CancelButton();

      GoogleAdsMobile.Instance.ShowRewardVideo(
        (type, amount) => {

          Debug.Log("Reward success");

          RewardVideoHint gp = UIManager.Instance.GetPanel<RewardVideoHint>();

          gp.gameObject.SetActive(true);
          //gp.ShowGift(giftParam, OnConplete);


          if (_showCount == 1) {
            PlayerManager.Instance.hintFirstLetter++;
            gp.ShowGift(new GiftElement.GiftElementParam() { type = GiftElement.Type.hintLetter }, () => {
              //panel.CancelButton();
              gp.gameObject.SetActive(false);
            });
          } else {
            PlayerManager.Instance.hintAnyLetter++;
            gp.ShowGift(new GiftElement.GiftElementParam() { type = GiftElement.Type.hintAnyletter }, () => {
              //panel.CancelButton();
              gp.gameObject.SetActive(false);
            });
          }

        },
        (err) => {
          Debug.Log("Reward err");

        }
      );

    };

    panel.OnCancel = () => { };

  }

  #endregion


}
