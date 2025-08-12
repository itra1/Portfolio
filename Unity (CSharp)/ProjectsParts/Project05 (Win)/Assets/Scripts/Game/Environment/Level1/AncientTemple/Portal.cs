using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Save;
using it.Game.Managers;

namespace it.Game.Environment.Level1.AncientTemple
{
  public class Portal : Environment, Game.Items.IInteraction
  {
	 /*
	  * Состояния:
	  * 0 - отключен
	  * 1 - включе
	  */
	 public bool IsInteractReady => State > 0;

	 [SerializeField]
	 private GameObject _activeGate;
	 public void ActivateGate()
	 {
		if (State == 1)
		  return;
		State = 1;
		Save();
		ConfirmState();
	 }

	 public void UseGate()
	 {
		//int levelNum = GameManager.Instance.UserManager.PlayerProgress.Level;

		//levelNum++;

		//GameManager.Instance.UserManager.PlayerProgress.Level = levelNum;
		//string sceneName = ProjectSettings.LevelScenes[levelNum];

		//GameManager.Instance.SceneManager.LoadScene(sceneName, true);


		it.Game.Managers.GameManager.Instance.NextLevel();

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		if(_activeGate != null)
		_activeGate.SetActive(State > 0);
	 }


	 public void StartInteract()
	 {
		if (State != 1)
		  return;

		State = 2;

		UseGate();
	 }

	 public void StopInteract()
	 {
	 }

  }
}