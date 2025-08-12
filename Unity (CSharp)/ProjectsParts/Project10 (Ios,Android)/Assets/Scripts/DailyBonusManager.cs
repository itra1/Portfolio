using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Менеджер дневного бонуса
/// </summary>
public class DailyBonusManager: Singleton<DailyBonusManager> {

  public static event Action OnChangePhase;

  public int[] dateParametrs;

  public DateTime nextShow { get; private set; }
  public DateTime lastShow { get; private set; }

  private Phases _phase;
  public Phases phase {
    get { return _phase; }
    set {
      //if (_phase == value) return;
      _phase = value;
      if (OnChangePhase != null) OnChangePhase();
    }
  }
  public enum Phases {
    none,
    wait,
    ready
  }

  private SaveData saves;

  private void Start() {
    Load();
  }

  public void Save() {
    saves.lastShow = lastShow.ToString();
    PlayerPrefs.SetString("daily", Newtonsoft.Json.JsonConvert.SerializeObject(saves));
  }

  public void Load() {

    if (PlayerPrefs.HasKey("daily")) {
      saves = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("daily"));
      lastShow = DateTime.Parse(saves.lastShow);
    } else {
      saves = new SaveData() {
        lastShow = DateTime.Now.ToString(),
        showCount = 0
      };
    }

    RecaldDailyBonus();
  }

  IEnumerator WaitCoroutine() {
    yield return new WaitForSecondsRealtime((nextShow - lastShow).Seconds);
    RecaldDailyBonus();
  }

  void RecaldDailyBonus() {
    saves.showCount = Math.Min(dateParametrs.Length - 1, saves.showCount);
    nextShow = lastShow.AddSeconds(dateParametrs[saves.showCount]);

    if (nextShow < DateTime.Now)
      phase = Phases.ready;
    else {
      phase = Phases.wait;
      StartCoroutine(WaitCoroutine());
    }

  }

  public void IconClick() {

    DailyBonus inst = UiController.ShowUi<DailyBonus>();
    inst.OnOpen = () => {
      saves.showCount++;
      lastShow = DateTime.Now;
      Save();
      RecaldDailyBonus();
    };
    inst.OnClose = () => {
      Destroy(inst);
    };
    inst.gameObject.SetActive(true);

  }

  [System.Serializable]
  private class SaveData {
    public string lastShow;
    public int showCount;
  }

}
