using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.NPC.Enemyes.Helpers
{
  /// <summary>
  /// Видимость преера
  /// </summary>
  public interface IPlayerCheck
  {
	 bool IsPlayerVisible { get; }
  }
}