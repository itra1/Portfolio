using HutongGames.PlayMaker;

using Obi;

namespace it.Game.PlayMaker.Transforms
{
  [ActionCategory("Obi")]
  public class FluidEmitterSetProperty : FsmStateAction
  {
	 public FsmGameObject emitterObject;
	 public FsmFloat speed = 1;
	 public FsmFloat lifespan = 1;

	 private ObiEmitter _emitter;

	 public override void OnEnter()
	 {
		if (_emitter == null)
		  _emitter = emitterObject.Value.GetComponent<ObiEmitter>();

		_emitter.speed = speed.Value;
		_emitter.lifespan = lifespan.Value;

	 }
  }
}