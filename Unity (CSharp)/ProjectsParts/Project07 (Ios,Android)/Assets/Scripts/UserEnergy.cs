using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Энергия игрока
/// </summary>
public class UserEnergy: Singleton<UserEnergy> {

  // Дата последнего изменения энергии
  private System.DateTime dateChange = System.DateTime.MinValue;

  // Максимальное число энергии
  public float maxEnergy = 100;

  // Время в секундах на восстановление единицы энергии
  private float secondRestore = 60;

  // Количество необходимое для запуск боя
  public float priceBattle = 10;

  // Достаточно ли энергии для боя
  public bool ExistsEnergy {
    get {
      return _energy >= priceBattle;
    }
  }

  /// <summary>
  /// Использование игры
  /// </summary>
  /// <returns>Использовано</returns>
  public bool UseGame() {

    if (!ExistsEnergy) return false;

    _energy -= priceBattle;
    Save();
    InitRestore();

    return true;
  }

  // Время следующего изменения энергии
  public System.DateTime NextChangeTime {
    get {
      if (_energy == maxEnergy)
        return System.DateTime.MaxValue;

      return dateChange.AddSeconds(secondRestore);
    }
  }

  //изменение Energy событие
  public static event System.Action<float> EnergyChange;
  private float _energy;
  public float energy {
    get { return _energy; }
    set {
      _energy = value;
      if (_energy > 100)
        _energy = 100;
      Save();
      if (EnergyChange != null) EnergyChange(_energy);
    }
  }

  public float energyPlus {
    get { return _energy; }
    set {
      _energy = value;
      Save();
      if (EnergyChange != null) EnergyChange(_energy);
    }
  }

  private void Start() {
    Load();
    InitRestore();

    StartCoroutine(RepeatCoroutine());
  }

  IEnumerator RepeatCoroutine() {

    while (true) {

      InitRestore();
      yield return new WaitForSeconds(1);
    }

  }

  private void InitRestore() {

    System.TimeSpan spanTime = System.DateTime.Now - dateChange;

    int countInc = (int)(spanTime.TotalSeconds / secondRestore);

    if (countInc == 0)
      return;


    energy = Mathf.Min(energy + countInc, maxEnergy);
    dateChange = dateChange.AddSeconds(countInc * secondRestore);

    Save();
  }

  private void Save() {

    SaveData saveData = new SaveData() {
      energy = _energy,
      date = dateChange.ToString()
    };

    PlayerPrefs.SetString("energy", Newtonsoft.Json.JsonConvert.SerializeObject(saveData));

  }

  private void Load() {

    if (!PlayerPrefs.HasKey("energy")) {
      _energy = maxEnergy;
      dateChange = System.DateTime.Now;
      return;
    }

    SaveData saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("energy"));

    _energy = saveData.energy;
    dateChange = System.DateTime.Parse(saveData.date);

    InitRestore();
  }

  public class SaveData {
    public float energy;
    public string date;
  }

}
