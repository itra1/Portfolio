using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

namespace it.Game.PlayMaker.Boses.SarDayyni
{
  public abstract class SarDayyniAttack : FsmStateAction
  {
	 [DisplayOrder(0)]
	 public FsmOwnerDefault _owner;

	 public FsmEvent OnComplete;

	 protected GameObject _go;
	 protected Animator _animator;

	 public override void OnEnter()
	 {
		if (_go == null)
		{
		  _go = Fsm.GetOwnerDefaultTarget(_owner);
		  _animator = _go.GetComponent<Animator>();
		}
	 }


  }
}