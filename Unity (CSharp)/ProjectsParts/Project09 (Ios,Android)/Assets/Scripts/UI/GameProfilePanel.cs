using UnityEngine;
using System.Collections;

/// <summary>
/// Гейм профиль панель
/// </summary>
public class GameProfilePanel : MonoBehaviour {

  public Actione OnCreate;
  public Actione OnGetProfile;
  public Actione OnUpdate;
  public Actione OnClose;

  public void CreateButton() {
    if(OnCreate != null) OnCreate();
  }

  public void GetProfileButton() {
    if(OnGetProfile != null) OnGetProfile();
  }

  public void UpdateButton() {
    if(OnUpdate != null) OnUpdate();
  }

  public void CloseButton() {
    Destroy(gameObject);
  }

}
