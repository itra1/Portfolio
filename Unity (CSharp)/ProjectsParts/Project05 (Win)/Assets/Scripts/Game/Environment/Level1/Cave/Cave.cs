using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level1.Cave
{
  public class Cave : Environment
  {
	 /*
	  * Состояния
	  * 0 - закрыто
	  * 1 - открыто
	  * 2 - пройдены
	  * 
	  */

	 private bool _playerInDistance = false;

	 //[SerializeField]
	 //private Collider _particleCollider;
	 [SerializeField]
	 private Game.Handles.TriggerHandler _playerTrigger;
	 [SerializeField]
	 private Game.Handles.TriggerHandler _outTrigger;
	 //[SerializeField]
	 //private LivingParticleController _particleController;
	 public GameObject barrier;

	 protected override void Start()
	 {
		base.Start();
		EnabledColliders(false);
		//_particleController.affector.transform.position = transform.position + Vector3.up * 10;


		var comp = GetComponent<Game.Handles.CheckToPlayerDistance>();
		comp.onPlayerInDistance = () =>
		{
		  _playerInDistance = true;
		  EnabledColliders(true);
		};

		comp.onPlayerOutDistance = () =>
		{
		  _playerInDistance = false;
		  //_particleController.affector.transform.position = transform.position + Vector3.up * 10;
		  EnabledColliders(false);
		};

		_outTrigger.onTriggerEnter.AddListener((col) =>
		{
		  if (col.GetComponent<Player.PlayerBehaviour>() != null)
		  {
			 State = 2;
			 ChangeCollider();
			 Save();
		  }
		});
		

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		//if (State != 1)
		//  _particleController.affector.transform.position = transform.position + Vector3.up * 10;

		if (State == 0)
		{
		  barrier.SetActive(true);
		  barrier.transform.localScale = new Vector3(5.88f, 5.88f, 5.88f);
		}
		if (State > 0)
		  barrier.SetActive(false);

		if (isForce && State > 0)
		  EnabledColliders(_playerInDistance);

	 }

	 private void EnabledColliders(bool enable)
	 {
		_playerTrigger.GetComponent<Collider>().enabled = enable;
		_outTrigger.GetComponent<Collider>().enabled = enable;
		//_particleCollider.enabled = !(State == 1 && enable);
	 }

	 private void Update()
	 {
		if (!_playerInDistance || (State != 1))
		  return;

		//_particleController.affector.transform.position
		//  = Game.Managers.GameManager.Instance.UserManager.PlayerBehaviour.transform.position;
	 }

	 private void ChangeCollider()
	 {
		//_particleCollider.enabled = (State == 1);
	 }

	 [ContextMenu("Open")]
	 public void SetOpen()
	 {
		State = 1;
		barrier.transform.DOScale(Vector3.zero, 1).OnComplete(() => { barrier.SetActive(false); });
		
		ChangeCollider();
		Save();
	 }

  }
}