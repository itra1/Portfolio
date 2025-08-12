using UnityEngine;
using System.Collections;

namespace it.Game.CatScenes.L1
{
  public class OutGate : CatScene
  {
	 public override int Number => 2;
	 private void OnTriggerEnter(Collider other)
	 {
		if (other.GetComponent<Game.Player.PlayerBehaviour>() == null)
		  return;

		Play();

	 }

	 public override void SetPlay()
	 {
		var player = Game.Player.PlayerBehaviour.Instance;

		//player.CatScenePlay(Number);

		base.SetPlay();
	 }
	 public override void SetStop()
	 {
		base.SetStop();

		var player = Game.Player.PlayerBehaviour.Instance;
		//player.CatSceneStop();

		Debug.Log("End cat scene");

	 }

  }


}