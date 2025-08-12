using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Прогресс игрока
/// </summary>
[Serializable]
public class UserProgress
{
  public float energyMax = 100;                       // Максимальный запас энергии
  public float energyRecoverSpeed = 5;                // Скорость восстановления энергии
  public int experience;                              // Опыт персонажа
  public int playerLevel;                             // Общий уровень игрока
  public int powerLevel = 0;                          // Уровень силы
  public int energyLevel = 0;                         // Уровень энергии
  public int healthLevel = 0;                         // Уровень здоровья
  public int medicineChestNextTime;                   // Время следующей аптечки
  public int medicineChestTime;                       // Время рассчета аптечки
  public int medicineChestCount;                      // Количество аптечек
  public List<Game.User.WeaponUserData> lostWeaponList = new List<Game.User.WeaponUserData>();  // Список потерянного оружия	
  public float returnLostPercent = 0;

  public void Init() {
    experience = 0;
    powerLevel = 0;
    energyLevel = 0;
    healthLevel = 0;
  }

  public float EnergyMax {
    get { return energyMax + (energyMax * 0.1f * EnergyLevel); }
  }

  public float EnergyRecoverSpeed {
    get { return energyRecoverSpeed; }
  }

  public int PlayerLevel() {

    if (experience < 500)
      return 0;
    if (experience < 1000)
      return 1;
    if (experience < 1500)
      return 2;

    return GetPlayerLevelRecurs(500, 1000, 1500, 3);

  }

  private int GetPlayerLevelRecurs(int param1, int param2, int param3, int level) {
    int value = (int)(Mathf.Ceil(((param1 + param2 + param3) * 0.7f) / 100) * 100);

    return value <= experience ? GetPlayerLevelRecurs(param2, param3, value, ++level) : level;
  }

  // Уровень силы

  public int PowerLevel {
    get { return powerLevel; }
    set {
      powerLevel = value;
      ExEvent.UserEvents.OnPowerLevel.CallAsync(powerLevel);
    }
  }

  // Уровень энергии

  public int EnergyLevel {
    get { return energyLevel; }
    set {
      energyLevel = value;
      ExEvent.UserEvents.OnEnergyLevel.CallAsync(energyLevel);
    }
  }

  public int HealthLevel {
    get { return healthLevel; }
    set {
        healthLevel = value;
        ExEvent.UserEvents.OnHealthLevel.CallAsync(healthLevel);
    }
  }

    // Уровень жизней

    public int StatPriceLevel(int level) {
    return GameDesign.Instance.statsPrice[level];
  }

  int GetStatPriceLevel(int param1, int param2, int levelNeed, int level) {
    int value = (int)(Mathf.Round(((param1 + param2) * 0.75f) / 100) * 100);
    if (level != levelNeed)
      return GetStatPriceLevel(param2, value, levelNeed, ++level);
    else
      return value;
  }


}
