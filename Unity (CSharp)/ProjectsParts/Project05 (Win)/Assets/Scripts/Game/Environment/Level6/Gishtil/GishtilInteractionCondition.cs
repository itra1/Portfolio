using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;

namespace it.Game.Environment.Level6.Gishtil
{
  public class GishtilInteractionCondition : MonoBehaviourBase, it.Game.Player.Interactions.IInteractionCondition
  {
	 [SerializeField]
	 private GishtilBossArena _arena;

	 bool IInteractionCondition.InteractionReady()
	 {
		return _arena.InteractionReady;
	 }
	 public void StartInteraction()
	 {
		_arena.Interaction();
	 }
  }
}