using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUi: UiPanel {
  public override void ManagerClose() {
    ClostButton();
  }

  public Animation animComp;

  public RectTransform etalon;
  public RectTransform etalonBorder;
  public RectTransform border;

  public GameObject blueLine;

  public RectTransform allProductsRect;

  public List<RectTransform> productList = new List<RectTransform>();

  private void Start() {
    RecalcSize();
    InitStatus();
  }

  protected override void OnEnable() {
    base.OnEnable();
    RecalcSize();
  }

  private void RecalcSize() {

    productList.ForEach(elem => {
      elem.localScale = Vector3.one;
      elem.localScale = new Vector3(Mathf.Min(etalon.rect.width / elem.sizeDelta.x, 1), Mathf.Min(etalon.rect.width / elem.sizeDelta.x, 1), 1);
    });
    border.localScale = new Vector3(etalonBorder.rect.width / border.sizeDelta.x, etalonBorder.rect.width / border.sizeDelta.x, 1);

  }

  [ExEvent.ExEventHandler(typeof(ExEvent.ShopEvents.OnProductBye))]
  public void OnProductBye(ExEvent.ShopEvents.OnProductBye eventData) {
    InitStatus();
  }

  protected virtual void InitStatus() {
    blueLine.SetActive(Shop.Instance.is30Percent);
    blueLine.GetComponent<RectTransform>().localScale = Vector3.one;

    if (Shop.Instance.isSpecialProduct) {
      blueLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(blueLine.GetComponent<RectTransform>().anchoredPosition.x, -552);
    } else {
      blueLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(blueLine.GetComponent<RectTransform>().anchoredPosition.x, -283);
    }

    if (!Shop.Instance.is30Percent && !Shop.Instance.isSpecialProduct)
      allProductsRect.sizeDelta = new Vector2(allProductsRect.sizeDelta.x, -250);
    else if (!Shop.Instance.isSpecialProduct && Shop.Instance.is30Percent) {
      allProductsRect.sizeDelta = new Vector2(allProductsRect.sizeDelta.x, -250);
    } else if (Shop.Instance.isSpecialProduct && !Shop.Instance.is30Percent) {
      allProductsRect.sizeDelta = new Vector2(allProductsRect.sizeDelta.x, -550);
    } else {
      allProductsRect.sizeDelta = new Vector2(allProductsRect.sizeDelta.x, -736);
    }

  }

  public void ByeProduct(BillingProductAbstract bill, System.Action callback) {

    Shop.Instance.ByeProduct(bill, callback);
  }

  public void ClostButton() {
    Hide(() => {
      isClosing = false;
      gameObject.SetActive(false);
    });
  }

  public override void Show(System.Action OnShow = null) {
    base.Show(OnShow);
    AudioManager.Instance.library.PlayWindowOpenAudio();
    animComp.Play("show");
  }

  public override void Hide(System.Action OnHide = null) {
    base.Hide(OnHide);
    AudioManager.Instance.library.PlayWindowCloseAudio();
    animComp.Play("hide");
  }

}
