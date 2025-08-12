using UnityEngine;
using System.Collections;

namespace it.Game.Environment.All
{
  public class ActivateOnEvent : Environment
  {
	 /*
	  * Состояния:
	  * 0- не активно
	  * 1- активно
	  * 2
	  */
	 [SerializeField]
	 private GameObject[] _objectActivateArray;


	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _onVisibleObjects;

	 protected override void Start()
	 {
		base.Start();
		if (State == 0)
		  ActivateObjects(false);
	 }
	 [ContextMenu("Activate")]
	 public void OnActive()
	 {
		if (State == 1)
		  return;
		State = 1;
		_onVisibleObjects?.Invoke();
		Save();
		ConfirmState();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		ActivateObjects(State > 0);
	 }

	 private void ActivateObjects(bool isActive)
	 {
		foreach (var elem in _objectActivateArray)
		  if(elem != null)
			 elem.SetActive(isActive);
	 }

  }
}