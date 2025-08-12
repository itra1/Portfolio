using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using Utilites.Geometry;
using it.Game.Environment.Handlers;

namespace it.Game.PlayMaker
{
  public abstract class PegasusBase : FsmStateAction
  {
	 [RequiredField]
	 public FsmGameObject _pegasusController;

	 protected PegasusController _pegasus;
	 public override void OnEnter()
	 {
		_pegasus = _pegasusController.Value.GetComponent<PegasusController>();
	 }
  }
}