#define CUT_SCENES
#define BOSS

using UnityEngine;
using System.Collections;
using Slate;
using it.Game.Managers;
using com.ootii.Actors;

namespace it.Game.Environment.Level6.Gishtil
{
  public class GishtilBattle : Environment
  {
	 /*
	  * Состояния
	  * 0 - бездействие
	  * 1 - первая часть боя
	  * 2 - вторая часть боя
	  * 3 - победа игрока
	  */

	 [SerializeField]
	 protected GishtilBossArena _arena;
	 [SerializeField]
	 protected it.Game.NPC.Enemyes.Boses.Hunter.Gishtil _enemy;

	 [SerializeField]
	 private Transform _spawnPosition;

	 [SerializeField]
	 private Cutscene _startCutScene;

	 [SerializeField]
	 private Cutscene _phaseChangeCutScene;
	 [SerializeField]
	 private Cutscene _phaseDeadCutScene;

	 private PlayMakerFSM _behaviourFSM;
	 private ActorController _enemyActor;

	 protected override void Start()
	 {
		_behaviourFSM = _enemy.GetFsm("Behaviour");
		_enemyActor = _enemy.GetComponent<ActorController>();
		base.Start();
		Clear();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		if (State == 0)
		  Clear();
	 }

	 /// <summary>
	 /// Сброс состояния босс арены
	 /// </summary>
	 public void Clear()
	 {
		_arena.Clear();
		_enemy.gameObject.SetActive(false);

		GishtilRoundBullet bul = gameObject.GetComponentInChildren<GishtilRoundBullet>(true);
		if (bul != null)
		  bul.gameObject.SetActive(false);
	 }

	 public void ActiveEnemy()
	 {
		_enemy.gameObject.SetActive(true);
	 }

	 protected override void OnStateChange()
	 {
		base.OnStateChange();
		_behaviourFSM.SendEvent("PhaseChange");
		_behaviourFSM.FsmVariables.GetFsmInt("State").Value = State;
	 }

	 /// <summary>
	 /// Событие, что все лучи включены
	 /// </summary>
	 [ContextMenu("AllLightingActive")]
	 public void AllLightingActive()
	 {
		if (State >= 2 || IsCutScene)
		  return;

#if CUT_SCENES

		IsCutScene = true;
		GameManager.Instance.GameInputSource.IsEnabled = false;
		_phaseChangeCutScene.Play(() =>
		{
		  _phaseChangeCutScene.Stop();
		  IsCutScene = false;
		  GameManager.Instance.GameInputSource.IsEnabled = true;
		  SetPhase2();
		});
#else
		SetPhase2();
#endif
	 }
	 /// <summary>
	 /// Игрок входит на арену
	 /// </summary>
	 [ContextMenu("PlayerEnterArena")]
	 public void PlayerEnterArena()
	 {
#if UNITY_EDITOR

		if (_isDebug)  return;

#endif

		if (State > 0 || IsCutScene) return;

		_enemy.transform.position = _spawnPosition.position;
		_enemy.transform.rotation = _spawnPosition.rotation;

		_enemyActor.SetVelocity(Vector3.zero);
		_enemyActor.SetRotationVelocity(Vector3.zero);

#if CUT_SCENES

		IsCutScene = true;
		GameManager.Instance.GameInputSource.IsEnabled = false;
		_startCutScene.Play(() =>
		{
		  _startCutScene.Stop();
		  IsCutScene = false;
		  GameManager.Instance.GameInputSource.IsEnabled = true;
		  SetPhase1();
		});

#else
		SetPhase1();
#endif

	 }

	 private void SetPhase1()
	 {
		State = 1;
		_behaviourFSM.FsmVariables.GetFsmGameObject("AttackTarget").Value = Player.PlayerBehaviour.Instance.gameObject;
		_behaviourFSM.SendEvent("OnBattle1");
		Debug.Log("Бой начался");
	 }

	 private void SetPhase2()
	 {
		State = 2;
		Debug.Log("Бой перешел во вторую фазу");
	 }

	 private void SetPhase3()
	 {
		State = 3;
		Debug.Log("Бой закончен");
	 }


	 [ContextMenu("BattleComplete")]
	 public void BattleComplete()
	 {
		if (State == 3 || IsCutScene)	  return;

#if CUT_SCENES

		IsCutScene = true;
		GameManager.Instance.GameInputSource.IsEnabled = false;
		_phaseDeadCutScene.Play(() =>
		{
		  _phaseDeadCutScene.Stop();
		  IsCutScene = false;
		  SetPhase3();
		  GameManager.Instance.GameInputSource.IsEnabled = true;
		});
#else
		SetPhase3();
#endif

	 }

  }
}