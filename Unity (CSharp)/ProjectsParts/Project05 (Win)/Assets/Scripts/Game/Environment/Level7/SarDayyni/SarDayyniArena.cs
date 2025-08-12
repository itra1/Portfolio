using UnityEngine;
using System.Collections;
using it.Game.NPC.Enemyes.Boses.SarDayyni;
using DG.Tweening;

namespace it.Game.Environment.Level7.SarDayyni
{
  /// <summary>
  /// Арена босса
  /// </summary>
  public class SarDayyniArena : Environment
  {
	 /*
	  * Состояния:
	  * 0 - ожидание
	  * 1 - фаза ожидания активации панелей
	  */

	 public NPC.Enemyes.Boses.SarDayyni.SarDayyni Enemy { get => _enemy; set => _enemy = value; }
	 public ControlPanel[] ControlPanels { get => _controlPanels; set => _controlPanels = value; }
	 public CoilElectric[] CoilElectric { get => _coilElectric; set => _coilElectric = value; }
	 public Obelisk[] Obelisks { get => _obelisks; set => _obelisks = value; }

	 [SerializeField] private it.Game.NPC.Enemyes.Boses.SarDayyni.SarDayyni _enemy;
	 [SerializeField] private ControlPanel[] _controlPanels;
	 [SerializeField] private CoilElectric[] _coilElectric;
	 [SerializeField] private Obelisk[] _obelisks;
	 [SerializeField] private AnimationCurve _curve;
	 [SerializeField] public Renderer _bossShield;
	 [SerializeField] private Transform _center;
	 [SerializeField] private SarDayyniShield _shield;
	 [SerializeField] private Slate.Cutscene _firstCutScene;

	 private Vector3 _centerStart;
	 private SarDayyaniRoom _room;
	 private Vector3 _startEnemyPosition;

	 protected override void Awake()
	 {
		base.Awake();
		_room = GetComponent<SarDayyaniRoom>();
		_controlPanels = gameObject.GetComponentsInChildren<ControlPanel>();
		_coilElectric = gameObject.GetComponentsInChildren<CoilElectric>();
		_obelisks = gameObject.GetComponentsInChildren<Obelisk>();
		_startEnemyPosition = _enemy.transform.position;
		_centerStart = _center.position;
	 }

	 protected override void Start()
	 {
		base.Start();

		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.InventaryGetItem, InventaryAddItem);

		for (int i = 0; i < _controlPanels.Length; i++)
		  _controlPanels[i].onActivate = ControlPanelActivate;
		for (int i = 0; i < _coilElectric.Length; i++)
		  _coilElectric[i].OnActivate = CoilsActivate;
		for (int i = 0; i < _obelisks.Length; i++)
		  _obelisks[i].onActivate = ObelisksActivate;

		Clear();

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if(State == 0)
		  {
			 Clear();
		  }
		}

	 }

	 protected override void OnDestroy()
	 {
		base.OnDestroy();
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.InventaryGetItem, InventaryAddItem);
	 }

	 private void InventaryAddItem(com.ootii.Messages.IMessage handle)
	 {
		string itemUuid = (string)handle.Data;

		if (itemUuid != "a929077e-3e35-4593-b33b-9ad2e2628865")
		  return;

		Phase3();

	 }

	 /// <summary>
	 /// Событие входа в арену
	 /// </summary>
	 public void PlayerEnterArena()
	 {
		if (State != 0) return;

		StartBattle();
	 }

	 public void StartBattle()
	 {
		State = 1;
		Managers.GameManager.Instance.GameInputSource.IsEnabled = false;
		_firstCutScene.Play(() =>
		{
		  Managers.GameManager.Instance.GameInputSource.IsEnabled = true;
		  State = 1;
		  //_enemy.Behaviour.FsmVariables.GetFsmInt("State").Value = 1;
		  //_enemy.Behaviour.SendEvent("StartBattle");
		});
	 }

	 private void ControlPanelActivate()
	 {
		for (int i = 0; i < _controlPanels.Length; i++)
		  if (!_controlPanels[i].IsActive)
			 return;

		Phase2();
	 }
	 private void CoilsActivate()
	 {
		for (int i = 0; i < _coilElectric.Length; i++)
		  if (!_coilElectric[i].IsActivated)
			 return;

		Phase4();
	 }
	 private void ObelisksActivate()
	 {
		for (int i = 0; i < _obelisks.Length; i++)
		  if (!_obelisks[i].IsActivated)
			 return;

		Phase5();
	 }
	 /// <summary>
	 /// Переход в фазу 2
	 /// </summary>
	 private void Phase2()
	 {
		State = 2;
		_enemy.Behaviour.FsmVariables.GetFsmInt("State").Value = 2;
		_shield.Open();
	 }
	 /// <summary>
	 /// Переход в фазу 3
	 /// </summary>
	 private void Phase3()
	 {
		State = 3;

	 }

	 private void Phase4()
	 {
		State = 4;
		_center.DOMoveY(_centerStart.y - 20, 5).SetEase(_curve).OnComplete(() =>
		{
		  _bossShield.material.DOFloat(0, "_Dissolve", 0).OnComplete(() =>
		  {
			 _enemy.transform.DOMoveY(_startEnemyPosition.y - 20, 5).SetEase(_curve).OnComplete(() =>
			 {
				_enemy.Behaviour.FsmVariables.GetFsmInt("State").Value = 3;
			 });
		  });
		});

	 }

	 private void Phase5()
	 {
		State = 5;
		Debug.Log("Battle complete");
	 }

	 private void Clear()
	 {
		for (int i = 0; i < _controlPanels.Length; i++)
		  _controlPanels[i].Clear();
		for (int i = 0; i < _coilElectric.Length; i++)
		  _coilElectric[i].Clear();
		for (int i = 0; i < _obelisks.Length; i++)
		  _obelisks[i].Clear();

		_bossShield.material.SetFloat("_Dissolve", 1);
		_enemy.Behaviour.SendEvent("StopBattle");
		_enemy.Behaviour.FsmVariables.GetFsmInt("State").Value = 1;
		_center.position = _centerStart;
		_enemy.transform.position = _startEnemyPosition;
		_shield.Clear();
		_room.Shadow();
	 }

  }
}