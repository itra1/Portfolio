using System.Collections;

using UnityEngine;
using it.Game.Environment.Level5.Leech;

namespace it.Game.NPC.Enemyes
{
  /// <summary>
  /// Пиявки пятого уровня
  /// </summary>
  public class Leech : Enemy
  {
	 private LeechZone _manager;


	 public void LaserHit()
	 {
		gameObject.SetActive(false);
	 }

  }

}