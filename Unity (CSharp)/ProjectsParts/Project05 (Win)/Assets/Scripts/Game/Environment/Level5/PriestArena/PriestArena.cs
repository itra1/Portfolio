using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level5.PriestArena
{
  /// <summary>
  /// Арена приста
  /// </summary>
  public class PriestArena : Environment
  {
	 /*
	  * Состоние:
	  * 
	  * 0 - отключена
	  * 1 - ждет игрока
	  * 2 - бой
	  * 
	  */

	 [SerializeField] private it.Game.NPC.Enemyes.Priest _priest;
	 [SerializeField] private GameObject _sphere;
	 [SerializeField] private PriestAbelisk[] _abelisks;
	 [SerializeField] private Slate.Cutscene _firstCutscene;
	 [SerializeField] private GameObject _playerTrigger;
	 public PriestAbelisk[] Abelisks { get => _abelisks; set => _abelisks = value; }

	 private string _priestFsm = "Behaviour";
	 private int _battleState = 0;
	 private DungeonBossArena _baseArena;

	 protected override void Awake()
	 {
		base.Awake();
		_baseArena = GetComponentInParent<DungeonBossArena>();

		_abelisks = GetComponentsInChildren<PriestAbelisk>();
		for (int i = 0; i < _abelisks.Length; i++)
		  _abelisks[i].OnActivate = InteractObelisk;
	 }

	 protected override void Start()
	 {
		base.Start();

	 }


	 /// <summary>
	 /// Подготовка арены
	 /// </summary>
	 [ContextMenu("BattleReady")]
	 public void BattleReady()
	 {
		if (State != 0)
		  return;

		SetPhase1();
		Save();
	 }

	 private void SetPhase0()
	 {
		Debug.Log("Установка фазы 0");
		State = 0;
		_priest.gameObject.SetActive(false);
		for (int i = 0; i < _abelisks.Length; i++)
		  _abelisks[i].SetState0();

		_baseArena.OpenFront(true, false);
		_baseArena.OpenRight(true, false);
		_baseArena.OpenLeft(true, false);

		_playerTrigger.gameObject.SetActive(false);

	 }

	 private void SetPhase1()
	 {
		Debug.Log("Установка фазы 1");
		State = 1;
		_priest.gameObject.SetActive(true);
		for (int i = 0; i < _abelisks.Length; i++)
		  _abelisks[i].SetState1();

		_baseArena.OpenFront(true, false);
		_baseArena.OpenRight(true, false);
		_baseArena.OpenLeft(true, false);

		_playerTrigger.SetActive(true);
	 }
	 private void SetPhase2()
	 {
		Debug.Log("Установка фазы 2");
		_playerTrigger.SetActive(false);

		Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = false;
		_firstCutscene.Play(() =>
		{
		  Game.Managers.GameManager.Instance.GameInputSource.IsEnabled = true;
		  State = 2;
		  _priest.GetFsm(_priestFsm).FsmVariables.GetFsmGameObject("AttackTarget").Value = Player.PlayerBehaviour.Instance.gameObject;
		  PriestPhase(0);
		});
	 }

	 private void SetPhase3()
	 {
		Debug.Log("Установка фазы 3");
		State = 3;
		_priest.gameObject.SetActive(true);
		for (int i = 0; i < _abelisks.Length; i++)
		  _abelisks[i].SetState2();
	 }

	 /// <summary>
	 /// Вход игрока в триггер активации
	 /// </summary>
	 public void PlayerEnter()
	 {

		if (State != 1) return;

		SetPhase2();

		//_priest.GetFsm(_priestFsm).FsmVariables.GetFsmGameObject("AttackTarget").Value = Player.PlayerBehaviour.Instance.gameObject;

		//State = 2;
		//Save();
		//PriestPhase(0);
	 }

	 private void PriestPhase(int battleState)
	 {
		_battleState = battleState;
		var fsm = _priest.GetFsm(_priestFsm);
		fsm.FsmVariables.GetFsmInt("BattleState").Value = _battleState;
		fsm.SendEvent("OnBattle");
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (!isForce) return;

		if (State == 0)
		  SetPhase0();

		if (State == 1)
		  SetPhase1();

		if (State == 2)
		  SetPhase2();

		if (State == 3)
		  SetPhase3();

	 }

	 private void InteractObelisk()
	 {
		if (_battleState != 0)
		  return;

		int countActive = _abelisks.Length;
		for (int i = 0; i < _abelisks.Length; i++)
		{
		  if (!_abelisks[i].IsActive)
			 countActive--;
		}

		if (countActive <= 2)
		{
		  PriestPhase(1);
		}

	 }

  }
}