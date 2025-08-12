using UnityEngine;

[System.Serializable]
public struct PlayerJoined {
  public string messageId;
  public string name;
  public int team;
  public float maxEnergy;
  public float currentEnergy;
}


public class PlayerJoinedToBattle : Packet {
  
  PlayerJoined playerJoided;

  public override void ReadImpl() {

    playerJoided = new PlayerJoined();

    playerJoided.messageId = ReadASCII();
    playerJoided.name = ReadUTF8();
    Debug.Log(playerJoided.name);
    playerJoided.team = ReadC();
    playerJoided.maxEnergy = ReadF();
    playerJoided.currentEnergy = ReadF();
  }

  public override void Process() {
    GameManager.instance.AddPlayers(playerJoided);
  }

  public string getType() {
    return "[S] 02:07 PlayerJoinedToBattle";
  }

}
