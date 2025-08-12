using UnityEngine;

public class MapUi: UiPanel {

  public System.Action onSetting;
  public System.Action onBase;
  public System.Action onNextLocation;
  public System.Action onBackLocation;

  protected override void OnEnable() {
    base.OnEnable();
    m_topLeftLeftButton.SetActive(UserManager.Instance.ReadyMoveToBlock(UserManager.Instance.SelectBlock + 1));
    m_buttomRightRightButton.SetActive(UserManager.Instance.ReadyMoveToBlock(UserManager.Instance.SelectBlock - 1));
  } 


  [SerializeField]
  private GameObject m_topLeftLeftButton;
  [SerializeField]
  private GameObject m_buttomRightRightButton;

  public void SettingButton() {

    if (onSetting != null)
      onSetting();
  }

  public void BaseButton() {

    if (onBase != null)
      onBase();
  }

  public void TopLeftLeftButton() {
    onNextLocation?.Invoke();
  }
  public void BottomRightRightButton() {
    onBackLocation?.Invoke();
  }


}
