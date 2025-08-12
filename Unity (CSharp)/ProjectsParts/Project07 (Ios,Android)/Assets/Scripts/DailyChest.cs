using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyChest {

  public string title;
  [UUID]
  public string uuid;

  private string playerPrefsKey {  get {  return "daylyChest" + uuid;  }  }

  private DateTime lastOpen;

  public bool isReady {
    get {
      return DateTime.Now > lastOpen.AddDays(1);
    }
  }

  public TimeSpan timeWait {
    get {
      return lastOpen.AddDays(1) - DateTime.Now;
    }
  }

  public void Save() {
    PlayerPrefs.SetString(playerPrefsKey, lastOpen.ToString());
  }

  public void Load() {

    if (!PlayerPrefs.HasKey(playerPrefsKey)) {
      lastOpen = DateTime.Now;
      Save();
      return;
    }

    lastOpen = DateTime.Parse(PlayerPrefs.GetString(playerPrefsKey));

  }

  public void Bye() {
    lastOpen = DateTime.Now;
    Save();
  }

}
