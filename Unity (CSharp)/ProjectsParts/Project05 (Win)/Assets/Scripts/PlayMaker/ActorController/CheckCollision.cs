using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using com.ootii.Actors;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class CheckCollision : ActorBase
  {
	 public FsmBool _everyFrame;
	 [UIHint(UIHint.Variable)]
	 [ArrayEditor(VariableType.GameObject)]
	 public FsmArray hits;
	 public FsmEvent _onCollision;
	 public FsmBool useLayers;
	 [HideIf("UseLayers")]
	 [UIHint(UIHint.Layer)]
	 public FsmInt[] _layers;

	 private bool UseLayers()
	 {
		return !useLayers.Value;
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		Check();
		if (!_everyFrame.Value)
		{
		  if (hits.Values.Length > 0)
			 Fsm.Event(_onCollision);
		  else
			 Finish();

		}
	}

	 public override void OnUpdate()
	 {
		base.OnUpdate();
		if (_everyFrame.Value)
		{
		  Check();
		  if (hits.Values.Length > 0)
			 Fsm.Event(_onCollision);
		}
	 }

	 private void Check()
	 {
		List<BodyShapeHit> _hodyes = new List<BodyShapeHit>();
		if (useLayers.Value)
		{
		  _hodyes = _actor.GetCollision(ActionHelpers.LayerArrayToLayerMask(_layers, false));
		  hits.Values = _hodyes.ToArray();
		}
		else
		{
		  _hodyes = _actor.GetCollision();
		  hits.Values = _hodyes.ToArray();
		}
		
		var bodyHits =  _actor.GetCollision();
		hits.Values = bodyHits.ToArray();
	 }
  }
}