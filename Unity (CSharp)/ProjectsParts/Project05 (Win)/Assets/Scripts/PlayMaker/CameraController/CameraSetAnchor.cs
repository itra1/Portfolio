using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
using com.ootii.Cameras;

namespace it.Game.PlayMaker.Camera
{
  public class CameraSetAnchor : CameraControllerBase
  {
	 public FsmGameObject anchor;
	 public override void OnEnter()
	 {
		base.OnEnter();
		_controller = _camera.Value.GetComponent<CameraController>();
		if (anchor.Value == null)
		  _controller.Anchor = null;
		else
		  _controller.Anchor = anchor.Value.transform;
	 }
  }
}