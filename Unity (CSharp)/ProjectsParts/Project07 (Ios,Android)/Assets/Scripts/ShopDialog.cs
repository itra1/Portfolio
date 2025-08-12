using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopDialog : UiDialog {
	public ScrollRect scrollRect;

  public RectTransform contentRect;
  
  public RectTransform chestRect;
  public RectTransform goldRect;
  public RectTransform energyRect;
  public RectTransform backsRect;

  private Pages targetPage;

  public enum Pages {
    none,
    chest,
    gold,
    energy,
    backs
  }

  public void SetPage(Pages page) {
    targetPage = page;

    PageConfirm();
  }

  protected override void OnEnable() {
    base.OnEnable();
    GameTime.timeScale = 0;

    PageConfirm();

  }

  protected override void OnDisable() {
    targetPage = Pages.none;
    GameTime.timeScale = 1;
  }
  
  public void CloseButton() {
    UiController.Instance.ClickSound();
    Hide();
	}

  /// <summary>
  /// Временно для дебага
  /// </summary>
  public void AddBacks() {
    UserManager.Instance.HardCash += 1000;
    UserManager.Instance.Gold += 1000;
  }

  public void OpenAllLevels()
  {
    UserManager.Instance.MaxCompleteLevel = LocationManager.Instance.GetLastLocation().Level;
  }

	public override void ShowComplete() {
		base.ShowComplete();

    PageConfirm();
  }

  private void PageConfirm() {
    switch (targetPage) {
      case Pages.none:
      case Pages.chest:
        scrollRect.verticalNormalizedPosition = 1;
        break;
      case Pages.gold:
        scrollRect.verticalNormalizedPosition = 0.3827f;
        break;
      case Pages.energy:
        scrollRect.verticalNormalizedPosition = 0.2094f;
        break;
      case Pages.backs:
        scrollRect.verticalNormalizedPosition = 0.0319f;
        break;
    }
  }

}
