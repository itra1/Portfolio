using UnityEngine;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Boses.Gishtil
{
  [ActionCategory("Enemyes")]
  [Tooltip("Движение к игроку")]
  public class GishtilMove : it.Game.PlayMaker.ActorController.MoveTargetAStar
  {
	 public FsmGameObject _targetObject;
	 private Animator _animator;
	 private int _stateAnim = 1;

	 public FsmEvent OnComplete;

	 public override void Awake()
	 {
		base.Awake();
		Inicialization();
		_animator = _go.GetComponent<Animator>();
	 }

	 public override void OnEnter()
	 {
		base.OnEnter();
		_animator.SetInteger("State", _stateAnim);
		TargetPosition = _targetObject.Value.transform.position;
	 }

	 public override void OnUpdate()
	 {
		base.OnUpdate();
		TargetPosition = _targetObject.Value.transform.position;
	 }

	 public override void OnExit()
	 {
		base.OnExit();
		_animator.SetInteger("State", 0);
	 }

	 private float forvardAnim = 0;

	 protected override void SetActorMove(Vector3 move)
	 {
		forvardAnim += (move.magnitude > 0 ? 1 : -1) * Time.deltaTime * 2;
		forvardAnim = Mathf.Clamp(forvardAnim, 0, 1);
		_animator.SetFloat("Forward", forvardAnim);

	 }

	 protected override void OnArrived()
	 {
		Fsm.Event(OnComplete);
	 }
  }
}