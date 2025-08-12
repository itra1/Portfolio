using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using it.Game.Handles;
using DG.Tweening;

namespace it.Game.Environment.Level5.ThreeObelisks
{
  public class CrystalSource : Environment, IInteractionCondition
  {
	 [SerializeField]
	 private string uuidItem;

	 [SerializeField]
	 private GameObject _crystal;


	 private bool _isExist;

	 public bool IsExist { get => _isExist; set => _isExist = value; }

	 public void Interaction()
	 {
		State = 2;
		Game.Managers.GameManager.Instance.Inventary.AddItem(uuidItem, 1);
		Save();
		ConfirmState();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State == 2)
		{
		  _crystal.SetActive(false);
		}
		else
		{
		  _crystal.SetActive(true);
		}

	 }


	 public bool InteractionReady()
	 {
		return State < 2;
	 }
  }
}