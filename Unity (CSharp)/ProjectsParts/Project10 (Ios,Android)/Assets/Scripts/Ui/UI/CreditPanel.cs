using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ExEvent;
using System;

public class CreditPanel : PanelUi {

  public GameObject closeIcon;            // Иконка закрытия
  public GameObject birdIcon;             // Иконка компании
  public GameObject textBlock;            // Иконка блока текста
  public GameObject thisPanel;            // Ссылка на родителя текущей панели
	
  void Start() {
    DeactiveAll();
  }

  protected override void OnEnable() {
		base.OnEnable();
		//CameraController.Instance.GoBlur(true);
    DeactiveAll();
  }

  protected override void OnDisable() {
    base.OnDisable();
    DeactiveAll();
  }

  public void CloseThis() {
		this.ClosingStart();
    UiController.ClickButtonAudio();
    GetComponent<Animator>().SetTrigger("close");
  }
	
  void DeactiveAll() {
    thisPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    closeIcon.SetActive(false);
    birdIcon.SetActive(false);
    textBlock.SetActive(false);
  }

  public void ButtomVisiteSite() {
    UiController.ClickButtonAudio();
    Application.OpenURL("http://www.kingbirdgames.com");
  }

  public void ButtomMoreGames() {
    UiController.ClickButtonAudio();
    //Application.OpenURL("https://itunes.apple.com/developer/king-bird-games/id1013592798");
    string myProduct = "1013592798";
    storeManager.showStoreWithProduct(myProduct);
  }

	/// <summary>
	/// Событие анимации
	/// Успешное закрытие
	/// </summary>
	public void HideAnimEvent() {
		if(OnClose != null) OnClose();
		OnClose = null;
		gameObject.SetActive(false);
	}
	
	public override void BackButton() {
		CloseThis();
	}
	
}
