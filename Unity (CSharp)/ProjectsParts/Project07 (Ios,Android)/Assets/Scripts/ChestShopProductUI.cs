using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestShopProductUI: ShopProductUi {

  public GameObject imageObject;
  public RectTransform parentBlock;

  private List<GameObject> recordList = new List<GameObject>();

  public ChestRecordUi recordUiPrefab;

  private bool isEnableInfo = false;

  public bool isChest;

  protected override void OnEnable() {
    base.OnEnable();

    Records();

  }

  public override void BuyButton() {

    isEnableInfo = false;
    imageObject.gameObject.SetActive(isEnableInfo);

    if (base.Buy())
    {

      if (SceneManager.GetActiveScene().name == "Base")
      {
        BaseUi bu = UiController.GetUi<BaseUi>();

        BaseUi.BonusType targetBonus = BaseUi.BonusType.coins;

        if (product is CoinsProduct)
          targetBonus = BaseUi.BonusType.coins;
        if (product is EnergyProduct)
          targetBonus = BaseUi.BonusType.energy;

        bu.AddNewItem(icon, targetBonus);
      }
      
    }
    
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

    if (isChest) {
      ChestProduct chestProduct = (ChestProduct)product;

      chestProduct.items.ForEach(elem => {

        GameObject recObj = Instantiate(recordUiPrefab.gameObject, recordUiPrefab.transform.parent);
        recordList.Add(recObj);
        recObj.transform.SetParent(recordUiPrefab.transform.parent);
        recObj.GetComponent<RectTransform>().sizeDelta = new Vector2(recordUiPrefab.GetComponent<RectTransform>().rect.width, recordUiPrefab.GetComponent<RectTransform>().rect.height);
        recObj.GetComponent<ChestRecordUi>().SetData(elem);
        recObj.SetActive(true);
      });
    }

    parentBlock.sizeDelta = new Vector2(parentBlock.rect.width, 35* recordList.Count);

  }

}
