using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUi : UiDialog {

  public I2.Loc.Localize text;

  private Tutorial.Tutorial data;

  public void SetData(Tutorial.Tutorial data) {
    text.Term = data.text;
    this.data = data;
  }

  public void CloseButton() {
    UiController.Instance.ClickSound();
    Hide(()=> {

      this.data.Complete();

    });

  }

}
