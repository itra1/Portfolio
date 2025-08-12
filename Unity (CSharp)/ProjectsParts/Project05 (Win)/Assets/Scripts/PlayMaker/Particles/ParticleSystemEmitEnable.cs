using UnityEngine;
using System.Collections;
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory("particles")]
  public class ParticleSystemEmitEnable : FsmStateAction
  {
	 public FsmOwnerDefault gameObject;
	 public FsmBool active;

	 public override void OnEnter()
	 {
		ParticleSystem.EmissionModule mod = Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<ParticleSystem>().emission;
		mod.enabled = active.Value;
	 }

  }
}