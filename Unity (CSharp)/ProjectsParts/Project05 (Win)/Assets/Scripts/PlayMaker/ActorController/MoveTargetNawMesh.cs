
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.ActorController
{
  [ActionCategory(ActionCategory.Character)]
  [HutongGames.PlayMaker.Tooltip("Актор контроллер. Перемещение на позицию с использование навмеша")]
  public class MoveTargetNawMesh : MoveNawMesh
  {
	 
	 public FsmGameObject TargetObject;
	 public FsmVector3 TargetPoint;

	 public FsmEvent onMoveComplete;

	 public FsmBool everyUpdate;

	 public override void OnEnter()
	 {
		base.OnEnter();
		SetTarget();
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();

		if (everyUpdate.Value)
		  SetTarget();
	 }

	 private void SetTarget()
	 {

		if (TargetObject.Value != null && !TargetObject.Value.Equals(Owner))
		  TargetPosition = TargetObject.Value.transform.position;
		else
		  TargetPosition = TargetPoint.Value;
	 }

	 protected override void OnArrived()
	 {
		Fsm.Event(onMoveComplete);
	 }

  }
}