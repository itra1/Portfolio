using UnityEngine;
using System.Collections;
using DG.Tweening;
using it.Game.Player.Interactions;

namespace it.Game.Environment.Level5.PriestArena
{
  public class PriestAbeliskCondition : MonoBehaviourBase, IInteractionCondition
  {
	 [SerializeField]
	 private PriestArena _arena;
	 [SerializeField]
	 private PriestAbelisk _abelisk;

	 public bool InteractionReady()
	 {
		return _arena.State == 2 && !_abelisk.IsActive;
	 }
  }
}