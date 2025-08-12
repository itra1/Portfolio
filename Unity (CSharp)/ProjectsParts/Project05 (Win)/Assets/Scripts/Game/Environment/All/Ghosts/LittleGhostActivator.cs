using UnityEngine;
using System.Collections;

namespace it.Game.Environment.All.Ghosts
{

  public class LittleGhostActivator : MonoBehaviourBase, Game.Items.IInteraction
  {
	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _onUse;

	 public bool IsInteractReady => true;

	 [ContextMenu("Use")]
	 public void StartInteract()
	 {
		_onUse?.Invoke();
	 }

	 public void StopInteract()
	 {
	 }
  }
}