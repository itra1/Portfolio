using System.Collections.Generic;
using it.Game.Player.Save;
using UnityEngine;
using UnityEngine.UI;
using Leguar.TotalJSON;
using DG.Tweening;
using it.Game.Environment.Handlers;

namespace it.Game.Environment.Challenges.KingGameOfUr
{

  /// <summary>
  /// Менеджер королеввкой игры Ур
  /// </summary>
  public class KingGameOfUr : Challenge
  {

	 /*
	  * Состояния
	  * 0 - начало
	  * 1 - готов
	  * 2 - активный
	  * 3 - выполненый
	  */

	 private PhaseType _Phase;
	 public Piramide m_PiramideRight;
	 public Piramide m_PiramideLeft;
	 [SerializeField]
	 private Light _buttonLight;

	 [SerializeField]
	 private Transform _cameraPosition;
	 [SerializeField]
	 private float _distancePlayerCheck = 9;

	 private bool m_isWait = false;

	 public bool IsWait
	 {
		get
		{
		  return m_isWait;
		}
	 }

	 public override bool IsInteractReady => (State <= 1);

	 [SerializeField]
	 private List<Section> m_ListSection = new List<Section>();

	 [SerializeField]
	 private User m_User = new User();
	 [SerializeField]
	 private Enemy m_Enemy = new Enemy();

	 [SerializeField]
	 private GameObject _gameActivator;

	 protected override void Start()
	 {
		base.Start();
		var comp = GetComponent<Game.Handles.CheckToPlayerDistance>();

		comp.onPlayerInDistance = () =>
		{

		};

		comp.onPlayerOutDistance = () =>
		{
		  if (State == 2)
		  {
			 StopInteract();
		  }
		};
		_buttonLight.intensity = 0f;
	 }

	 [ContextMenu("SetReady")]
	 public void SetReady()
	 {
		if (State != 0)
		  return;

		State = 1;

		_buttonLight.DOIntensity(3.4f, 1);
		Save();

	 }

	 [ContextMenu("Play")]
	 public void Play()
	 {
		m_isWait = false;
		ResetGame();
		StartUserStep();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		  _gameActivator.SetActive(State < 2);

		if (State == 1 || State == 2)
		  _buttonLight.intensity = 3.4f;
		else
		  _buttonLight.intensity = 0f;
	 }

	 private void ResetGame()
	 {
		m_User.Initiate(this);
		m_Enemy.Initiate(this);
	 }

	 public enum PhaseType
	 {
		user,
		enemy
	 }

	 public void StartUserStep()
	 {

		_Phase = PhaseType.user;
		m_User.Step(StepStart, StepComplete, OnMoveNextSection);

	 }

	 public void StartEnemyStep()
	 {
		_Phase = PhaseType.enemy;
		m_Enemy.Step(StepStart, StepComplete, OnMoveNextSection);
	 }

	 public void StepStart()
	 {
		m_isWait = true;
		ClearButtons();
	 }

	 [ContextMenu("UserWin")]
	 public void UserWin()
	 {
		var pegasusObject = transform.Find("Pegasus");
		
		if(pegasusObject != null)
		{
		  var pegasus = pegasusObject.GetComponent<PegasusController>();
		  pegasus.Activate(() =>
		  {

			 DOVirtual.DelayedCall(1, () =>
			 {
				ConfirmWin();
			 });

			 DOVirtual.DelayedCall(3, () =>
			 {
				pegasus.Deactivate();
			 });

		  });
		}
		else
		{
		  ConfirmWin();
		}
	 }

	 private void ConfirmWin()
	 {

		State = 3;
		OnComplete?.Invoke();
		Save();
		StopInteract();
	 }
	 public void EnemyWin()
	 {
		StopInteract();
		OnFailed?.Invoke();
		Save();
	 }

	 public void StepComplete()
	 {

		if (_Phase == PhaseType.user)
		{
		  if (m_User.IsComplete())
		  {
			 UserWin();
			 return;
		  }
		}
		else
		{
		  if (m_Enemy.IsComplete())
		  {
			 EnemyWin();
			 return;
		  }
		}

		// Проверка на соответствие точки
		if (_Phase == PhaseType.user)
		{
		  if (m_Enemy.CheckEqualsSection(StepComplete, m_User.ActualSection))
			 return;
		}
		else
		{
		  if (m_User.CheckEqualsSection(StepComplete, m_Enemy.ActualSection))
			 return;
		}

		// Переход хода
		m_isWait = false;
		if (_Phase == PhaseType.user)
		{

		  if (m_User.LastMove && m_User.IsSuperSection)
		  {
			 StartUserStep();
			 return;
		  }

		  StartEnemyStep();
		}
		else
		{

		  if (m_Enemy.LastMove && m_Enemy.IsSuperSection)
		  {
			 StartEnemyStep();
			 return;
		  }
		  StartUserStep();
		}
	 }

	 public void OnMoveNextSection(string uuid, bool isFinal)
	 {
		if (_Phase == PhaseType.user)
		{
		  if (!isFinal)
			 m_Enemy.Chip.SetTransparent(m_Enemy.CheckInOne(uuid));
		  else
			 m_Enemy.Chip.SetTransparent(false);
		}
		else
		{
		  if (!isFinal)
			 m_User.Chip.SetTransparent(m_User.CheckInOne(uuid));
		  else
			 m_User.Chip.SetTransparent(false);
		}

	 }

	 public void ClearButtons()
	 {
		m_PiramideRight.Clear();
		m_PiramideLeft.Clear();
	 }
	 /// <summary>
	 /// Использование кнопок терминала
	 /// </summary>
	 /// <param name="num">Кнопки: 0 - левая, 1 - правая</param>
	 public void OnUseButton(int num)
	 {
		//if(State == 0)
		//{
		//  StartInteract();
		//  return;
		//}

		if (State != 2)
		  return;

		if (!(!m_isWait && _Phase == PhaseType.user))
		  return;
		m_User.ButtonClick(m_User.Buttons[num]);
	 }

	 public override void StartInteract()
	 {
		if (State != 1)
		  return;
		base.StartInteract();
		State = 2;
		_gameActivator.SetActive(false);
		FocusCamera(_cameraPosition, 0.5f);
		Play();
	 }

	 public override void StopInteract()
	 {
		base.StopInteract();

		if (State == 2)
		{
		  State = 1;
		  ResetGame();
		}
		UnFocusCamera(0.5f);
	 }

	 #region Save

	 protected override JValue SaveData()
	 {
		JSON saveData = new JSON();

		JSON saveDataEnemy = new JSON();
		saveDataEnemy.Add("x", m_Enemy.Chip.transform.position.x);
		saveDataEnemy.Add("y", m_Enemy.Chip.transform.position.y);
		saveDataEnemy.Add("z", m_Enemy.Chip.transform.position.z);
		saveData.Add("enemy", saveDataEnemy);

		JSON saveDataPlayer = new JSON();
		saveDataPlayer.Add("x", m_User.Chip.transform.position.x);
		saveDataPlayer.Add("y", m_User.Chip.transform.position.y);
		saveDataPlayer.Add("z", m_User.Chip.transform.position.z);
		saveData.Add("player", saveDataPlayer);

		return saveData;
	 }

	 protected override void LoadData(Leguar.TotalJSON.JValue data)
	 {
		base.LoadData(data);

		JSON loadData = data as JSON;

		m_User.Chip.transform.position = new Vector3(loadData.GetJSON("player").GetFloat("x"),
		  loadData.GetJSON("player").GetFloat("y"),
		  loadData.GetJSON("player").GetFloat("z"));
		m_Enemy.Chip.transform.position = new Vector3(loadData.GetJSON("enemy").GetFloat("x"),
		  loadData.GetJSON("enemy").GetFloat("y"),
		  loadData.GetJSON("enemy").GetFloat("z"));
	 }


	 #endregion
  }

}