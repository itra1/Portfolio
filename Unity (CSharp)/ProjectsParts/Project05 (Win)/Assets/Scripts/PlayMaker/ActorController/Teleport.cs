using UnityEngine;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Teleport")]
  public class Teleport : ActorDriver
  {
    public FsmGameObject target;

    public override void OnEnter()
    {
      base.OnEnter();


      _actor.SetPosition(target.Value.transform.position);
      _actor.SetRotation(target.Value.transform.rotation);
      _actor.SetRelativeVelocity(Vector3.zero);
      _actor.SetVelocity(Vector3.zero);

      Finish();

    }
  }
}