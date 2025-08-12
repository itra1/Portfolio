using UnityEngine;
using System.Collections;
namespace it.Game.Environment.All.Doors
{
  public sealed class DoorStandart : Door
  {
	 [SerializeField]
	 private Animator _animator;

	 protected override void ConfirmState(bool isForce = false)
	 {

		_animator.SetBool("Open", State == 1);
		if(isForce)
		_animator.SetTrigger("Force");

		base.ConfirmState(isForce);
	 }

  }
}