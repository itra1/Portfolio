using UnityEngine;
using System.Collections;
namespace it.Game.Environment.All
{

  /// <summary>
  /// Конечная активация объектов при появлении игрока
  /// </summary>
  [AddComponentMenu("Environment/All/ActivateGameObjectsOnPlayerEnter")]
  [RequireComponent(typeof(Game.Handles.PlayerEnterSection))]
  public class ActivateGameObjectsOnPlayerEnter : Environment
  {
	 /*
	  * Состояния
	  * 0 - не активны
	  * 1 - активны
	  */
	 [SerializeField]
	 private GameObject[] _gameObjects;

	 protected override void Start()
	 {
		base.Start();
		var playerEnterСomp = GetComponent<Game.Handles.PlayerEnterSection>();
		playerEnterСomp.onPlayerEnter = PlayerEnter;
		ActivateObjects(true);
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		ActivateObjects(State >= 1);
	 }

	 private void PlayerEnter()
	 {
		State = 1;
		ConfirmState();
		Save();
	 }

	 private void ActivateObjects(bool active)
	 {
		foreach (var elem in _gameObjects)
		  elem.SetActive(active);
	 }

  }
}