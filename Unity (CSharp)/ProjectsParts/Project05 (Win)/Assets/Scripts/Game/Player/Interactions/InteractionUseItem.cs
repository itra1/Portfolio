using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Player.Interactions
{
  public class InteractionUseItem : MonoBehaviourBase, IInteractionCondition
  {
	 [SerializeField]
	 private string _itemUUID;

	 public string ItemUUID { get => _itemUUID; private set => _itemUUID = value; }


	 public void AddItem(RootMotion.FinalIK.FullBodyBipedIK fullBody, Game.Player.Interactions.InteractionSystem iSystem, UnityEngine.Events.UnityAction onComplete)
	 {

		Interactions.InteractionObject iterObject = GetComponent<Interactions.InteractionObject>();

		GameObject itemPrefab = Game.Managers.GameManager.Instance.Inventary.GetPrefab(ItemUUID);

		GameObject itemInst = MonoBehaviour.Instantiate(itemPrefab, fullBody.references.rightHand);
		itemInst.transform.localPosition = Vector3.zero;
		itemInst.transform.localScale = Vector3.one;
		itemInst.transform.localEulerAngles = Vector3.zero;

		var compItem = itemInst.GetComponent<Game.Items.Inventary.InventaryItem>();

		if (compItem != null)
		  compItem.ClearUsageItem();

		itemInst.SetActive(true);
		//iSystem.SetPoser(RootMotion.FinalIK.FullBodyBipedEffector.RightHand, iterObject);

	 }

	 public bool InteractionReady()
	 {
		return Game.Managers.GameManager.Instance.Inventary.ExistsItem(_itemUUID);
	 }
  }
}