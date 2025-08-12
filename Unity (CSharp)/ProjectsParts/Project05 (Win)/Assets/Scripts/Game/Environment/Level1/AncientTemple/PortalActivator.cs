using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Level1.AncientTemple
{
  public class PortalActivator : Environment, Game.Items.IInteraction
  {
	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _onClick;
	 public bool IsInteractReady => State <= 0;

	 [ContextMenu("Use")]
	 public void StartInteract()
	 {
		if (State > 0)
		  return;
		State = 1;
		_onClick?.Invoke();
		Save();
	 }

	 public void StopInteract()
	 {
	 }
  }
}