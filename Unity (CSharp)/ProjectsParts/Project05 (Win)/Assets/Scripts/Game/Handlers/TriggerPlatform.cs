using UnityEngine;
using System.Collections;
using it.Game.Player;

namespace it.Game.Handles
{
  public class TriggerPlatform : MonoBehaviourBase, IPlayerTriggerEnter
  {
	 public void OnPlayerTriggerEnter()
	 {
		PlayerBehaviour.Instance.Platform = transform;
	 }

	 public void OnPlayerTriggerExit()
	 {
		if (PlayerBehaviour.Instance.Platform != null && PlayerBehaviour.Instance.Platform.Equals(transform))
		  PlayerBehaviour.Instance.Platform = null;
	 }

  }
}