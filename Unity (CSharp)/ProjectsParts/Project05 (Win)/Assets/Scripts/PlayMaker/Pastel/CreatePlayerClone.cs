using UnityEngine;
using HutongGames.PlayMaker;
using com.ootii.Actors.AnimationControllers;
using UnityEngine.AI;
using it.Game.Player;

namespace it.Game.PlayMaker
{
  [ActionCategory("Pastel")]
  public class CreatePlayerClone : FsmStateAction
  {
	 public FsmGameObject _actorSource;
	 public FsmBool _removePlayerBehaviour = true;
	 public FsmBool _removeActorController = true;
	 public FsmBool _removeMotionController = true;
	 public FsmBool _removeNavMeshAgent = true;
	 public FsmGameObject _actorResult;

	 public override void OnEnter()
	 {
		GameObject inst =  UnityEngine.MonoBehaviour.Instantiate(_actorSource.Value);
		if (_removePlayerBehaviour.Value)
		  UnityEngine.MonoBehaviour.DestroyImmediate(inst.GetComponentInChildren<PlayerBehaviour>());
		if (_removeMotionController.Value)
		  UnityEngine.MonoBehaviour.DestroyImmediate(inst.GetComponentInChildren<MotionController>());
		if (_removeActorController.Value)
		UnityEngine.MonoBehaviour.DestroyImmediate(inst.GetComponentInChildren<com.ootii.Actors.ActorController>());
		if (_removeNavMeshAgent.Value)
		  UnityEngine.MonoBehaviour.DestroyImmediate(inst.GetComponentInChildren<NavMeshAgent>());
		_actorResult.Value = inst;

		Finish();
	 }
  }
}