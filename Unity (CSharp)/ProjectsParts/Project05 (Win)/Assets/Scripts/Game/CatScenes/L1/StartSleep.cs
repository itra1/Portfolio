using UnityEngine;
using System.Collections;

namespace it.Game.CatScenes.L1
{
  public class StartSleep : CatScene
  {
	 public override int Number => 1;

	 protected override void Start()
	 {
		base.Start();
		SetReady();
	 }

	 //private void OnTriggerEnter(Collider other)
	 //{
		//if (other.GetComponent<Game.Player.PlayerBehaviour>() == null)
		//  return;

		//Play();
		//// Сразу факсируем полный просмотр
		//State = 3;
		//Save();
	 //}
	 //private void OnTriggerStay(Collider other)
	 //{
		//if (other.GetComponent<Game.Player.PlayerBehaviour>() == null)
		//  return;

		//if (State == 1)
		//  Play();
	 //}

	 public void PlayerEnter()
	 {

		Play();
		// Сразу факсируем полный просмотр
		State = 3;
		Save();
	 }

	 public override void SetPlay()
	 {
		var player = Game.Player.PlayerBehaviour.Instance;

		//player.CatScenePlay(Number);

		base.SetPlay();

		InvokeSeconds(Stop, 35f);
	 }
	 public override void SetStop()
	 {
		base.SetStop();

		var player = Game.Player.PlayerBehaviour.Instance;
		//player.CatSceneStop();

	 }




	 #region Save

	 #endregion
  }
}