using UnityEngine;

/// <summary>
/// Подключение к бою
/// </summary>
public class ConnectedToBattle : Packet {

  string battleId;
  int team;

  public override void ReadImpl() {

    battleId = ReadASCII();
    team = ReadC();
  }

  public override void Process() {
    GameManager.instance.BattleJoinStart(battleId, team);
    //Debug.Log(Util.unixTimeUnvMilliseconds + " : "  + battleId + " : " + team);
  }
}
