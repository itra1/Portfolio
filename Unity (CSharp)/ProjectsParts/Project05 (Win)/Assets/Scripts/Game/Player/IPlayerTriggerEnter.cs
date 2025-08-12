using UnityEngine;
using System.Collections;

namespace it.Game.Player
{
  /// <summary>
  /// Интерфейс взаимодействия игрока с триггером
  /// </summary>
  public interface IPlayerTriggerEnter
  {
	 void OnPlayerTriggerEnter();
	 void OnPlayerTriggerExit();
  }
}