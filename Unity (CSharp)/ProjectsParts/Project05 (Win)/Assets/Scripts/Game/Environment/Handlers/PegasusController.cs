using System.Collections;
using System.Collections.Generic;

using Pegasus;

using UnityEngine;

namespace it.Game.Environment.Handlers
{
  public class PegasusController : MonoBehaviourBase
  {

	 [SerializeField]
	 private Pegasus.PegasusManager _pegasusManager;
	 private UnityEngine.Events.UnityAction OnComplete;
	 public UnityEngine.Events.UnityAction<int> OnIndexChenge;
	 private Transform _cameraSourceParent;
	 private int _inx;

	 private bool _fromCameraPosition = true;
	 public bool FromCameraPosition { get => _fromCameraPosition; set => _fromCameraPosition = value; }



	 public PegasusManager PegasusManager
	 {
		get
		{

		  if (_pegasusManager == null)
			 _pegasusManager = GetComponent<Pegasus.PegasusManager>();
		  return _pegasusManager;
		}
		set => _pegasusManager = value;
	 }


	 public bool Activate(UnityEngine.Events.UnityAction onComplete)
	 {
		if (PegasusManager == null)
		  return false;

		if (FromCameraPosition)
		{
		  var poi = PegasusManager.GetFirstPOI();
		  poi.transform.rotation = CameraBehaviour.Instance.transform.rotation;
		  PegasusManager.MovePoi(poi, CameraBehaviour.Instance.transform.position - poi.transform.position);
		}
		OnComplete = onComplete;
		_inx = -1;
		CameraBehaviour.Instance.CameraController.Anchor = null;
		CameraBehaviour.Instance.CameraController.ActivateMotor(5);
		PegasusManager.m_target = CameraBehaviour.Instance.gameObject;
		PegasusManager.StartFlythrough(true);
		Game.Managers.GameManager.Instance.GameInputSource.enabled = false;

		//PegasusManager.PauseFlythrough();

		StartCoroutine(CoroutinaWait());

		return true;
	 }
	 private IEnumerator CoroutinaWait()
	 {

		while (PegasusManager.m_currentState != Pegasus.PegasusConstants.FlythroughState.Stopped)
		{
		  if (_inx != PegasusManager.m_currentSegmentIdx)
		  {
			 _inx = PegasusManager.m_currentSegmentIdx;
			 OnIndexChenge?.Invoke(_inx);
		  }
		  yield return null;
		}
		OnComplete?.Invoke();
		OnComplete = null;
	 }

	 private void OnDrawGizmosSelected()
	 {
		if (_pegasusManager == null)
		  _pegasusManager = GetComponent<Pegasus.PegasusManager>();
		if (_pegasusManager == null)
		  _pegasusManager = GetComponentInChildren<Pegasus.PegasusManager>();
	 }

	 [ContextMenu("Activate")]
	 public void Activate()
	 {
		Activate(null);
	 }

	 [ContextMenu("Deactivate")]
	 public void Deactivate()
	 {
		CameraBehaviour.Instance.CameraController.Anchor = Game.Player.PlayerBehaviour.Instance.transform;
		PegasusManager.m_target = null;
		PegasusManager.StopFlythrough();
		CameraBehaviour.Instance.CameraController.ActivateMotor(0);
		Game.Managers.GameManager.Instance.GameInputSource.enabled = true;
	 }

  }
}