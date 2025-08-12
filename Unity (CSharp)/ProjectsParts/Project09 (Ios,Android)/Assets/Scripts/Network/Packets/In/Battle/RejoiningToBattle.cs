using UnityEngine;
using System.Collections;

public class RejoiningToBattle : Packet {

  public override void Process() {
    Debug.Log(Util.unixTimeUnvMilliseconds);
    GameManager.instance.PlayerAddInQueue();
  }
}
