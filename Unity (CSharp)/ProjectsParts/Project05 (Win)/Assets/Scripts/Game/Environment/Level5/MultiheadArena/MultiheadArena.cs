using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace it.Game.Environment.Level5.MultiheadArena
{
  /// <summary>
  /// Арена с многоголовым
  /// </summary>
  public class MultiheadArena : Environment
  {
	 [SerializeField] private NPC.Enemyes.Multihead _enemy;
	 [SerializeField] private Transform _startPosition;
	 [SerializeField] private Transform _endPosition;
	 [SerializeField] private GameObject _boxesObjects;
	 [SerializeField] private Transform _content;

	 private GameObject _boxInst;

	 protected override void Start()
	 {
		base.Start();
		Clear();
	 }

	 private void GetInstanceBoxes()
	 {
		_boxesObjects.gameObject.SetActive(false);

		if (_boxInst != null)
		  Destroy(_boxInst);

		_boxInst = Instantiate(_boxesObjects);
		gameObject.SetActive(true);
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (State == 0)
		  Clear();

	 }

	 private void Clear()
	 {
		_enemy.gameObject.SetActive(false);
		GetInstanceBoxes();
	 }

	 [ContextMenu("Play")]
	 public void Play()
	 {
		if (State != 0)
		  return;

		State = 1;
		SpawnEnemy();

	 }

	 private void SpawnEnemy()
	 {
		_enemy.transform.position = _startPosition.position;
		_enemy.transform.rotation = _startPosition.rotation;
		_enemy.GetComponent<com.ootii.Actors.ActorController>().SetPosition(_enemy.transform.position);
		_enemy.GetComponent<com.ootii.Actors.ActorController>().SetRotation(_enemy.transform.rotation);
		_enemy.gameObject.SetActive(true);
		PlayMakerFSM fsm = _enemy.GetFsm("Level5");
		fsm.SendEvent("Battle");
	 }

	 /// <summary>
	 /// Секция выхода
	 /// </summary>
	 public void MultiheadExit()
	 {

	 }

	 /// <summary>
	 /// Игрок покинул арену
	 /// </summary>
	 public void PlayerExitGate()
	 {

	 }

  }
}