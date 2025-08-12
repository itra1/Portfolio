using UnityEngine;
using System.Collections;

namespace it.Game.Environment.All.StoneSymbolButton
{
  [RequireComponent(typeof(Game.Handles.PlayerEnterSection))]
  public class StoneSymbolButton : Environment, Game.Items.IInteraction
  {

	 /*
	  * Сщстшяние
	  * 0 - ожидает
	  * 1 - активно
	  * 2 - врата открыты
	  */
	 public bool IsInteractReady => State <= 0;

	 [SerializeField] private UnityEngine.Events.UnityEvent _activeEvent;
	 [SerializeField] private UnityEngine.VFX.VisualEffect _symbol;
	 [SerializeField] private GameObject _sphere;
	 [SerializeField] private GameObject _circleParticles;
	 [SerializeField] private ParticleSystem _rayParticle;
	 [SerializeField] private Game.Environment.Handlers.RayTripleDrawner _rayDrawner;
	 [SerializeField] private float _timeBeforeEvent = 0.7f;
	 [SerializeField] private float _timeAfterEvent = 0.5f;

	 protected override void Start()
	 {
		base.Start();

		var cmp = GetComponent<Game.Handles.PlayerEnterSection>();
		cmp.onPlayerEnter = () =>
		{
		  _symbol.SendEvent("OnPlay");
		};
		cmp.onPlayerExit = () =>
		{
		  _symbol.SendEvent("OnStop");
		};
	 }

	 private void Deactive()
	 {

		if (_sphere != null) _sphere.SetActive(false);
		if (_rayParticle != null)
		{
		  var emission = _rayParticle.emission;
		  emission.enabled = false;
		}
		if (_rayDrawner != null)
		  _rayDrawner.gameObject.SetActive(true);
		if (_circleParticles != null)
		  _circleParticles.SetActive(false);
	 }

	 private void Activate()
	 {

		if (_sphere != null) _sphere.SetActive(true);
		if (_rayParticle != null)
		{
		  var emission = _rayParticle.emission;
		  emission.enabled = false;
		}
		if (_rayDrawner != null)  _rayDrawner.gameObject.SetActive(true);
		if (_circleParticles != null)
		  _circleParticles.SetActive(true);
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State == 0)
		  Deactive();
		else
		  Activate();

	 }

	 private void OnActiveConfirm()
	 {
		State = 2;
		if(_sphere != null)
		  _sphere.SetActive(true);
		if (_rayDrawner != null)
		  _rayDrawner.gameObject.SetActive(true);
		if (_circleParticles != null)
		  _circleParticles.SetActive(true);
		_activeEvent?.Invoke(); 
		if (_rayParticle != null)
		{
		  var emission = _rayParticle.emission;
		  emission.enabled = false;
		}
		Save();
	 }

	 [ContextMenu("Use")]
	 public void StartInteract()
	 {
		State = 1;


		DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransformAndForget("Effects", transform, 1, null, 0, "StoneButton");

		if (_sphere != null)
		  _sphere.SetActive(true);
		if (_rayDrawner != null)
		{
		  _rayDrawner.gameObject.SetActive(true);
		  _rayDrawner.StartVisualLine();
		}
		if (_rayParticle != null)
		{
		  var emission = _rayParticle.emission;
		  emission.enabled = true;
		}

		var pegasus = GetComponentInChildren<it.Game.Environment.Handlers.PegasusController>(true);

		if (pegasus == null)
		{
		  InvokeSeconds(() =>
		  {
			 OnActiveConfirm();
		  }, 4);
		  return;
		}
		Save();

		pegasus.Activate(() =>
		{
		  DG.Tweening.DOVirtual.DelayedCall(_timeBeforeEvent, () =>
		  {


			 OnActiveConfirm();
			 DG.Tweening.DOVirtual.DelayedCall(_timeAfterEvent, () =>
			 {
				pegasus.Deactivate();
			 });

		  });
		});

		return;

	 }

	 public void StopInteract()
	 {
	 }
  }
}