using UnityEngine;
using System.Collections;
using it.Game.Player.Interactions;
using it.Game.Managers;

namespace it.Game.Environment.Handlers
{
  public class CheckInventaryItem : MonoBehaviour, IInteractionCondition
  {
	 [SerializeField]
	 private string _itemUuid;
	 public bool InteractionReady()
	 {
		return GameManager.Instance.Inventary.ExistsItem(_itemUuid);
	 }
  }
}