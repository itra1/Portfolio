using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.NPC
{
  public class NPCAnimMoveHelper : MonoBehaviour
  {

	 public Animator Animator
	 {
		get
		{
		  if (_animator == null)
			 _animator = GetComponent<Animator>();
		  return _animator;
		}
		set
		{
		  _animator = value;
		}
	 }
	 public com.ootii.Actors.ActorController Character
	 {
		get
		{
		  if (_character == null)
			 _character = GetComponent<com.ootii.Actors.ActorController>();
		  if (_character == null)
			 _character = GetComponentInParent<com.ootii.Actors.ActorController>();
		  return _character;
		}
		set
		{
		  _character = value;
		}
	 }

	 private Animator _animator;
	 private com.ootii.Actors.ActorController _character;

	 private void OnAnimatorMove()
	 {
		if (Character == null) return;

		Character.gameObject.SendMessage("OnAnimatorMove");
	 }

	 private void OnAnimatorIK(int layerIndex)
	 {

		if (Character == null) return;

		Character.gameObject.SendMessage("OnAnimatorIK", layerIndex);
	 }
  }
}