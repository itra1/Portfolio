using HutongGames.PlayMaker;
namespace it.Game.PlayMaker
{
  [ActionCategory("Tutorial")]
  public class TutorialShow : FsmStateAction
  {
	 public FsmInt _tutorial;
	 public FsmEvent onComplete;


	 public override void OnEnter()
	 {
		it.Game.Managers.TutorialManager.Instance.ShowTutorial(_tutorial.Value, () =>
		{
		  Fsm.Event(onComplete);
		});
	 }

  }
}