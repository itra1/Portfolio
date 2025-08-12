using UnityEngine;
using System.Collections;

namespace it.Game.Handles
{
  public class PlayerUseOnEvent : MonoBehaviourBase, Game.Items.IInteraction
  {
	 public UnityEngine.Events.UnityEvent _onUse;


	 public bool IsInteractReady => true;

	 public void StartInteract()
	 {
		_onUse?.Invoke();
	 }

	 public void StopInteract()
	 {
		return;
	 }

  }
}