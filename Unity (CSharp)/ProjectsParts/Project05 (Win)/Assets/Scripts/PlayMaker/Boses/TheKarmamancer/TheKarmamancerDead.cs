using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.Boses.TheKarmamancer
{
  /// <summary>
  /// Смерть игрока
  /// </summary>

  [ActionCategory("Enemyes")]
  [Tooltip("Смерть босса")]
  public class TheKarmamancerDead : FsmStateAction
  {
	 public override void OnEnter()
	 {
		base.OnEnter();

		Owner.SetActive(false);

	 }
  }
}