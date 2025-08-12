using HutongGames.PlayMaker;


namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory("Actor Controller")]
  public class Jump : ActorBase
  {
	 public FsmFloat jump = 3;
	 public override void OnEnter()
	 {
		_actor.AddImpulse(_actor._Transform.up * jump.Value);
		Finish();
	 }

	 

  }
}