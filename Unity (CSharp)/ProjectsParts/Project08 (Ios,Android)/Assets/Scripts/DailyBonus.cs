using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Дневной бонус
/// </summary>
public class DailyBonus : Singleton<DailyBonus> {

	public static event Action<Phases> OnChangePhase;
	
	private Coroutine _waitCor;

	private string _pushId;
	
	public DateTime serverTime;
	public DateTime updateServerTime;
	private bool firstLoad = false;

	private DateTime _lastShow = DateTimeOffset.UtcNow.DateTime.AddHours(-24);
	private DateTime _nextShow = DateTimeOffset.UtcNow.DateTime;

	public bool isNetActive;
	
	public bool isDownload { get; set; }

	public DateTime nextShow {
		get { return _nextShow; }
		set { _nextShow = value; }
	}

	private void Start() {
		NetManager.Instance.GetTime(DownloadComplete, () => { });
		phase = Phases.none;
	}

	private void OnApplicationPause(bool pause) {
		if (!pause) {
			phase = Phases.none;
			NetManager.Instance.GetTime(DownloadComplete, () => { });
		}
	}
	
	private Phases _phase;

	public Phases phase {
		get { return _phase; }
		set {
			if (_phase == value) return;
			_phase = value;
			if (OnChangePhase != null) OnChangePhase(_phase);
		}
	}

	public enum Phases {
		none,
		wait,
		ready
	}
	
	public void DownloadComplete(string timeDownload) {
		if (!ExistBonusLevel()) return;

#if UNITY_EDITOR
		if(!isNetActive)
			timeDownload = "";
#endif

		isDownload = true;
		serverTime = DateTime.Parse(timeDownload);
		updateServerTime = DateTimeOffset.UtcNow.DateTime;
		if (!firstLoad) {
#if UNITY_EDITOR
			_lastShow = serverTime.AddHours(-24);
#else
			_lastShow = serverTime.AddHours(-24);
#endif
			firstLoad = true;
		}

		if ((serverTime - _lastShow).TotalHours < 0) {
			nextShow = serverTime;
		} else if ((serverTime - _lastShow).TotalHours > 24)
			nextShow = serverTime;
		else {
			nextShow = serverTime.AddHours(24 - (serverTime - _lastShow).TotalHours);
		}

		InitData();
		Save();
	}

	private bool ExistBonusLevel() {
		return true;
	}

	public void Save() {
		var data = JsonUtility.ToJson(new SaveDailyBonus() {
			lastShow = _lastShow.ToString(),
			pushId = _pushId,
			firstLoad = firstLoad
		});
		PlayerPrefs.SetString("dailyBonus", data);
	}

	public void Load() {

		if (!PlayerPrefs.HasKey("dailyBonus")) {
#if UNITY_EDITOR
			_lastShow = DateTimeOffset.UtcNow.DateTime.AddHours(-24);
#endif
			return;
		}

		SaveDailyBonus saveObject = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveDailyBonus>(PlayerPrefs.GetString("dailyBonus"));

		_lastShow = DateTime.Parse(saveObject.lastShow);
		_pushId = saveObject.pushId;
		firstLoad = saveObject.firstLoad;

		if (isDownload)
			InitData();
	}

	void InitData() {

		if (_nextShow > serverTime) {
			Wait();
			return;
		}

		if (_waitCor != null)
			StopCoroutine(_waitCor);

		Ready();
	}

	IEnumerator WaitShow(TimeSpan tagetTime) {
		yield return new WaitForSecondsRealtime((float)tagetTime.TotalSeconds);
		CheckReady();
	}

	void Wait() {
		_waitCor = StartCoroutine(WaitShow(_nextShow - serverTime));
		phase = Phases.wait;
	}

	private void CheckReady() {
		NetManager.Instance.GetTime(DownloadComplete, () => { });
	}

	void Ready() {
		PushManager.Instance.RemovePush(_pushId);
		phase = Phases.ready;
	}

	public void OpenDialog() {

		if (phase != Phases.ready) return;

		BonusLevelUi bonusDialog = UIManager.Instance.GetPanel(UiType.bonusLevelStart) as BonusLevelUi;
		bonusDialog.gameObject.SetActive(true);
		bonusDialog.Show();
		bonusDialog.OnPlay = () => {

			_lastShow = DateTimeOffset.UtcNow.DateTime;
			NetManager.Instance.GetTime((serverTime) => {
				this.serverTime= DateTime.Parse(serverTime);
				updateServerTime = DateTimeOffset.UtcNow.DateTime;
				_lastShow = this.serverTime;
				nextShow = _lastShow.AddDays(1);
				InitData();
				CreatePush(24);
				Save();
			}, () => {
				nextShow = _lastShow.AddDays(1);
				InitData();
				CreatePush(24);
				Save();
			});
			Save();

			GameManager.Instance.PlayBonusLevel();
			phase = Phases.wait;
		};
	}

	private void CreatePush(int hours) {
		
		string text = LanguageManager.GetTranslate((UnityEngine.Random.value <= 0.5f ? "push.dailyBonus1" : "push.dailyBonus2"));
		_pushId = PushManager.Instance.CreatePush(text, DateTime.Now.AddHours(hours));
	}

	public void GetGift() {
		//phase = Phases.none;
		GameManager.Instance.BackFromBonusLevel(() => {
			UIManager.Instance.GetPanel(UiType.dailyBonusConfirm).gameObject.SetActive(false);
		});

	}
	
}

[System.Serializable]
public class SaveDailyBonus {
	public string lastShow;
	public string pushId;
	public bool firstLoad;
}