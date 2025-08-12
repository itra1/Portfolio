using System.Collections.Generic;
using UnityEngine;
using Game.User;

public class Shop: UiDialog {

  [SerializeField]
  private TMPro.TextMeshProUGUI coinsCount;
  public List<ShopPage> pages;
  public ShopLibrary library;
  public ShopByeDialog changeDialog;
  public Animation mainAnimation;

  private readonly string _hideAnim = "ShopHide";
  private readonly string _showAnim = "ShopShow";
  private List<RectTransform> pagesList = new List<RectTransform>();
  //private int pageActive = 0;

  void OnEnable() {
    changeDialog.gameObject.SetActive(false);
    OnCoinsChange(null);
    //coinsCount.text = User.Instance.Coins.ToString();
    //FillDataProducts();
    InitPages();
    SetActivePage(pages[0]);
    mainAnimation.Play(_showAnim);
  }

  void OnDisable() {
    pagesList.ForEach(x => Destroy(x.gameObject));
  }

  void InitPages() {
    foreach (var page in pages)
      page.Init(library, SetActivePage, Bye);
  }

  public void Bye(ShopProductBehaviour product) {
    changeDialog.gameObject.SetActive(true);
    changeDialog.SetData(product);

    changeDialog.OnConfirm = (productConfirm, count) => {
      for (int i = 0; i < count; i++)
        productConfirm.Bye();
    };
  }

  void SetActivePage(ShopPage selectPage) {
    selectPage.transform.SetAsLastSibling();
    pages.ForEach(x => x.SetFocus(selectPage == x));
  }

  public void AddCoinsButton() {
    UIController.ClickPlay();
    UserManager.Instance.silverCoins.Value += 1000;
  }

  public void CloseButton() {
    UIController.ClickPlay();
    mainAnimation.Play(_hideAnim);
  }

  [ContextMenu("Add 1000 coins")]
  public void AddCoins() {
    UserManager.Instance.silverCoins.Value += 1000;
  }

  public void Close() {
    gameObject.SetActive(false);
  }

  [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.OnCoins))]
  private void OnCoinsChange(ExEvent.UserEvents.OnCoins eventData) {
    coinsCount.text = UserManager.Instance.silverCoins.Value.ToString();
    Vector2 deltaSize = coinsCount.GetComponent<RectTransform>().sizeDelta;
    deltaSize.x = coinsCount.preferredWidth;
    coinsCount.GetComponent<RectTransform>().sizeDelta = deltaSize;
  }


  void SetPages(int pageNum) {
    int deltaX = (int)Camera.main.pixelWidth;

    //int deltaY = (int)Camera.main.pixelHeight;

    for (int i = 0; i < pagesList.Count; i++) {
      if (i == pageNum) {
        pagesList[i].anchoredPosition = new Vector2(0, 0);
      } else {
        pagesList[i].anchoredPosition = new Vector2(deltaX, 0);
      }
    }

  }

}
