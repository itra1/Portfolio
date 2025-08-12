using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Информационная панель
/// </summary>
public class InfoPanel : MonoBehaviour {

  public Text infoText;

  public Actione<GameObject> OnOk;
  public Actione<GameObject> OnCancel;
  public Actione<GameObject> OnClose;

  public GameObject buttonGroup1;
  public GameObject buttonGroup2;
  public GameObject buttonGroup3;

  ButtonGroup _buttonGroup;
  
  public void Init(string title, ButtonGroup buttonGroup, Actione<GameObject> OnButtonOK = null, Actione<GameObject> OnButtonCancel = null, Actione<GameObject> OnButtonClose = null) {

    infoText.text = title;
    _buttonGroup = buttonGroup;
    OnOk = OnButtonOK;
    OnCancel = OnButtonCancel;
    OnClose = OnButtonClose;
    SwitchButtonGroup();
  }

  public enum ButtonGroup {
    none,
    ok,
    cancel,
    ok_cancel
  }

  void SwitchButtonGroup() {
    buttonGroup1.SetActive(_buttonGroup == ButtonGroup.ok);
    buttonGroup2.SetActive(_buttonGroup == ButtonGroup.ok_cancel);
    buttonGroup3.SetActive(_buttonGroup == ButtonGroup.cancel);
  }

  public void ButtonOk() {
    if(OnOk != null) OnOk(gameObject);
    Close();
  }
  public void ButtonCancel() {
    if(OnCancel != null) OnCancel(gameObject);
    Close();
  }

  public void Close() {
    if(OnClose != null) OnClose(gameObject);
    Destroy(gameObject);
  }

}
