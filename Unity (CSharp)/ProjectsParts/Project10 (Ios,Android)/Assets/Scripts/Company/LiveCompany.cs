using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Company.Live {

  public abstract class LiveCompany: Singleton<LiveCompany> {

    /// <summary>
    /// Событие изменения значения
    /// </summary>
    public static event Action<float> OnChange;

    /// <summary>
    /// Максимальное значение показателя
    /// </summary>
    public abstract float maxValue { get; }

    /// <summary>
    /// Стоимость одного забега
    /// </summary>
    public abstract float oneRunPrice { get; }

    /// <summary>
    /// Количество секунд необходимые до восстановления
    /// </summary>
    protected abstract float secondRepeat { get; }

    /// <summary>
    /// Актуальный показатель
    /// </summary>
    public float value;

    /// <summary>
    /// Дата окончания безлимита
    /// </summary>
    public DateTime unlimEnd;

    /// <summary>
    /// Оставшееся время безлимита
    /// </summary>
    public TimeSpan unlimRemain { get { return unlimEnd - DateTime.Now; } }
    public TimeSpan nextValue { get { return lastRepeat.AddHours(1) - DateTime.Now; } }

    /// <summary>
    /// Активный безлимит
    /// </summary>
    public virtual bool isUnlim { get { return _isUnlimited || unlimEnd >= DateTime.Now; } }

    public bool _isUnlimited = false;

    public bool isUnlimited {
      get { return _isUnlimited; }
      set {
        _isUnlimited = value;
        Save();
        if (OnChange != null) OnChange(this.value);
      }
    }

    /// <summary>
    /// Готов, достаточно значения для запуска
    /// </summary>
    public bool isReady { get { return isUnlim || value >= oneRunPrice || isUnlimited; } }

    /// <summary>
    /// Время последнего восстановления энергии
    /// </summary>
    private DateTime lastRepeat;

    protected virtual void Start() {
      Load();
    }

    protected override void OnDestroy() {
      base.OnDestroy();

    }

    protected Coroutine repeatCoroutine;
    protected virtual IEnumerator Repeat() {

      while (maxValue > value) {

        DateTime next = lastRepeat.AddSeconds(secondRepeat);

        if ((next - DateTime.Now).TotalSeconds <= 0) {

          value++;
          lastRepeat = next;
          Save();
        } else
          yield return new WaitForSecondsRealtime((float)(next - lastRepeat).TotalSeconds);
      }

    }

    protected virtual void Load() {

      if (!PlayerPrefs.HasKey("liveCompany")) {

        value = maxValue;
        unlimEnd = DateTime.MinValue;
        lastRepeat = DateTime.MinValue;
        _isUnlimited = false;
        return;
      }

      SaveData save = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("liveCompany"));

      value = save.value;
      lastRepeat = DateTime.Parse(save.lastRepeat);
      unlimEnd = DateTime.Parse(save.unlim);
      _isUnlimited = save.isUnlimited;

      if (repeatCoroutine == null)
        repeatCoroutine = StartCoroutine(Repeat());

    }

    public void Save() {

      SaveData save = new SaveData {
        value = value,
        lastRepeat = lastRepeat.ToString(),
        unlim = unlimEnd.ToString(),
        isUnlimited = _isUnlimited
      };

      PlayerPrefs.SetString("liveCompany", JsonUtility.ToJson(save));
    }

    /// <summary>
    /// Добавление значения
    /// </summary>
    /// <param name="hourAdded"></param>
    public void AddHourUnlim(int hourAdded) {
      unlimEnd = unlimEnd > DateTime.Now ? unlimEnd.AddHours(hourAdded) : DateTime.Now.AddHours(hourAdded);
      Save();
    }

    public void AddValue(float value) {
      this.value += value;
      this.value = Mathf.Min(this.value, this.maxValue);
      Save();
    }

    /// <summary>
    /// Используется забег
    /// </summary>
    public void UseRun() {

      if (isUnlimited || isUnlim || GameManager.activeLevelData.gameMode == GameMode.survival) return;

      bool isFull = maxValue == value;

      value -= oneRunPrice;

      if (maxValue > value) {

        if (isFull)
          lastRepeat = DateTime.Now;

        if (repeatCoroutine == null)
          repeatCoroutine = StartCoroutine(Repeat());
        Save();
      }

    }

  }

  public class SaveData {
    public float value;
    public string lastRepeat;
    public string unlim;
    public bool isUnlimited;
  }

}