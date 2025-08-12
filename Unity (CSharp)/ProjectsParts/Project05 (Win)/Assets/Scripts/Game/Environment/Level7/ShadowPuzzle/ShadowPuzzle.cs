using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Level8.ShadowPuzzle
{
  public class ShadowPuzzle : Environment
  {
	 /*
	  * Состояния:
	  * 0 - обычное
	  * 1 - игрок в фокусе
	  * 2 - пройдено
	  * 
	  */

	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _OnComplete;
	 [HideInInspector]
	 [SerializeField]
	 private List<ShadowPuzzleBlock> _shadowBlocks;
	 [SerializeField]
	 private Light[] _lights;
	 [SerializeField]
	 private Transform[] _laserPoint;

	 protected override void Start()
	 {
		base.Start();
		FindAndSubscribe();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		switch (State)
		{
		  case 0:
			 SetLightActive(false);
			 SetTriggerBlocks(false);
			 break;
		  case 1:
			 SetLightActive(true);
			 SetTriggerBlocks(true);
			 break;
		  case 2:
			 SetLightActive(false);
			 SetTriggerBlocks(false);
			 break;
		}

	 }

	 [ContextMenu("Find items")]
	 private void FindAndSubscribe()
	 {
		_shadowBlocks = new List<ShadowPuzzleBlock>();
		var blockArr = GetComponentsInChildren<ShadowPuzzleBlock>();

		for (int i = 0; i < blockArr.Length; i++)
		{
		  _shadowBlocks.Add(blockArr[i]);
		  blockArr[i].OnTriggerEnterEvent = OnTriggerEnterEvent;
		}
	 }

	 public void SetLightActive(bool setActive)
	 {
		for (int i = 0; i < _lights.Length; i++)
		{
		  _lights[i].enabled = setActive;
		}
	 }

	 private void OnTriggerEnterEvent(ShadowPuzzleBlock block)
	 {
		if (State != 1) return;

		if (!block.IsOpen)
		{
		  AttackPlay();
		}
	 }

	 private void AttackPlay()
	 {
		Debug.Log("Attack player");
	 }

	 public void PlayerEnter()
	 {
		if (State == 2) return;

		State = 1;
		ConfirmState();
	 }

	 public void PlayerExit()
	 {
		if (State == 2) return;

		State = 0;
		ConfirmState();
	 }

	 private void SetTriggerBlocks(bool focus)
	 {
		for (int i = 0; i < _shadowBlocks.Count; i++)
		{
		  var box = _shadowBlocks[i].GetComponent<BoxCollider>();

		  box.size = new Vector3((focus ? 1 - (Player.PlayerBehaviour.Instance.ActorController.Radius*2) : 1),
			 box.size.y,
			 (focus ? 1 - (Player.PlayerBehaviour.Instance.ActorController.Radius*2) : 1));
		}
	 }

	 /// <summary>
	 /// Евент окончания лабиринта
	 /// </summary>
	 public void Final()
	 {
		if (State == 2) return;

		_OnComplete?.Invoke();
		State = 2;
		ConfirmState();
	 }

  }
}