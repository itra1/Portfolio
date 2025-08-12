using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Environment.Level1.OnixAncientSculpture
{
  public class KnitePosition : Environment, it.Game.Player.Interactions.IInteractionCondition
  {
	 [SerializeField]
	 private UnityEngine.Events.UnityEvent onKnitePosition;
	 [SerializeField]
	 private GameObject _knite;
	 [SerializeField]
	 private string _knifeUuid;
	 public bool IsInteractReady => true;

	 protected override void Start()
	 {
		if(State == 0)
		  _knite.gameObject.SetActive(false);
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);


		_knite.gameObject.SetActive(State == 2);
	 }

	 public void StartInteract()
	 {
		State = 2;
		_knite.gameObject.SetActive(true);
		Game.Managers.GameManager.Instance.Inventary.Remove(_knifeUuid);
		onKnitePosition?.Invoke();
		Save();
	 }

	 public void StopInteract()
	 {
	 }



	 public bool InteractionReady()
	 {
		return Game.Managers.GameManager.Instance.Inventary.ExistsItem(_knifeUuid);

	 }
  }
}