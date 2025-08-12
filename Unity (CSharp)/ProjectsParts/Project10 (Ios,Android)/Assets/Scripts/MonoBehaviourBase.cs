using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourBase : MonoBehaviour {

  public List<System.Action> invokeList = new List<System.Action>();

  /// <summary>
  /// Переопределение инвока
  /// </summary>
  /// <param name="func">Исполняемая функция</param>
  /// <param name="time">Задержка исполнения</param>
  public Coroutine Invoke(System.Action func, float time) {

    invokeList.Add(func);
    StartCoroutine(WaitFunction(() => {
      invokeList.Remove(func);
    }, time));

    return StartCoroutine(WaitFunction(func, time));
  }

  public bool IsInvoking(System.Action func) {
    return invokeList.Contains(func);
  }

  /// <summary>
  /// Отмена выполнения
  /// </summary>
  /// <param name="corotine"></param>
  public void CancelInvoke(Coroutine corotine) {
    StopCoroutine(corotine);
  }

  IEnumerator WaitFunction(System.Action func, float time) {
    yield return new WaitForSeconds(time);
    func();
  }

}
