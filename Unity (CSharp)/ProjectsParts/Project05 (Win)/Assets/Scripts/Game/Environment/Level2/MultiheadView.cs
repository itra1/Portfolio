using UnityEngine;
using System.Collections;
using it.Game.NPC.Enemyes;

namespace it.Game.Environment.Level2
{
  public class MultiheadView : Environment
  {

	 [SerializeField]
	 private PlayMakerFSM _startFSM;
	 [SerializeField]
	 private PlayMakerFSM _playerLoss;

	 [SerializeField]
	 private Transform _particles;
	 private Vector3 _startParticlesPosition;

	 private Multihead _enemy;
	 private Multihead Enemy
	 {
		get
		{
		  if (_enemy == null)
			 _enemy = GetComponentInChildren<Multihead>(true);
		  return _enemy;
		}
	 }

	 protected override void Start()
	 {
		_startParticlesPosition = _particles.position;
		base.Start();

		if(State == 0)
		{
		  ResetData();
		  Enemy.RestoreWaterLily();
		}

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		_startFSM.SendEvent("StopFSM");
		ResetData();

		if (State != 2)
		{
		  ResetData();
		  Enemy.RestoreWaterLily();
		}
	 }

	 /// <summary>
	 /// Первоначальный запуск анимации
	 /// </summary>
	 [ContextMenu("PlayerEnter")]
	 public void PlayerEnter()
	 {
		if (!_isActived) return;

		if (State != 0)
		  return;

		State = 1;
		_startFSM.SendEvent("StartFSM");


	 }

	 private void ResetData()
	 {

		if (Enemy != null)
		  Enemy.gameObject.SetActive(false);

		ParticleSystem[] particles = _particles.GetComponentsInChildren<ParticleSystem>();
		for(int i = 0; i < particles.Length; i++)
		{
		  var _emitionModule = particles[i].emission;
		  var _emitionCurve = _emitionModule.rateOverTime;
		  _emitionCurve.constant = 0;
		  _emitionModule.rateOverTime = _emitionCurve;
		}

	 }


	 /// <summary>
	 /// Игрок ушел
	 /// </summary>
	 [ContextMenu("PlayerOut")]
	 public void PlayerOut()
	 {
		if (State != 1)
		  return;

		State = 2;
		Save();
		Enemy.gameObject.SetActive(false);
	 }

	 [ContextMenu("PlayerLoss")]
	 public void PlayerContact()
	 {
		Debug.Log("PlayerContact");
		_playerLoss.SendEvent("StartFSM");
	 }

	 /// <summary>
	 /// Событие полного появления
	 /// </summary>
	 public void EnemyShowComplete()
	 {
		Enemy.FindParticle();
		//Enemy.ActiveBehaviourTree("Move To Player");
	 }

  }
}