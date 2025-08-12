using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDialog : UiDialog {

	public System.Action OnNext;
  public System.Action OnBack;

  public I2.Loc.Localize nameLocation;
	public I2.Loc.Localize descriptionLocation;
	public Image imageLocation;

public void SetData(Location location) {
  location.Initiate();
  nameLocation.Term = location.Title;

  descriptionLocation.Term = location.Description;
	imageLocation.sprite = location.IconLogo;
}

	public void NextButton() {
    OnNext?.Invoke();
  }

  public void BackButton() {
    OnBack?.Invoke();
  }

	public void CloseButton() {
    Hide();
	}


}
