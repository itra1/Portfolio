using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Boses
{
  /// <summary>
  /// Телепортация босса во временную позицию
  /// </summary>
  [ActionCategory("Enemyes")]
  public class TheKarmamancerTeleportToTempPosition : FsmStateAction
  {
	 /// <summary>
	 /// Целева япозиция
	 /// </summary>
	 public FsmGameObject _targetPosition;

	 public override void OnEnter()
	 {
		base.OnEnter();
		Owner.transform.position = _targetPosition.Value.transform.position;
		Finish();
	 }

  }
}