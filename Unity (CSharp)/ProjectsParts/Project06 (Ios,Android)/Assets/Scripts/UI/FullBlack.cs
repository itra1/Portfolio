using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Полное затемнение экрана с интерфейсом
/// </summary>
public class FullBlack : MonoBehaviour {

  public Image fullBlack;

  public static FullBlack instance;

  void Start() {
    GameManager.OnChangeFullScreen += ChangeFullScreen;
  }

  void OnDestroy() {
    GameManager.OnChangeFullScreen -= ChangeFullScreen;
  }

  void ChangeFullScreen() {
    StartCoroutine(ChangeFullScreenIenum());
  }

  IEnumerator ChangeFullScreenIenum() {
    fullBlack.color = new Color(0 , 0 , 0 , 1);
    yield return new WaitForSeconds(1);
    fullBlack.color = new Color(0 , 0 , 0 , 0);
  }

}
