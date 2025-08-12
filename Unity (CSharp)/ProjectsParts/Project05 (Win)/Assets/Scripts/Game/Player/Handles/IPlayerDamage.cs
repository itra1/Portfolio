using UnityEngine;
using System.Collections;

namespace it.Game.Player.Handlers
{
  public interface IPlayerDamage
  {
	 float PlayerDamageValue {get;}

	 bool IgnoreGracePeriod { get; }
  }
}