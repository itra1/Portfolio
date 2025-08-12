using System.Collections;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

using UnityEngine;

namespace it.Game.PlayMaker.Helpers
{
  [ActionCategory("Helpers")]
  [HutongGames.PlayMaker.Tooltip("присвоение данныех одной ссылке другой")]
  public class GameObjectToGameObject : FsmStateAction
  {
	 public FsmGameObject _source;
	 public FsmGameObject _target;

	 public override void OnEnter()
	 {
		_target.Value = _source.Value;
		Finish();
	 }

  }
}