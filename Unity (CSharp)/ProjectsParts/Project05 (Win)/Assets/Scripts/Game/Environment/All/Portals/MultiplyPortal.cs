using UnityEngine;
using System.Collections;
using it.Game.Managers;

namespace it.Game.Environment.All.Portals
{
  public class MultiplyPortal : MonoBehaviourBase, Game.Items.IInteraction
  {
	 [SerializeField]
	 private Transform _targetPosition;

	 public bool IsInteractReady => true;

	 public void StartInteract()
	 {
		UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			  new Color32(0, 0, 0, 255), 1, null, () =>
			  {

				 Game.Player.PlayerBehaviour.Instance.PortalJump(_targetPosition);
			  }, () =>
			  {

			  });
	 }

	 public void StopInteract()
	 {
	 }


	 private void OnDrawGizmosSelected()
	 {
		if (_targetPosition != null)
		  Gizmos.DrawLine(transform.position, _targetPosition.position);
	 }
  }
}