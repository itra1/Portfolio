using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Ссылки на созданные экземпляры игроков
/// </summary>
[System.Serializable]
public struct PlayersInstance {
	public Player player;                 // Ссылка на контроллер игрока
	public float timeUpdateInfo;          // Время последнего обновления информации
}

/// <summary>
/// Боевая сцена
/// </summary>
public class BattleScene : MonoBehaviour {

	/// <summary>
	/// Событие изменения фазы боя
	/// </summary>
	public static event Actione<BattlePhase> ChangeBattlePhase;
	public static event Actione<TeamResult> OnBattleEnd;
	/// <summary>
	/// Событие изменение таймера уровня
	/// </summary>
	/// <param name="timeEndBattle">Время окончания уровня</param>
	public static event Actione<float> OnChangeBattleTimer;

	public static BattleScene instance;               // Ссылка на экземпляр плеера
	public BattlePhase battlePhase;                   // Текущая фаза боя

	public Player winner { get; set; }

	void Start() {
		BattlePhaseChange(BattlePhase.load);
		instance = this;

		EnergyInit();
		ResultsBattle.OnBattleResult += OnBattleResult;
		PlayersManager.OnPlayerDead += OnPlayerDead;
		TimerInfo.OnTimeFromServer += OnChangeTimeInfo;
	}

	void OnDestroy() {
		instance = null;
		ResultsBattle.OnBattleResult -= OnBattleResult;
		PlayersManager.OnPlayerDead -= OnPlayerDead;
		TimerInfo.OnTimeFromServer -= OnChangeTimeInfo;
	}

	/// <summary>
	/// Изменение таймера приходящее с сервера
	/// </summary>
	/// <param name="data"></param>
	public void OnChangeTimeInfo(TimeInfoData data) {
		if(data.type == TimerType.StartTimer) BattlePhaseChange(BattlePhase.ready);
		if(data.type == TimerType.GeneralTimer) BattlePhaseChange(BattlePhase.battle);
		//realtimeSinceStartup = Time.realtimeSinceStartup;
		secondLeft = data.secondLeft;
	}

	void Update() {
		TimeBattleCalc();
	}

	/// <summary>
	/// Событие изменения фазы боя
	/// </summary>
	/// <param name="newBattlePhase">Новая фаза боя</param>
	public void BattlePhaseChange(BattlePhase newBattlePhase) {
		battlePhase = newBattlePhase;
		if(ChangeBattlePhase != null) ChangeBattlePhase(battlePhase);
	}

	/// <summary>
	/// Обработка персонажей
	/// </summary>
	/// <param name="itFirstPlayer"></param>
	void OnPlayerDead(bool itFirstPlayer) {
		if(battlePhase != BattlePhase.battle) return;
	}

	void OnBattleResult(List<TeamResult> resultData) {

		int playerTeam = GameManager.instance.playerJoined.Find(x=>x.messageId == User.instance.gameProfile.id).team;
		TeamResult resultTeam = resultData.Find(x=>x.teamId == playerTeam);

		BattlePhaseChange(BattlePhase.finished);
		Helpers.Invoke(this, EndBattle, 3f);
		if(OnBattleEnd != null) OnBattleEnd(resultTeam);
	}


	/// <summary>
	/// Перезагрузка сцены
	/// </summary>
	void EndBattle() {
		SceneManager.LoadScene("Menu");
	}

	#region Timer

	//float realtimeSinceStartup;
	float secondLeft;

	/// <summary>
	/// Покадровый рассчет боя
	/// </summary>
	void TimeBattleCalc() {
		secondLeft -= Time.unscaledDeltaTime;
		if(battlePhase == BattlePhase.load) secondLeft = 3;
		OnChangeBattleTimer(Mathf.Max(0, secondLeft));
	}

	/// <summary>
	/// Время окончания боя
	/// </summary>
	public float timeEndBattle { get { return secondLeft; } }

	/// <summary>
	/// Отправка сообщения на сервер о окончании боя
	/// </summary>
	void BattleFinish() {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.battle.id);
		//Network.instance.BattleFinish(form, null);
	}

	#endregion

	#region Энергия

	/// <summary>
	/// Событие изменения скорости восстановления энергии
	/// </summary>
	/// <param name="newEnergySpeedRepeat">Скорость восстановления энергии</param>
	public static event Actione<float> EnergySpeedRepeatChange;

	/// <summary>
	/// Параметры восстановления энергии
	/// </summary>
	[System.Serializable]
	public struct EnegryRepeatParametrs {
		public float timeStart;
		public float repeatValue;
	}
	/// <summary>
	/// Параметры изменения энергии
	/// </summary>
	public EnegryRepeatParametrs[] enegryRepeatParametr;

	/// <summary>
	/// Время для следующей проверки изменения скорости восстановления энергии
	/// </summary>
	float? energyNextTimeCheck;

	/// <summary>
	/// Скорость восстановления энергии
	/// </summary>
	public static float energyRepeat;

	/// <summary>
	/// Инифиализация энергии
	/// </summary>
	void EnergyInit() {
		energyRepeat = enegryRepeatParametr[0].repeatValue;
		energyNextTimeCheck = enegryRepeatParametr[1].timeStart;
	}

	/// <summary>
	/// Обновление энергии каждый кадр
	/// </summary>
	void EnergyUpdate() {
		if(energyNextTimeCheck != null /*&& Time.time - timeStartBattle >= energyNextTimeCheck*/) {
			for(int i = 0; i < enegryRepeatParametr.Length; i++) {
				if(enegryRepeatParametr[i].timeStart == energyNextTimeCheck) {
					energyRepeat = enegryRepeatParametr[i].repeatValue;
					if(i == enegryRepeatParametr.Length - 1)
						energyNextTimeCheck = null;
					else
						energyNextTimeCheck = enegryRepeatParametr[i + 1].timeStart;

					if(EnergySpeedRepeatChange != null) EnergySpeedRepeatChange(energyRepeat);
				}
			}
		}
	}

	#endregion

}
