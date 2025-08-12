using UnityEngine;
using System.Collections;
using it.Game.Player;
using it.Game.Environment.Handlers;
using DG.Tweening;

namespace it.Game.Environment.Level1
{
  /// <summary>
  /// Состояние:
  /// 
  /// 0 - ждемс
  /// 1 - первое появление
  /// </summary>
  public class DeerGhostFirst : Environment
  {
	 [SerializeField]
	 private GameObject _deerPrefab;
	 [SerializeField]
	 private PlayMakerFSM _playMakerFsm;
	 [SerializeField]
	 private Transform _firstSpawn;
	 [SerializeField]
	 private Transform _scullSpawn;
	 [SerializeField]
	 private Transform _finalSpawn;

	 [SerializeField]
	 private PegasusController _pegasus;

	 private GameObject _instDeer;

	 public GameObject InstDeer
	 {
		get => _instDeer;
		set => _instDeer = value;
	 }

	 private void CreateDeer()
	 {
		if (_instDeer != null)
		  return;

		_instDeer = Instantiate(_deerPrefab, transform);
		_playMakerFsm.FsmVariables.GetFsmGameObject("Deer").Value = InstDeer;
	 }

	 public void FirstTrigger()
	 {
		if (State > 0) return;

		//State = 1;
		Show1();
		State = 2;
		Save();
	 }

	 private void Show1()
	 {
		CreateDeer();
		InstDeer.transform.position = _firstSpawn.position;
		_playMakerFsm.SendEvent("Show1");
	 }

	 private void Show2()
	 {
		CreateDeer();
		InstDeer.transform.position = _scullSpawn.position;
		_playMakerFsm.SendEvent("Show2");
	 }

	 private void Show3()
	 {
		if (State >= 4) return;
		State = 4;
		Save();
		CreateDeer();
		InstDeer.transform.position = _finalSpawn.position;
		_playMakerFsm.SendEvent("Show3");
	 }

	 public void NoteReadComplete()
	 {
		if (State >= 3) return;
		State = 3;
		Save();
		Managers.GameManager.Instance.GameInputSource.enabled = false;
		_pegasus.Activate(() =>
		{
		  _playMakerFsm.SendEvent("NoteReadComplete");
		  DOVirtual.DelayedCall(2, () =>
		  {
			 _pegasus.Deactivate();
			 Managers.GameManager.Instance.GameInputSource.enabled = true;
		  });
		});
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if (State == 1)
		  {
			 Show1();
		  }
		  if (State == 2)
		  {
			 Show2();
		  }
		  if (State == 3)
		  {
			 Show3();
		  }
		  if (State == 4)
		  {
			 if (_instDeer != null)
				_instDeer.gameObject.SetActive(false);
		  }
		}

	 }

  }
}