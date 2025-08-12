using System;
using System.Collections.Generic;
using UnityEngine;

public class GameLetter: MonoBehaviour {

  public Action<int> OnClick;

  public Animation anim;

  public Sprite defaultBack;
  public Sprite bonusBack;

  public SpriteRenderer backGraphic;
  public SpriteRenderer activeGraphic;
  public SpriteRenderer alphaRender;
  public List<Sprite> spriteList;
  public Crossword.Letter crosswordLetter;

  private string _letter;
  private int _num;

  private bool _isHint;
  public bool isHint { get { return _isHint; } }

  public bool _isCoin;

  public bool isCoin {
    get { return _isCoin; }
    set {
      _isCoin = value;
      coin.gameObject.SetActive(_isCoin);
    }
  }
  public SpriteRenderer coin;

  public string letter {
    get { return _letter; }
  }

  public bool isOpen = false;

  private void OnEnable() {

    backGraphic.sprite = PlayerManager.Instance.company.isBonusLevel ? bonusBack : defaultBack;

    isOpen = false;
    backGraphic.gameObject.SetActive(false);
    alphaRender.gameObject.SetActive(false);

  }

  public void SetData(int num, string letter) {
    _num = num;
    _letter = letter;
    _isHint = false;
    alphaRender.sprite = spriteList.Find(x => x.name == letter.ToUpper());

  }

  public void SetStatus(bool isOpen, bool isAnim = false, bool isAutoDeactive = false) {

    if (this.isOpen) return;

    this.isOpen = isOpen;

    if (_isCoin) {
      if (this.isOpen)
        isCoin = false;
      if (!isAutoDeactive) {
        var panel = UIManager.Instance.GetPanel<PlayGamePlay>();
        panel.StartCoinsMover(transform.position);
      }
    }

    if (isAnim)
      Animated();
  }

  public void Animated(bool isFirst = false) {

    if (isFirst) {
      backGraphic.gameObject.SetActive(true);
      PlayOpenLetterAudio();

      transform.localScale = Vector3.one;
      anim.Play("firstShow");
      return;
    }

    if (isOpen)
      ShowAnim();

    if (_isHint)
      SetHint(false);

  }

  public void SetHint(bool isFirst) {
    if (this.isOpen) return;
    _isHint = true;


    if (!isFirst) {
      PlayOpenLetterAudio();
      anim.Play("hint");
    }
  }

  private void OnDisable() {
    SetStatus(false, false, true);
  }

  public void Click() {
    if (OnClick != null) OnClick(_num);
  }

  public void ShowAnim() {
    PlayOpenLetterAudio();
    anim.Play(_isHint ? "showHint" : "show");
  }

  public List<AudioClipData> openLetterAudio;
  public void PlayOpenLetterAudio() {
    AudioManager.PlayEffects(openLetterAudio[UnityEngine.Random.Range(0, openLetterAudio.Count)], AudioMixerTypes.effectUi);
  }

}
