using System.Collections;

using UnityEngine;
using com.ootii.Messages;
using it.Game.Player;

namespace it.Game.Environment.Level1
{
  public class MysticalTeeth : MonoBehaviour
  {
	 [SerializeField]
	 private Transform _spawn1Position;
	 [SerializeField]
	 private Transform _spawn2Position;

	 private int _lastTrigger = 0;
	 private bool _isTeleport;

	 private void OnEnable()
	 {
		MessageDispatcher.AddListener(EventsConstants.PlayerFormChange, OnFormChange, true);
	 }

	 private void OnDisable()
	 {
		MessageDispatcher.RemoveListener(EventsConstants.PlayerFormChange, OnFormChange, true);
	 }
	 private void OnFormChange(IMessage message)
	 {
		if (PlayerBehaviour.Instance.Form == 0)
		  _isTeleport = true;
	 }

	 public void Trigger1Enter()
	 {
		_lastTrigger = 1;
	 }

	 public void Trigger2Enter()
	 {
		_lastTrigger = 2;
	 }

	 public void TriggerLabirint()
	 {
		PlayerPortal();
	 }

	 private void PlayerPortal()
	 {
		//if (!_isTeleport) return;

		//_isTeleport = false;

		Vector3 jumpPosition = _lastTrigger == 1 ? _spawn1Position.position : _spawn2Position.position;
		Player.PlayerBehaviour.Instance.PortalJump(jumpPosition);
	 }

  }
}