using UnityEngine;
using it.Game.Items;
using it.Game.Environment.Handlers;
using it.Game.Player.Interactions;

namespace it.Game.Environment
{
  [RequireComponent(typeof(AddNote))]
  public class NoteStorm : Environment, IPickUpItemFromInventary, IInteractionCondition
  {
	 /* 
	  * Состояния
	  * 0 - готов к взятию
	  * 1 - подобран
	  */

	 [SerializeField]
	 private GameObject _content;
	 [SerializeField]
	 private ParticleSystem _particles;

	 public string PickUpItem => GetComponent<AddNote>().NoteUuid;

	 public void Interaction()
	 {
		SetNote();
		State = 1;
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State == 0)
		  ResetData();
		else
		  _content.SetActive(false);
	 }

	 private void SetNote()
	 {
		var force = _particles.forceOverLifetime;
		force.enabled = true;
		var emiss = _particles.emission;
		emiss.enabled = false;
	 }

	 private void ResetData()
	 {
		var force = _particles.forceOverLifetime;
		force.enabled = false;
		var emiss = _particles.emission;
		emiss.enabled = true;
		_content.SetActive(true);
	 }

	 public bool InteractionReady()
	 {
		return State == 0;
	 }
  }
}