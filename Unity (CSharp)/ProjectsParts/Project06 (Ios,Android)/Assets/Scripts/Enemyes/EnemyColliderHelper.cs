using UnityEngine;
using System.Collections;

/// <summary>
/// Помошник врагам для определения объектов в зоне достижения
/// </summary>
public class EnemyColliderHelper : MonoBehaviour {

  public delegate void TriggetStayDelegate(Collider2D col);
  public TriggetStayDelegate TriggerEnter;
  public TriggetStayDelegate TriggerExit;
  public TriggetStayDelegate TriggerStay;

  void OnTriggerEnter2D(Collider2D col) {
    if (TriggerEnter != null) TriggerEnter(col);
  }

  void OnTriggerExit2D(Collider2D col) {
    if (TriggerExit != null) TriggerExit(col);
  }

  /// <summary>
  /// Нахождение в зоне действия заклинания
  /// </summary>
  /// <param name="oth"></param>
  void OnTriggerStay2D(Collider2D oth) {
    if (TriggerStay != null) TriggerStay(oth);
  }

}
