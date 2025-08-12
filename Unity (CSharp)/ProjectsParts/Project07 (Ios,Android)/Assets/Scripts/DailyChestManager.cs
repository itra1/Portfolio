using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyChestManager: Singleton<DailyChestManager> {

  public List<DailyChest> chestList;

  private void Start() {
    chestList.ForEach(x => x.Load());
  }

  public DailyChest GetChest(string uuid) {
    return chestList.Find(x => x.uuid == uuid);
  }

  public void ByeChest(string uuid) {
    chestList.Find(x => x.uuid == uuid).Bye();
  }

}
