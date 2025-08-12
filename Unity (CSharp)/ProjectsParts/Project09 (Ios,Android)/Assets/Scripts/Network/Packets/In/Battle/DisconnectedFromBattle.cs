using UnityEngine;
using System.Collections;

public class DisconnectedFromBattle : Packet {

  public static event Actione OnDisconnectBattle;

  public override void ReadImpl() {
    base.ReadImpl();
  }

  public override void Process() {
    if(OnDisconnectBattle != null) OnDisconnectBattle();
  }

}
