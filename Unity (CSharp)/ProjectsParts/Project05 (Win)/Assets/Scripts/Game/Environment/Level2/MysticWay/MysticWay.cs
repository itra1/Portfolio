using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level2.MysticWay
{
  /// <summary>
  /// Мистический путь
  /// </summary>
  public class MysticWay : Environment
  {

	 /// <summary>
	 /// Вход игроков
	 /// </summary>
	 public void PlayerEnterTrigger()
	 {
		PlayerDamage();
	 }

	 public void PlayerDamage()
	 {

	 }

  }
}