using System.Net.Sockets;
using com.ootii.Actors;
using com.ootii.Actors.AnimationControllers;
using HutongGames.PlayMaker;
namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Pastel")]
  public class EnableMotionController : FsmStateAction
  {
	 [RequiredField]
	 public FsmOwnerDefault _gameObject;
	 public FsmBool _enabledMotionController;
	 public FsmBool _enabledActorController;
	 public override void OnEnter()
	 {
		var go = Fsm.GetOwnerDefaultTarget(_gameObject);
		go.GetComponent<MotionController>().IsEnabled = _enabledMotionController.Value;
		go.GetComponent<com.ootii.Actors.ActorController>().IsEnabled = _enabledActorController.Value;
		Finish();
	 }
  }
}