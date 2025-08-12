using UnityEngine;
using System.Collections;

namespace it.Game.Environment.All.Doors
{
  /// <summary>
  /// Дверь
  /// </summary>
  public class Door : Environment
  {
	 /*
	  * States:
	  * 0 - close
	  * 1 - open
	  */

	 public UnityEngine.Events.UnityEvent _onOpen;
	 public UnityEngine.Events.UnityEvent _onClose;

	 private Animator _animator;
	 private Renderer[] _renderers;

	 protected override void Start()
	 {
		base.Start();
		GetRenderers();

		for (int i = 0; i < _renderers.Length; i++)
		  _renderers[i].material = Instantiate(_renderers[i].material);


	 }
	 [ContextMenu("Open")]
	 public void Open()
	 {
		if (State == 1)
		  return;
		State = 1;
		ConfirmState(false);
		_onOpen?.Invoke();
		Save();
	 }
	 [ContextMenu("Close")]
	 public void Close()
	 {
		if (State == 0)
		  return;
		State = 0;
		ConfirmState(false);
		_onClose?.Invoke();
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {

		if (_animator == null)
		  _animator = GetComponent<Animator>();
		_animator.SetBool("Open", State != 0);

		it.Game.Scenes.Level7LocationManager _locationManager = (it.Game.Scenes.Level7LocationManager)Managers.GameManager.Instance.LocationManager;
		GetRenderers();

		for (int i = 0; i < _renderers.Length; i++)
		{
		  _renderers[i].material.SetColor("_EmissionColor", (State == 0 ? _locationManager.ColorBlocked : _locationManager.ColorOpen));
		}
	 }

	 private void GetRenderers()
	 {

		if (_renderers == null)
		  _renderers = GetComponentsInChildren<Renderer>();
	 }

  }
}