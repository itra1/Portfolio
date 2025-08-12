using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Utils
{
  /// <summary>
  /// Запрет на уничтожение
  /// </summary>
  public class DontDestroy : MonoBehaviourBase
  {
	 private void Awake()
	 {
		DontDestroyOnLoad(gameObject);
	 }
  }
}