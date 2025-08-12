using System.Collections;

using UnityEngine;

namespace it.Game.Player
{
  public class PlayerHelpers
  {

	 public static bool IsPlayer(Transform tr)
	 {
		bool isPlay = tr.GetComponent<PlayerBehaviour>() != null;

		if(!isPlay)
		  isPlay = tr.GetComponentInParent<PlayerBehaviour>() != null;

		return isPlay;
	 }

  }
}