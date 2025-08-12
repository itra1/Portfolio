using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Handles
{
  public class MouseHandler : MonoBehaviourBase
  {
	 public UnityEngine.Events.UnityAction onMouseUp;
	 public UnityEngine.Events.UnityAction onMouseDown;
	 public UnityEngine.Events.UnityAction onMouseHold;

	 private void Start() { }

	 private void Update()
	 {

		if (Game.Managers.GameManager.Instance.EnvironmentInputSource.IsJustReleased(KeyCode.Mouse0))
		  onMouseUp?.Invoke();

		if (Game.Managers.GameManager.Instance.EnvironmentInputSource.IsJustPressed(KeyCode.Mouse0))
		  onMouseDown?.Invoke();

		if (Game.Managers.GameManager.Instance.EnvironmentInputSource.IsPressed(KeyCode.Mouse0))
		  onMouseHold?.Invoke();

	 }

	 public void Clear()
	 {
		onMouseUp = null;
		onMouseDown = null;
		onMouseHold = null;
	 }

	 public RaycastHit GetMouseRayHit(float distance = 20)
	 {
		Ray ray = CameraBehaviour.Instance.Camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, distance);
		return hit;
	 }

	 public RaycastHit[] GetMouseRayHits(float distance = 20)
	 {
		Ray ray = CameraBehaviour.Instance.Camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, distance);
		return hits;
	 }


  }
}