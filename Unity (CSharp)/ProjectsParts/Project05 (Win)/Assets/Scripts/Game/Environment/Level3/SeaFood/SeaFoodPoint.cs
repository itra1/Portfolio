using UnityEngine;
using System.Collections;
namespace it.Game.Environment.Level3
{
  public class SeaFoodPoint : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {
	 /// <summary>
	 /// Источник еды
	 /// 
	 /// Располагается на карте
	 [ContextMenu("Activate")]
	 public void Activate()
	 {
		GetComponentInChildren<it.Game.Items.Inventary.SeaGrid>().GetItemAnimate();
	 }

	 public bool InteractionReady()
	 {
		return GetComponentInChildren<it.Game.Items.Inventary.SeaGrid>() != null;
	 }
  }
}