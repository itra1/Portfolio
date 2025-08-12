using UnityEngine;
using System.Collections;

public class BattleJoinEnd : Packet {
  
  public override void Process() {
    GameManager.instance.BattleJoinEnd();
  }

}
