using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsWidget: ExEvent.EventBehaviour {

  public Text text;

  public Animation animComponent;
  public Animation blockAnim;
  public CoinsWidgetHelper animHelper;

  public bool changeVisible = true;

  private int _coinsCount = 0;

  private void Start() {
    
    animHelper.OnBound = BaunsComplete;

  }

  private void OnEnable() {

#if UNITY_IOS
    if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX) {
    GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -75);
    }
#endif

    _coinsCount = PlayerManager.Instance.coins;
    text.text = _coinsCount.ToString();
    PlayVisible();
  }

  private void OnDisable() {
    StopAllCoroutines();
  }

  private Coroutine changeProgress;

  [ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.CoinsChange))]
  private void ChangePlayerCount(ExEvent.PlayerEvents.CoinsChange coins) {

    if (!gameObject.activeInHierarchy) return;

    if (changeProgress != null)
      StopCoroutine(changeProgress);
    changeProgress = StartCoroutine(ChangeCoins(_coinsCount, coins.count));
  }

  IEnumerator ChangeCoins(int startCoins, int endCoins) {
    PlayerBauns();
    float delta = (endCoins - startCoins) / 10f;

    delta = delta > 0 ? Mathf.Max(1, delta) : Mathf.Min(-1, delta);
    delta = Mathf.Round(delta);

    while (startCoins != endCoins) {
      startCoins += (int)delta;

      if ((delta > 0 && startCoins > endCoins) || (delta < 0 && startCoins < endCoins))
        startCoins = endCoins;

      AudioManager.Instance.library.PlayCoinsAudio();
      text.text = startCoins.ToString();
      yield return new WaitForSeconds(0.05f);
    }

    _coinsCount = endCoins;

  }

  public bool isVisible;
  private bool isBaunsEffect;

  public void ShowWidget() {

    if (!isVisible) {
      PlayVisible();
    }

    isVisible = true;
  }

  public void HideWidget() {

    if (isVisible && !isBaunsEffect) {
      PlayHide();
    }

    isVisible = false;
  }

  public void AddCoins() {
    //PlayerBauns();
  }

  public void BaunsComplete() {
    isBaunsEffect = false;

    if (!isVisible) {
      PlayHide();
    }
  }

  private void PlayVisible() {
    if (animComponent == null || !changeVisible) return;
    animComponent.Play("show");
  }

  private void PlayHide() {
    if (animComponent == null || !changeVisible) return;
    animComponent.Play("hide");
  }

  public void PlayerBauns() {
    if (!gameObject.activeInHierarchy) return;
    isBaunsEffect = true;
    blockAnim.Play("bauns");
  }

}
