using System.Collections;

using UnityEngine;

namespace it.Game.Handles
{
  public class AirFallTrigger : TriggerPlayerHandler
  {

	 private void Start()
	 {
		onTriggerEnter.AddListener(() =>
		{
		  var airFall = Player.PlayerBehaviour.Instance.MotionController.GetMotion<it.Game.Player.MotionControllers.Motions.AirFall>();
		  Player.PlayerBehaviour.Instance.MotionController.ActivateMotion(airFall);
		});
	 }

  }
}