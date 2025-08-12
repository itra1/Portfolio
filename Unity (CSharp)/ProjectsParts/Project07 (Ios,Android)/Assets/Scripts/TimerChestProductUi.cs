using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerChestProductUi : ShopProductUi {

  public GameObject imageObject;
  public RectTransform parentBlock;

  private List<GameObject> recordList = new List<GameObject>();

  public ChestRecordUi recordUiPrefab;

  public Timer timrUi;

  public string uuidChest;

  public Sprite activeSprite;
  public Sprite deactiveSprite;
  public Button buttonBye;
  public Image buttonSprite;

  private DailyChest dailyChest;
  public GameObject timerBlock;

  private bool isEnableInfo = false;

  protected override void OnEnable() {
    base.OnEnable();
    if (priceText != null)
      priceText.text = "ОТКРЫТЬ";
    dailyChest = DailyChestManager.Instance.GetChest(uuidChest);
    timrUi.onComplete = InitTimer;

    Records();

    InitTimer();

  }

  private void InitTimer() {

    buttonBye.interactable = dailyChest.isReady;

    buttonSprite.sprite = dailyChest.isReady ? activeSprite : deactiveSprite;

    timerBlock.SetActive(!dailyChest.isReady);

    if (dailyChest.isReady) {
      return;
    }

    timrUi.StartTimer(dailyChest.timeWait);

  }

  public override void BuyButton() {
    base.BuyButton();

    isEnableInfo = false;
    imageObject.gameObject.SetActive(isEnableInfo);

  }

  public void ClickImage() {
    isEnableInfo = !isEnableInfo;
    imageObject.gameObject.SetActive(isEnableInfo);
  }

  private void Records() {

    recordUiPrefab.gameObject.SetActive(false);

    for (int i = 0; i < recordList.Count; i++)
      Destroy(recordList[i]);

    recordList.Clear();

    ChestProduct chestProduct = (ChestProduct)product;

    chestProduct.items.ForEach(elem => {

      GameObject recObj = Instantiate(recordUiPrefab.gameObject, recordUiPrefab.transform.parent);
      recordList.Add(recObj);
      recObj.transform.SetParent(recordUiPrefab.transform.parent);
      recObj.GetComponent<RectTransform>().sizeDelta = new Vector2(recordUiPrefab.GetComponent<RectTransform>().rect.width, recordUiPrefab.GetComponent<RectTransform>().rect.height);
      recObj.GetComponent<ChestRecordUi>().SetData(elem);
      recObj.SetActive(true);
    });

    parentBlock.sizeDelta = new Vector2(parentBlock.rect.width, 35 * recordList.Count);

  }
}
