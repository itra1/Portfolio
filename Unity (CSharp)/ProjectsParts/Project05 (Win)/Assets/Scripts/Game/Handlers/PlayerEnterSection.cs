using UnityEngine;
using System.Collections;

namespace it.Game.Handles
{
  [RequireComponent(typeof(Collider))]
  public class PlayerEnterSection : MonoBehaviourBase
  {
	 public UnityEngine.Events.UnityAction onPlayerEnter;
	 public UnityEngine.Events.UnityAction onPlayerExit;

	 private void OnTriggerEnter(Collider other)
	 {
		if (other.GetComponent<Game.Player.IPlayer>() != null)
		  onPlayerEnter?.Invoke();
	 }

	 private void OnTriggerExit(Collider other)
	 {

		if (other.GetComponent<Game.Player.IPlayer>() != null)
		  onPlayerExit?.Invoke();
	 }
  }
}