using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Singleton<Shop> {

  public DateTime startTime = DateTime.Now;
  public DateTime dateLastBye = DateTime.MinValue;
  public bool is30Percent {
    get {
      return DateTime.Now < dateLastBye.AddDays(3);
    }
  }

  public float countCoeff {
    get {
      return DateTime.Now < dateLastBye.AddDays(3) ? 1.3f : 1;
    }
  }

  public bool isSpecialProduct {
    get {
      return Shop.Instance.startTime.AddDays(1) > System.DateTime.Now;
    }
  }


  private void Start() {
    Load();
  }
  
  public void Save() {

    SaveData save = new SaveData() {
      dateLastBye = dateLastBye.ToString(),
      startTime = startTime.ToString()
    };

    PlayerPrefs.SetString("shop", Newtonsoft.Json.JsonConvert.SerializeObject(save));

  }

  public void Load() {

    if (!PlayerPrefs.HasKey("shop")) {
      startTime = DateTime.Now;

      Save();

      return;
    }

    string dataString = PlayerPrefs.GetString("shop");

    SaveData saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(dataString);

    dateLastBye = DateTime.Parse(saveData.dateLastBye);
    startTime = DateTime.Parse(saveData.startTime);

  }

  public void ByeProduct(BillingProductAbstract bill, System.Action callback) {
    BillingManager.Instance.ByeProduct(bill, callback);
  }

  public class SaveData {
    public string dateLastBye;
    public string startTime;
  }

  public void ByeComplete() {

    dateLastBye = DateTime.Now;

    Save();

    ExEvent.ShopEvents.OnProductBye.Call();
  }

}
